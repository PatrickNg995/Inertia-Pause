// SkinChange.cs
// Purpose: Swap ONLY the mesh (skin) on a character that already has a working armature/Animator.
// Usage:
// 1) On your animated character, add this script.
// 2) Set `targetRenderer` to the current SkinnedMeshRenderer (the one that already animates correctly).
// 3) Set `sourceMeshPrefab` to a prefab or FBX instance that contains ONLY a SkinnedMeshRenderer (no armature).
// 4) Press Play and call ApplyNewSkin() via the ContextMenu button or from your own code.
//
// Notes:
// - The new mesh must be weighted to a skeleton with the same bone names as your existing rig.
// - We re-use the target's bones/rootBone so animations continue to work.
// - We avoid System.Diagnostics.Debug and System.Net.Mime.* collisions by fully qualifying Unity types.

using System;
using System.Linq;
using UnityEngine;

public class SkinChange : MonoBehaviour
{
    [Header("Existing character components")]
    [Tooltip("The SkinnedMeshRenderer that is currently visible/animated on THIS character.")]
    public SkinnedMeshRenderer targetRenderer;

    [Header("New skin source (mesh only, no armature)")]
    [Tooltip("A prefab or FBX instance that has a SkinnedMeshRenderer with the NEW mesh. It can sit anywhere in the Project.")]
    public SkinnedMeshRenderer sourceMeshPrefab;

    [Header("Options")]
    [Tooltip("If true, we replace the mesh IN-PLACE on the targetRenderer. If false, we instantiate a parallel renderer and disable the old one.")]
    public bool replaceInPlace = true;

    [Tooltip("Copy materials from the source mesh to the target.")]
    public bool copyMaterials = true;

    [Tooltip("Show the old mesh after swap? Usually keep this OFF.")]
    public bool keepOldVisible = false;

    [Tooltip("If true, disable shadow casting/receiving on the new renderer (useful for debugging).")]
    public bool disableShadowsOnNew = false;

    [Tooltip("If true, set updateWhenOffscreen to avoid culling glitches during testing.")]
    public bool updateWhenOffscreen = false;

    // Prevent ambiguous references: always qualify Unity types we had conflicts with.
    private static void Log(string msg) => UnityEngine.Debug.Log($"[SkinChange] {msg}");
    private static void LogWarning(string msg) => UnityEngine.Debug.LogWarning($"[SkinChange] {msg}");
    private static void LogError(string msg) => UnityEngine.Debug.LogError($"[SkinChange] {msg}");

    private void Reset()
    {
        // Try to auto-find a SkinnedMeshRenderer on this GameObject or its children.
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<SkinnedMeshRenderer>(true);
    }

    [ContextMenu("Apply New Skin (once)")]
    public void ApplyNewSkin()
    {
        if (targetRenderer == null)
        {
            LogError("TargetRenderer is not assigned. Drag the existing character's SkinnedMeshRenderer here.");
            return;
        }
        if (sourceMeshPrefab == null)
        {
            LogError("SourceMeshPrefab is not assigned. Drag a prefab/FBX instance that has the NEW SkinnedMeshRenderer.");
            return;
        }

        // Ensure the source has a mesh to copy.
        var srcMesh = sourceMeshPrefab.sharedMesh;
        if (srcMesh == null)
        {
            LogError("SourceMeshPrefab has no sharedMesh. Make sure it has a SkinnedMeshRenderer with a Mesh.");
            return;
        }

        // (A) Replace in place: keep the existing renderer, reuse its bones, only swap its mesh & materials.
        if (replaceInPlace)
        {
            Log("Replacing mesh in-place on targetRenderer.");
            SwapInPlace(targetRenderer, sourceMeshPrefab);
            return;
        }

        // (B) Parallel instance: instantiate a new renderer as a sibling so transform stays local (no (0,0,0) world issue).
        Log("Instantiating a parallel SkinnedMeshRenderer and reusing target bones.");
        var newRenderer = Instantiate(sourceMeshPrefab, targetRenderer.transform.parent);
        newRenderer.gameObject.name = sourceMeshPrefab.gameObject.name + "_Live";

        // Match local transform 1:1 with the old renderer so it aligns correctly.
        newRenderer.transform.SetLocalPositionAndRotation(
            targetRenderer.transform.localPosition,
            targetRenderer.transform.localRotation
        );
        newRenderer.transform.localScale = targetRenderer.transform.localScale;

        // Critical: rebind to the existing armature
        newRenderer.rootBone = targetRenderer.rootBone;
        newRenderer.bones = targetRenderer.bones;

        // Mesh & materials
        newRenderer.sharedMesh = srcMesh;
        if (copyMaterials)
            newRenderer.sharedMaterials = sourceMeshPrefab.sharedMaterials;

        // Optional renderer flags
        newRenderer.updateWhenOffscreen = updateWhenOffscreen;
        if (disableShadowsOnNew)
        {
            newRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            newRenderer.receiveShadows = false;
        }

        // Hide or keep the original
        targetRenderer.enabled = keepOldVisible;

        // Copy bounds to reduce pop-in/culling issues
        newRenderer.localBounds = ComputeSafeBounds(newRenderer);

        Log("Skin swap complete (parallel renderer).");
    }

    private void SwapInPlace(SkinnedMeshRenderer target, SkinnedMeshRenderer source)
    {
        // Keep the existing bones/rootBone and Animator; only swap mesh & materials.
        target.sharedMesh = source.sharedMesh;

        if (copyMaterials)
            target.sharedMaterials = source.sharedMaterials;

        target.updateWhenOffscreen = updateWhenOffscreen;
        if (disableShadowsOnNew)
        {
            target.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            target.receiveShadows = false;
        }

        // Refresh bounds to avoid culling issues after mesh change.
        target.localBounds = ComputeSafeBounds(target);

        Log("Skin swap complete (in-place).");
    }

    /// <summary>
    /// Builds a conservative bounds if the mesh doesn't have good import-time bounds.
    /// </summary>
    private static Bounds ComputeSafeBounds(SkinnedMeshRenderer smr)
    {
        var mesh = smr.sharedMesh;
        if (mesh == null || mesh.vertexCount == 0)
            return smr.localBounds;

        // Use mesh bounds as a baseline, mildly inflated.
        var b = mesh.bounds;
        const float inflate = 1.05f;
        b.Expand(new Vector3(b.size.x * (inflate - 1f), b.size.y * (inflate - 1f), b.size.z * (inflate - 1f)));
        return b;
    }

    // (Optional) Quick sanity checks for common pitfalls.
    [ContextMenu("Debug: Print Bone Summary")]
    private void PrintBoneSummary()
    {
        if (targetRenderer == null)
        {
            LogWarning("No targetRenderer assigned.");
            return;
        }

        var bones = targetRenderer.bones;
        var root = targetRenderer.rootBone;
        Log($"Target bones: {bones?.Length ?? 0}, rootBone={(root ? root.name : "NULL")}");
        if (bones != null && bones.Length > 0)
            Log($"First bones: {string.Join(", ", bones.Take(6).Select(b => b ? b.name : "null"))} ...");
    }
}
