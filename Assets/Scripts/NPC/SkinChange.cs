using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinChange : MonoBehaviour
{
    [Header("Skins (Humanoid FBX prefabs)")]
    [SerializeField] private List<GameObject> skins = new List<GameObject>();
    [SerializeField] private int activeSkinIndex = 0;

    [Header("Refs")]
    [SerializeField] private Animator targetAnimator;        // single animator on Enemy
    [SerializeField] private Transform modelContainer;       // holder for spawned skin (zeroed)

    [Header("Weapon")]
    [SerializeField] private Transform weapon;               // Gun root in the scene (NOT under ModelContainer)
    [SerializeField] private HumanBodyBones handBone = HumanBodyBones.RightHand;
    [SerializeField] private bool preferWeaponMount = true;  // use RightHand/WeaponMount if present
    [SerializeField] private bool waitOneFrameBeforeAttach = true;

    [Header("Optional saved grip (use only if you want per-rig offsets)")]
    [SerializeField] private bool useSavedGrip = false;      // set false if you have a good WeaponMount
    [SerializeField] private Vector3 savedLocalPos;
    [SerializeField] private Vector3 savedLocalEuler;
    [SerializeField] private Vector3 savedLocalScale = Vector3.one;

    private Avatar originalAvatar;
    private SkinnedMeshRenderer[] defaultRenderers;

    void Awake()
    {
        if (!targetAnimator)
            targetAnimator = GetComponentInChildren<Animator>(true);

        if (!modelContainer)
        {
            var t = transform.Find("_ModelContainer");
            modelContainer = t ? t : new GameObject("_ModelContainer").transform;
            modelContainer.SetParent(transform, false);
        }

        // ensure clean TRS on the holder
        modelContainer.localPosition = Vector3.zero;
        modelContainer.localRotation = Quaternion.identity;
        modelContainer.localScale = Vector3.one;

        originalAvatar = targetAnimator ? targetAnimator.avatar : null;
        defaultRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);

        // sanity: weapon should NOT be inside modelContainer at edit time
        if (weapon && weapon.IsChildOf(modelContainer))
            weapon.SetParent(transform, true);
    }

    void Start()
    {
        ApplySkinByIndex(activeSkinIndex);
    }

    [ContextMenu("Apply Next Skin")]
    public void ApplyNextSkin()
    {
        if (skins.Count == 0) return;
        activeSkinIndex = (activeSkinIndex + 1) % skins.Count;
        ApplySkinByIndex(activeSkinIndex);
    }

    [ContextMenu("Revert To Default")]
    public void RevertToDefault()
    {
        ClearContainer();

        if (targetAnimator)
        {
            targetAnimator.avatar = originalAvatar;
            targetAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            targetAnimator.Rebind();
            targetAnimator.Update(0f);
        }

        if (defaultRenderers != null)
        {
            foreach (var r in defaultRenderers)
                if (r) r.enabled = true;
        }

        // detach weapon back to root (optional)
        if (weapon) weapon.SetParent(transform, true);
    }

    public void ApplySkinByIndex(int index)
    {
        if (skins == null || skins.Count == 0) return;
        if (index < 0 || index >= skins.Count) index = 0;

        // keep weapon safe from ClearContainer()
        if (weapon && weapon.IsChildOf(modelContainer))
            weapon.SetParent(transform, true);

        // hide default renderers (if any baked into Enemy)
        if (defaultRenderers != null)
            foreach (var r in defaultRenderers) if (r) r.enabled = false;

        // nuke old skin
        ClearContainer();

        // spawn new skin
        var prefab = skins[index];
        if (!prefab) return;

        var inst = Instantiate(prefab, modelContainer);
        inst.name = prefab.name + "_Instance";
        inst.transform.localPosition = Vector3.zero;
        inst.transform.localRotation = Quaternion.identity;
        inst.transform.localScale = Vector3.one;

        // adopt humanoid avatar from skin; remove extra animators on the spawned model
        var srcAnimator = inst.GetComponentInChildren<Animator>(true);
        if (targetAnimator)
        {
            if (srcAnimator && srcAnimator.avatar && srcAnimator.avatar.isHuman && srcAnimator.avatar.isValid)
                targetAnimator.avatar = srcAnimator.avatar;

            var extras = inst.GetComponentsInChildren<Animator>(true);
            foreach (var a in extras) DestroyImmediate(a);

            targetAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            targetAnimator.Rebind();
            targetAnimator.Update(0f);
        }

        // ensure SMRs bind to our Animator
        RemapAllSMRs(inst, targetAnimator);

        // attach weapon after bones are valid
        if (weapon && targetAnimator)
        {
            if (waitOneFrameBeforeAttach) StartCoroutine(AttachWeaponNextFrame());
            else AttachWeaponNow();
        }
    }

    private IEnumerator AttachWeaponNextFrame()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        AttachWeaponNow();
    }

    private void AttachWeaponNow()
    {
        if (!weapon || !targetAnimator) return;

        var hand = targetAnimator.GetBoneTransform(handBone);
        if (!hand)
        {
            Debug.LogWarning("[SkinChange] Hand bone not found; cannot attach weapon.");
            return;
        }

        // find or create mount
        Transform mount = hand;
        if (preferWeaponMount)
        {
            var found = hand.Find("WeaponMount");
            if (found) mount = found;
            else
            {
                // create a clean mount if missing
                var go = new GameObject("WeaponMount");
                mount = go.transform;
                mount.SetParent(hand, false);
                mount.localPosition = Vector3.zero;
                mount.localRotation = Quaternion.identity;
                mount.localScale = Vector3.one;
            }
        }

        // parent without keeping world space
        weapon.SetParent(mount, false);

        if (useSavedGrip)
        {
            // reapply captured local pose (per-rig/per-weapon offsets)
            weapon.localPosition = savedLocalPos;
            weapon.localEulerAngles = savedLocalEuler;
            weapon.localScale = savedLocalScale;
        }
        else
        {
            // zeroed: rely on WeaponMount's baked orientation
            weapon.localPosition = Vector3.zero;
            weapon.localRotation = Quaternion.identity;
            weapon.localScale = Vector3.one;
        }
    }

    private void ClearContainer()
    {
        for (int i = modelContainer.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(modelContainer.GetChild(i).gameObject);
            else Destroy(modelContainer.GetChild(i).gameObject);
#else
            Destroy(modelContainer.GetChild(i).gameObject);
#endif
        }
    }

    private void RemapAllSMRs(GameObject root, Animator anim)
    {
        if (!root || !anim) return;

        // map by transform name
        var map = new Dictionary<string, Transform>(256);
        foreach (var t in anim.GetComponentsInChildren<Transform>(true))
            if (!map.ContainsKey(t.name)) map.Add(t.name, t);

        var hips = anim.GetBoneTransform(HumanBodyBones.Hips) ?? anim.transform;

        var smrs = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (var r in smrs)
        {
            var orig = r.bones;
            var remap = new Transform[orig.Length];
            for (int i = 0; i < orig.Length; i++)
            {
                var b = orig[i];
                if (b && map.TryGetValue(b.name, out var m)) remap[i] = m;
                else remap[i] = b;
            }
            r.bones = remap;
            r.rootBone = hips;

            // safe defaults to avoid culled/zero bounds
            r.updateWhenOffscreen = true;
            if (r.localBounds.size.sqrMagnitude < 1e-6f)
                r.localBounds = new Bounds(Vector3.zero, Vector3.one * 2f);
        }
    }

    // -------- Optional: capture current perfect pose to saved fields --------
    [ContextMenu("Capture Current Weapon Local Pose To Saved Fields")]
    private void CaptureCurrentWeaponPose()
    {
        if (!weapon || !targetAnimator) return;

        var hand = targetAnimator.GetBoneTransform(handBone);
        if (!hand) return;

        var mount = preferWeaponMount ? (hand.Find("WeaponMount") ?? hand) : hand;

        // ensure it is under the same parent to read local space correctly
        if (weapon.parent != mount) weapon.SetParent(mount, true);

        savedLocalPos = weapon.localPosition;
        savedLocalEuler = weapon.localEulerAngles;
        savedLocalScale = weapon.localScale;
        useSavedGrip = true;

        Debug.Log("[SkinChange] Saved weapon local pose: pos=" + savedLocalPos + " rot=" + savedLocalEuler + " scale=" + savedLocalScale);
    }
}
