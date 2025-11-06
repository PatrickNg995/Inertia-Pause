using UnityEngine;

public class WeaponAutoAttach : MonoBehaviour
{
    public Animator animator;          // character animator
    public Transform weapon;           // gun root
    public HumanBodyBones handBone = HumanBodyBones.RightHand;
    public Vector3 savedLocalPos;      // captured once in T-pose
    public Vector3 savedLocalEuler;    // captured once in T-pose
    public Vector3 savedLocalScale = Vector3.one;

    void OnEnable() { StartCoroutine(AttachNextFrame()); }

    System.Collections.IEnumerator AttachNextFrame()
    {
        // allow Rebind / avatar update to finish
        yield return null;
        yield return new WaitForEndOfFrame();

        var hand = animator.GetBoneTransform(handBone);
        if (!hand) yield break;

        var mount = hand.Find("WeaponMount");
        if (!mount) { mount = new GameObject("WeaponMount").transform; mount.SetParent(hand, false); }

        weapon.SetParent(mount, false);

        // compensate for scaled armatures
        var p = mount.lossyScale; if (p == Vector3.zero) p = Vector3.one;
        weapon.localScale = savedLocalScale; // use 1,1,1 if you want gun’s own scale
        weapon.localPosition = savedLocalPos;
        weapon.localEulerAngles = savedLocalEuler;
    }
}
