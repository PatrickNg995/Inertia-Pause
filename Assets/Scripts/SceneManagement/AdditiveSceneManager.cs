using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneManager : MonoBehaviour
{
    [SerializeField] private Scene _additiveScene;
    [SerializeField] private Scene _menuUIScene;

    private Scene _loadedAdditiveScene;
    private Scene _loadedMenuUIScene;
    private Scene _loadedEnvironmentScene;
    private Scene _loadedScenarioAssetsScene;

    /// <summary>
    /// Unloads all scenes, then loads the main menu, which consists of the UI scene, environment scene and assets scene.
    /// </summary>
    /// <param name="environmentScene">A reference to the environment scene to be loaded, which consists of the level environment, terrain, and lighting.</param>
    /// <param name="scenarioAssetsScene">A reference to the scenario assets scene to be loaded, which consists of puzzle objects and dynamic camera angles.</param>
    public async void LoadMainMenu(Scene environmentScene, Scene scenarioAssetsScene)
    {
        _loadedEnvironmentScene = environmentScene;
        _loadedScenarioAssetsScene = scenarioAssetsScene;
    }

    /// <summary>
    /// Unloads all scenes, then loads a scenario, which consists of an environment scene and assets scene. If the same environment scene is already loaded,
    /// then that scene is not unloaded.
    /// </summary>
    /// <param name="environmentScene">A reference to the environment scene to be loaded, which consists of the level environment, terrain, and lighting.</param>
    /// <param name="scenarioAssetsScene">A reference to the scenario assets scene to be loaded, which consists of puzzle objects and scripting.</param>
    public async void LoadScenario(Scene environmentScene, Scene scenarioAssetsScene)
    {
        _loadedEnvironmentScene = environmentScene;
        _loadedScenarioAssetsScene = scenarioAssetsScene;
    }

    /// <summary>
    /// Restarts a scenario, which reloads the assets scene but keeps the environment loaded.
    /// </summary>
    public async void ReloadScenario()
    {

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

    private async void UpdateScenesAsync(Scene[] unloadList, Scene[] loadList, bool forceReload = false)
    {

    }

    private async void UnloadScenesAsync(Scene[] scenes)
    {

    }

    // Loads multiple scenes concurrently.
    private async void LoadScenesAsync(Scene[] scenes)
    {

    }
}
