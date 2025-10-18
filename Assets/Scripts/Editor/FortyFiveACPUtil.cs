using UnityEditor;

public class FortyFiveACPUtil : Editor
{
    // For fortyfiveacp's testing AdditiveSceneManager. This file will be deleted after the UI is implemented.

    [MenuItem("fortyfiveacp/Change Scene/Load Main Menu")]
    public static void LoadMainMenu()
    {
        AdditiveSceneManager additiveSceneManager = FindFirstObjectByType<AdditiveSceneManager>();
        additiveSceneManager.LoadMainMenu();
    }

    [MenuItem("fortyfiveacp/Change Scene/Load Gameplay")]
    public static void LoadGameplay()
    {
        AdditiveSceneManager additiveSceneManager = FindFirstObjectByType<AdditiveSceneManager>();
        additiveSceneManager.LoadScenario("2-office", "2-office-s1");
    }

    [MenuItem("fortyfiveacp/Change Scene/Restart Scenario")]
    public static void RestartGameplay()
    {
        AdditiveSceneManager additiveSceneManager = FindFirstObjectByType<AdditiveSceneManager>();
        additiveSceneManager.ReloadScenario();
    }
}
