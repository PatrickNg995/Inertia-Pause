using System.Collections.Generic;
using UnityEngine;

public class SkinChange : MonoBehaviour
{
    [Header("Skins (Humanoid FBX prefabs)")]
    [SerializeField] private List<GameObject> skins = new List<GameObject>();
    [SerializeField] private int activeSkinIndex = 0;

    [Header("References (auto-found if empty)")]
    [SerializeField] private Animator targetAnimator;     // your main Animator (kept)
    [SerializeField] private Transform modelContainer;    // where the spawned skin sits

    // default state cache for revert
    private Avatar originalAvatar;
    private SkinnedMeshRenderer[] defaultRenderers;

    private void Awake()
    {
        if (!targetAnimator)
            targetAnimator = GetComponentInChildren<Animator>(true);

        if (!modelContainer)
        {
            var t = transform.Find("_ModelContainer");
            modelContainer = t ? t : new GameObject("_ModelContainer").transform;
            modelContainer.SetParent(transform, false);
            modelContainer.localPosition = Vector3.zero;
            modelContainer.localRotation = Quaternion.identity;
            modelContainer.localScale = Vector3.one;
        }

        originalAvatar = targetAnimator ? targetAnimator.avatar : null;

        // cache all existing SMRs present at startup as the "default body"
        defaultRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
    }

    private void Start()
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
        // remove spawned skin
        ClearContainer();

        // restore default avatar and renderers
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
    }

    public void ApplySkinByIndex(int index)
    {
        if (skins == null || skins.Count == 0) return;
        if (index < 0 || index >= skins.Count) index = 0;

        var prefab = skins[index];
        if (!prefab) return;

        // hide default body renderers
        if (defaultRenderers != null)
        {
            foreach (var r in defaultRenderers)
                if (r) r.enabled = false;
        }

        // clear any previous skin and spawn a new one
        ClearContainer();

        var instance = Instantiate(prefab, modelContainer);
        instance.name = prefab.name + "_Instance";
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        // grab avatar from the spawned model's Animator (humanoid) and apply to the main Animator
        var srcAnimator = instance.GetComponentInChildren<Animator>(true);
        if (targetAnimator)
        {
            if (srcAnimator && srcAnimator.avatar && srcAnimator.avatar.isHuman && srcAnimator.avatar.isValid)
            {
                targetAnimator.avatar = srcAnimator.avatar;
            }

            // remove extra animators on the spawned model to avoid double-driving
            var extraAnims = instance.GetComponentsInChildren<Animator>(true);
            foreach (var a in extraAnims) DestroyImmediate(a);

            // ensure immediate evaluation with the new avatar/skeleton
            targetAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            targetAnimator.Rebind();
            targetAnimator.Update(0f);
        }

        // optional renderer safeties
        var smrs = instance.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (var r in smrs)
        {
            r.updateWhenOffscreen = true;
            if (r.localBounds.size.sqrMagnitude < 0.0001f)
                r.localBounds = new Bounds(Vector3.zero, Vector3.one * 2f);
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
}
