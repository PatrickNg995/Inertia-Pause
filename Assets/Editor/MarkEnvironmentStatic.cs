using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MarkEnvironmentStatic
{
    // Tags that belong to dynamic/living objects.
    private static readonly HashSet<string> _dynamicTags = new HashSet<string>
    {
        "NPC", "Enemy", "Ally", "Enemies", "Allies",
        "Bullets", "Grenades", "Mines", "Missiles",
        "ExplosiveBarrels", "Chandeliers", "Boxes", "Shelves",
        "Lethal", "Glass",
    };

    [MenuItem("Tools/Mark Environment Geometry Static")]
    static void Execute()
    {
        var renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);

        var toMark = new List<GameObject>();
        foreach (var r in renderers)
        {
            if (ShouldMark(r.gameObject))
                toMark.Add(r.gameObject);
        }

        if (toMark.Count == 0)
        {
            Debug.Log("[MarkEnvironmentStatic] Nothing new to mark.");
            return;
        }

        Undo.RecordObjects(toMark.ToArray(), "Mark Environment Geometry Static");

        foreach (var go in toMark)
        {
            var existing = GameObjectUtility.GetStaticEditorFlags(go);
            GameObjectUtility.SetStaticEditorFlags(go, existing | StaticEditorFlags.BatchingStatic);
        }

        Debug.Log($"[MarkEnvironmentStatic] Marked {toMark.Count} renderers as BatchingStatic.");
    }

    static bool ShouldMark(GameObject go)
    {
        // Already batching-static — nothing to do.
        if ((GameObjectUtility.GetStaticEditorFlags(go) & StaticEditorFlags.BatchingStatic) != 0)
            return false;

        // Skip objects on dynamic layers.
        int layer = go.layer;
        if (layer == LayerMask.NameToLayer("Player") ||
            layer == LayerMask.NameToLayer("PlayerBoundary") ||
            layer == LayerMask.NameToLayer("InteractionObjects"))
            return false;

        // Skip if the object or any ancestor has a Rigidbody (physics-driven = dynamic).
        if (go.GetComponentInParent<Rigidbody>() != null)
            return false;

        // Skip if any child has a Rigidbody (parent of a ragdoll or physics cluster).
        if (go.GetComponentInChildren<Rigidbody>() != null)
            return false;

        // Skip objects with dynamic tags.
        if (_dynamicTags.Contains(go.tag))
            return false;

        // Skip particle systems — they're always dynamic.
        if (go.TryGetComponent<ParticleSystem>(out _))
            return false;

        return true;
    }
}
