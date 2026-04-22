using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Batch-processes all gameplay scenes:
///   Environment scenes  → BatchingStatic + OccluderStatic (large meshes) + OccludeeStatic + occlusion bake
///   Puzzle scenes       → OccludeeStatic only (NPCs/props benefit from environment occluders)
/// </summary>
public static class BatchSceneOptimizer
{
    private const string EnvironmentsFolder = "Assets/Scenes/Environments";
    private const string PuzzlesFolder      = "Assets/Scenes/Puzzles";

    // Mesh bounding-box volume (m³) required to act as an occluder.
    private const float OccluderMinVolume = 1.5f;

    // -------------------------------------------------------------------------

    [MenuItem("Tools/Batch Optimize All Scenes")]
    static void RunAll()
    {
        if (!PromptSave()) return;

        string originalScene = SceneManager.GetActiveScene().path;

        ProcessFolder(EnvironmentsFolder, isEnvironment: true,  bake: true);
        ProcessFolder(PuzzlesFolder,      isEnvironment: false, bake: false);

        if (!string.IsNullOrEmpty(originalScene))
            EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);

        Debug.Log("[BatchSceneOptimizer] All scenes processed.");
    }

    [MenuItem("Tools/Batch Optimize Environment Scenes Only")]
    static void RunEnvironmentsOnly()
    {
        if (!PromptSave()) return;

        string originalScene = SceneManager.GetActiveScene().path;
        ProcessFolder(EnvironmentsFolder, isEnvironment: true, bake: true);

        if (!string.IsNullOrEmpty(originalScene))
            EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);

        Debug.Log("[BatchSceneOptimizer] Environment scenes processed.");
    }

    [MenuItem("Tools/Batch Optimize Puzzle Scenes Only")]
    static void RunPuzzlesOnly()
    {
        if (!PromptSave()) return;

        string originalScene = SceneManager.GetActiveScene().path;
        ProcessFolder(PuzzlesFolder, isEnvironment: false, bake: false);

        if (!string.IsNullOrEmpty(originalScene))
            EditorSceneManager.OpenScene(originalScene, OpenSceneMode.Single);

        Debug.Log("[BatchSceneOptimizer] Puzzle scenes processed.");
    }

    // -------------------------------------------------------------------------

    static void ProcessFolder(string folder, bool isEnvironment, bool bake)
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { folder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log($"[BatchSceneOptimizer] Opening {path}");

            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

            if (isEnvironment)
                ApplyEnvironmentFlags();
            else
                ApplyPuzzleFlags();

            EditorSceneManager.SaveScene(scene);

            if (bake)
            {
                Debug.Log($"[BatchSceneOptimizer] Baking occlusion for {scene.name}...");
                StaticOcclusionCulling.Compute();
            }
        }
    }

    // Environment scenes: mark static geometry for batching and occlusion.
    static void ApplyEnvironmentFlags()
    {
        var renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        int batching = 0, occluders = 0, occludees = 0;

        foreach (var r in renderers)
        {
            GameObject go = r.gameObject;
            StaticEditorFlags existing = GameObjectUtility.GetStaticEditorFlags(go);
            StaticEditorFlags next = existing;

            // All renderers become occludees.
            next |= StaticEditorFlags.OccludeeStatic;
            occludees++;

            // Static-safe geometry gets batching flag.
            if (IsSafeToMakeStatic(go))
            {
                next |= StaticEditorFlags.BatchingStatic;
                batching++;

                // Large geometry also becomes an occluder.
                if (BoundsVolume(r) >= OccluderMinVolume)
                {
                    next |= StaticEditorFlags.OccluderStatic;
                    occluders++;
                }
            }

            if (next != existing)
                GameObjectUtility.SetStaticEditorFlags(go, next);
        }

        Debug.Log($"  Batching: {batching}  Occluders: {occluders}  Occludees: {occludees}");
    }

    // Puzzle scenes: only mark objects as occludees so they're culled by environment walls.
    static void ApplyPuzzleFlags()
    {
        var renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        int count = 0;

        foreach (var r in renderers)
        {
            GameObject go = r.gameObject;
            StaticEditorFlags existing = GameObjectUtility.GetStaticEditorFlags(go);
            StaticEditorFlags next = existing | StaticEditorFlags.OccludeeStatic;

            if (next != existing)
            {
                GameObjectUtility.SetStaticEditorFlags(go, next);
                count++;
            }
        }

        Debug.Log($"  OccludeeStatic applied to {count} object(s).");
    }

    // -------------------------------------------------------------------------

    static readonly HashSet<string> DynamicTags = new HashSet<string>
    {
        "NPC", "Enemy", "Ally", "Enemies", "Allies",
        "Bullets", "Grenades", "Mines", "Missiles",
        "ExplosiveBarrels", "Chandeliers", "Boxes", "Shelves",
        "Lethal", "Glass",
    };

    static bool IsSafeToMakeStatic(GameObject go)
    {
        int layer = go.layer;
        if (layer == LayerMask.NameToLayer("Player") ||
            layer == LayerMask.NameToLayer("PlayerBoundary") ||
            layer == LayerMask.NameToLayer("InteractionObjects"))
            return false;

        if (go.GetComponentInParent<Rigidbody>() != null) return false;
        if (go.GetComponentInChildren<Rigidbody>() != null) return false;
        if (DynamicTags.Contains(go.tag)) return false;
        if (go.TryGetComponent<ParticleSystem>(out _)) return false;

        return true;
    }

    static float BoundsVolume(MeshRenderer r)
    {
        Vector3 s = r.bounds.size;
        return s.x * s.y * s.z;
    }

    static bool PromptSave()
    {
        return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    }
}
