using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneManager : MonoBehaviour
{
    [SerializeField] private Scene _additiveScene;
    [SerializeField] private Scene _menuUIScene;

    private Scene _loadedScenarioAssetsScene;

    /// <summary>
    /// Unloads all scenes, then loads the main menu, which consists of the UI scene, environment scene and assets scene.
    /// </summary>
    /// <param name="environmentScene">A reference to the environment scene to be loaded, which consists of the level environment, terrain, and lighting.</param>
    /// <param name="scenarioAssetsScene">A reference to the scenario assets scene to be loaded, which consists of puzzle objects and dynamic camera angles.</param>
    public void LoadMainMenu(Scene environmentScene, Scene scenarioAssetsScene)
    {
        _loadedScenarioAssetsScene = scenarioAssetsScene;
        StartCoroutine(WithLoadingScreen(LoadScenesOnlyAsync(_menuUIScene, environmentScene, scenarioAssetsScene)));
    }

    /// <summary>
    /// Unloads all scenes, then loads a scenario, which consists of an environment scene and assets scene. If the scenes are already loaded, they are
    /// not loaded again.
    /// </summary>
    /// <param name="environmentScene">A reference to the environment scene to be loaded, which consists of the level environment, terrain, and lighting.</param>
    /// <param name="scenarioAssetsScene">A reference to the scenario assets scene to be loaded, which consists of puzzle objects and scripting.</param>
    public void LoadScenario(Scene environmentScene, Scene scenarioAssetsScene)
    {
        _loadedScenarioAssetsScene = scenarioAssetsScene;
        StartCoroutine(WithLoadingScreen(LoadScenesOnlyAsync(environmentScene, scenarioAssetsScene)));
    }

    /// <summary>
    /// Restarts a scenario, which reloads the assets scene but keeps the environment loaded.
    /// </summary>
    public void ReloadScenario()
    {
        StartCoroutine(WithLoadingScreen(ReloadScenarioAssets()));
    }

    private IEnumerator WithLoadingScreen(IEnumerator loadingRoutine)
    {
        StartLoad();
        yield return loadingRoutine;
        EndLoad();
    }

    // Shows loading screen.
    private void StartLoad()
    {
        // Show loading screen
    }

    // Hides loading screen.
    private void EndLoad()
    {
        // Hide loading screen
    }

    // Unloads all scenes except those that are going to be loaded, then load the scenes requested.
    private IEnumerator LoadScenesOnlyAsync(params Scene[] scenes)
    {
        IEnumerable<Scene> currentlyLoadedScenes = GetAllLoadedScenes().Where(scene => scene != _additiveScene);
        IEnumerable<Scene> scenesToUnload = currentlyLoadedScenes.Except(scenes);
        yield return UnloadScenesAsync(scenesToUnload);

        IEnumerable<Scene> scenesToLoad = scenes.Except(currentlyLoadedScenes);
        yield return LoadScenesAsync(scenesToLoad);
    }

    // Gets a list of all loaded scenes.
    private IEnumerable<Scene> GetAllLoadedScenes()
    {
        List<Scene> loadedScenes = new ();

        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            loadedScenes.Add(SceneManager.GetSceneAt(i));
        }

        return loadedScenes;
    }

    // Unloads multiple scenes concurrently.
    private IEnumerator UnloadScenesAsync(IEnumerable<Scene> scenes)
    {
        AsyncOperation[] operations = new AsyncOperation[scenes.Count()];

        for (int i = 0; i < scenes.Count(); i++)
        {
            Scene scene = scenes.ElementAt(i);
            operations[i] = SceneManager.UnloadSceneAsync(scene);
        }

        foreach (AsyncOperation operation in operations)
        {
            yield return operation;
        }
    }

    // Loads multiple scenes concurrently.
    private IEnumerator LoadScenesAsync(IEnumerable<Scene> scenes)
    {
        AsyncOperation[] operations = new AsyncOperation[scenes.Count()];

        for (int i = 0; i < scenes.Count(); i++)
        {
            Scene scene = scenes.ElementAt(i);
            operations[i] = SceneManager.LoadSceneAsync(scene.buildIndex, LoadSceneMode.Additive);
            operations[i].allowSceneActivation = false;
        }

        foreach (AsyncOperation operation in operations)
        {
            yield return new WaitUntil(() => operation.progress >= 0.9f);
            operation.allowSceneActivation = true;
        }
    }

    // Unloads the scenario assets scene then loads it again.
    private IEnumerator ReloadScenarioAssets()
    {
        List<Scene> scene = new () { _loadedScenarioAssetsScene };

        yield return UnloadScenesAsync(scene);
        yield return LoadScenesAsync(scene);
    }
}
