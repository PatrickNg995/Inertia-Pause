using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneManager : MonoBehaviour
{
    /// <summary>
    /// Invoked when loading starts.
    /// </summary>
    public Action OnStartLoad;

    /// <summary>
    /// Invoked when all scenes have finished loading.
    /// </summary>
    public Action OnEndLoad;

    private const string MAIN_MENU_UI = "MainMenu";
    private const string MAIN_MENU_ENVIRONMENT = "2-office";
    private const string MAIN_MENU_SCENARIO = "MainMenuScenario";

    private Scene _additiveScene;
    private string _loadedScenarioAssetsSceneName;

    void Start()
    {
        _additiveScene = SceneManager.GetActiveScene();
        LoadMainMenu();
    }

    /// <summary>
    /// Unloads all scenes, then loads the main menu and its environment and asset scenes.
    /// </summary>
    public void LoadMainMenu()
    {
        Debug.Log("Loading main menu scenes");
        _loadedScenarioAssetsSceneName = MAIN_MENU_SCENARIO;
        StartCoroutine(WithLoadingScreen(LoadScenesOnlyAsync(MAIN_MENU_UI, MAIN_MENU_ENVIRONMENT, MAIN_MENU_SCENARIO)));
    }

    /// <summary>
    /// Unloads all scenes, then loads a scenario, which consists of an environment scene and assets scene. If the scenes are already loaded, they are
    /// not loaded again.
    /// </summary>
    /// <param name="environmentSceneName">A reference to the environment scene to be loaded, which consists of the level environment, terrain, and lighting.</param>
    /// <param name="scenarioAssetsSceneName">A reference to the scenario assets scene to be loaded, which consists of puzzle objects and scripting.</param>
    public void LoadScenario(string environmentSceneName, string scenarioAssetsSceneName)
    {
        Debug.Log($"Loading scenario scenes: {environmentSceneName}, {scenarioAssetsSceneName}");
        _loadedScenarioAssetsSceneName = scenarioAssetsSceneName;
        StartCoroutine(WithLoadingScreen(LoadScenesOnlyAsync(scenarioAssetsSceneName, environmentSceneName)));
    }

    /// <summary>
    /// Restarts a scenario, which reloads the assets scene but keeps the environment loaded.
    /// </summary>
    public void ReloadScenario()
    {
        Debug.Log($"Reloading scenario scene {_loadedScenarioAssetsSceneName}");
        StartCoroutine(WithLoadingScreen(ReloadScenarioAssets()));
    }

    // Runs loadingRoutine with a loading screen.
    private IEnumerator WithLoadingScreen(IEnumerator loadingRoutine)
    {
        StartLoad();
        yield return loadingRoutine;
        EndLoad();
    }

    // Shows loading screen.
    private void StartLoad()
    {
        OnStartLoad?.Invoke();
    }

    // Hides loading screen.
    private void EndLoad()
    {
        OnEndLoad?.Invoke();
    }

    // Unloads all scenes except those that are going to be loaded, then load the scenes requested.
    private IEnumerator LoadScenesOnlyAsync(params string[] scenes)
    {
        IEnumerable<string> currentlyLoadedScenes = GetAllLoadedScenes().Where(scene => scene != _additiveScene).Select(scene => scene.name);
        IEnumerable<string> scenesToUnload = currentlyLoadedScenes.Except(scenes);

        IEnumerable<string> alreadyLoadedScenes = currentlyLoadedScenes.Intersect(scenes);
        if (alreadyLoadedScenes.Any())
        {
            Debug.Log($"Scenes are already loaded: {string.Join(", ", alreadyLoadedScenes)}");
        }

        if (scenesToUnload.Any())
        {
            yield return UnloadScenesAsync(scenesToUnload);
        }

        IEnumerable<string> scenesToLoad = scenes.Except(currentlyLoadedScenes);
        if (scenesToLoad.Any())
        {
            yield return LoadScenesAsync(scenesToLoad);
        }
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
    private IEnumerator UnloadScenesAsync(IEnumerable<string> scenes)
    {
        AsyncOperation[] operations = new AsyncOperation[scenes.Count()];

        for (int i = 0; i < scenes.Count(); i++)
        {
            Debug.Log($"Unload scene {scenes.ElementAt(i)}");
            operations[i] = SceneManager.UnloadSceneAsync(scenes.ElementAt(i));
        }

        foreach (AsyncOperation operation in operations)
        {
            yield return operation;
        }
    }

    // Loads multiple scenes concurrently.
    private IEnumerator LoadScenesAsync(IEnumerable<string> scenes)
    {
        AsyncOperation[] operations = new AsyncOperation[scenes.Count()];

        // Start loading all scenes.
        for (int i = 0; i < scenes.Count(); i++)
        {
            string scene = scenes.ElementAt(i);
            Debug.Log($"Load scene {scene}");
            operations[i] = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            operations[i].allowSceneActivation = false;
        }

        // Wait for all scenes to finish loading.
        foreach (AsyncOperation operation in operations)
        {
            yield return new WaitUntil(() => operation.progress >= 0.9f);
        }

        // Activate all scenes at the same time.
        foreach (AsyncOperation operation in operations)
        {
            operation.allowSceneActivation = true;
        }

        // Set the first scene as the active scene after it is activated.
        yield return new WaitUntil(() => operations[0].isDone);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scenes.First()));
    }

    // Unloads the scenario assets scene then loads it again.
    private IEnumerator ReloadScenarioAssets()
    {
        List<string> scene = new () { _loadedScenarioAssetsSceneName };

        yield return UnloadScenesAsync(scene);
        yield return LoadScenesAsync(scene);
    }
}
