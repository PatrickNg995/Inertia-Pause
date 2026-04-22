using UnityEditor;
using UnityEngine;

public static class SetupOcclusionCulling
{
    // Mesh bounds volume (m³) required before an object is used as an occluder.
    // Small objects (boxes, NPCs) won't waste bake time as occluders.
    private const float OccluderMinVolume = 1.5f;

    [MenuItem("Tools/Setup Occlusion Culling and Bake")]
    static void SetupAndBake()
    {
        int occluders = Apply();
        Debug.Log($"[OcclusionCulling] Baking with {occluders} occluder(s)...");
        StaticOcclusionCulling.Compute();
    }

    [MenuItem("Tools/Setup Occlusion Culling (flags only, no bake)")]
    static void SetupOnly()
    {
        int occluders = Apply();
        Debug.Log($"[OcclusionCulling] Flags set — {occluders} occluder(s). Run bake manually via Rendering > Occlusion Culling.");
    }

    static int Apply()
    {
        var renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        int occluderCount = 0;

        foreach (var r in renderers)
        {
            GameObject go = r.gameObject;
            StaticEditorFlags existing = GameObjectUtility.GetStaticEditorFlags(go);
            StaticEditorFlags next = existing;

            // Every renderer should be an occludee — it can be hidden when behind an occluder.
            next |= StaticEditorFlags.OccludeeStatic;

            // Only large, already-batching-static geometry becomes an occluder.
            // Dynamic objects (NPCs, physics props) are excluded because they're not BatchingStatic.
            bool isBatchingStatic = (existing & StaticEditorFlags.BatchingStatic) != 0;
            if (isBatchingStatic && BoundsVolume(r) >= OccluderMinVolume)
            {
                next |= StaticEditorFlags.OccluderStatic;
                occluderCount++;
            }

            if (next != existing)
                GameObjectUtility.SetStaticEditorFlags(go, next);
        }

        return occluderCount;
    }

    static float BoundsVolume(MeshRenderer r)
    {
        Vector3 s = r.bounds.size;
        return s.x * s.y * s.z;
    }
}
