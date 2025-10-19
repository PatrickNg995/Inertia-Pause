using UnityEditor;
using UnityEngine;

public class MainMenuPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private MainMenuView _view;

    private const string BUILD_NUMBER_FORMAT = "{0} V{1} (Alpha)";

    private const string OFFICE_SCENE_NAME = "2-office";
    private const string SCENARIO_A_SCENE_NAME = "2-office-s1";
    private const string SCENARIO_B_SCENE_NAME = "2-office-s2";
    private const string SCENARIO_C_SCENE_NAME = "2-office-s3";

    private AdditiveSceneManager _sceneManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sceneManager = AdditiveSceneManager.Instance;

        _view.BuildText.text = string.Format(BUILD_NUMBER_FORMAT, Application.platform, Application.version);

        _view.ScenarioAButton.onClick.AddListener(() => _sceneManager.LoadScenario(OFFICE_SCENE_NAME, SCENARIO_A_SCENE_NAME));
        _view.ScenarioBButton.onClick.AddListener(() => _sceneManager.LoadScenario(OFFICE_SCENE_NAME, SCENARIO_B_SCENE_NAME));
        _view.ScenarioCButton.onClick.AddListener(() => _sceneManager.LoadScenario(OFFICE_SCENE_NAME, SCENARIO_C_SCENE_NAME));

        _view.ExitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnExitClicked()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit(0);
#endif
    }
}
