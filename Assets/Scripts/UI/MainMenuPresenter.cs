using UnityEditor;
using UnityEngine;

public class MainMenuPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private MainMenuView _view;

    [Header("Other Presenters")]
    [SerializeField] private LevelSelectPresenter _levelSelectPresenter;
    [SerializeField] private OptionsMenuPresenter _optionsMenuPresenter;

    private const string BUILD_NUMBER_FORMAT = "{0} V{1} (Alpha)";
    private const string FIRST_LEVEL_ENVIRONMENT = "2-office";
    private const string FIRST_LEVEL_SCENARIO_ASSETS = "2-office-s1";

    private AdditiveSceneManager _sceneManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _view.BuildText.text = string.Format(BUILD_NUMBER_FORMAT, Application.platform, Application.version);

        // TODO: Hide Continue button if no last level.
        // _view.ContinueButton.enabled = false;

        _optionsMenuPresenter.OnMenuClose += () => OpenMenu();
        _levelSelectPresenter.OnMenuClose += () => OpenMenu();

        _view.ContinueButton.Button.onClick.AddListener(OnContinueClicked);
        _view.NewGameButton.Button.onClick.AddListener(OnNewGameClicked);
        _view.ScenarioSelectButton.Button.onClick.AddListener(OnScenarioSelectClicked);
        _view.OptionsButton.Button.onClick.AddListener(OnOptionsClicked);
        _view.ExitButton.Button.onClick.AddListener(OnExitClicked);
    }

    public void OpenMenu()
    {
        _view.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        _view.gameObject.SetActive(false);
    }

    private void OnContinueClicked()
    {
        // TODO: Load last level.
        Debug.LogWarning("Continue button not implemented yet, use Scenario Select.");
    }

    private void OnNewGameClicked()
    {
        // TODO: Add confirmation box here. "Are you sure you want to start the game from the beginning?"
        _sceneManager.LoadScenario(FIRST_LEVEL_ENVIRONMENT, FIRST_LEVEL_SCENARIO_ASSETS);
    }

    private void OnScenarioSelectClicked()
    {
        _view.gameObject.SetActive(false);
        _levelSelectPresenter.OpenMenu();
    }

    private void OnOptionsClicked()
    {
        _view.gameObject.SetActive(false);
        _optionsMenuPresenter.OpenMenu();
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
