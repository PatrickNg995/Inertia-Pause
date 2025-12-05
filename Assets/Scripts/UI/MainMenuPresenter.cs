using UnityEditor;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class MainMenuPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private MainMenuView _view;

    [Header("Models")]
    [SerializeField] private LevelSelectPresenter _levelSelectPresenter;
    [SerializeField] private OptionsMenuPresenter _optionsMenuPresenter;
    [SerializeField] private SavedLevelProgressManager _progressManager;

    private const string BUILD_NUMBER_FORMAT = "{0} V{1}";
    private const string FIRST_LEVEL_ENVIRONMENT = "1-promenade";
    private const string NORMAL_FIRST_LEVEL_SCENARIO_ASSETS = "1-promenade-easy";
    private const string HARD_FIRST_LEVEL_SCENARIO_ASSETS = "1-promenade-hard";

    private AdditiveSceneManager _sceneManager;
    private PlayerActions _inputActions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sceneManager = AdditiveSceneManager.Instance;

        _view.gameObject.SetActive(true);
        _view.MainMenuScreen.SetActive(false);
        _view.DifficultyPopup.SetActive(false);
        _view.StartScreen.SetActive(true);

        _view.BuildText.text = string.Format(BUILD_NUMBER_FORMAT, Application.platform, Application.version);
        _view.DescriptionText.text = string.Empty;

        // Hide Continue button for first time players.
        bool isContinueAvailable = _progressManager.LevelProgressData.CurrentLevelAssetsName != null && _progressManager.LevelProgressData.CurrentLevelAssetsName != string.Empty;
        _view.ContinueButton.gameObject.SetActive(isContinueAvailable);

        _optionsMenuPresenter.OnMenuClose += () => OpenMenu();
        _levelSelectPresenter.OnMenuClose += () => OpenMenu();

        _view.ContinueButton.Button.onClick.AddListener(OnContinueClicked);
        _view.NewGameButton.Button.onClick.AddListener(OnNewGameClicked);
        _view.ScenarioSelectButton.Button.onClick.AddListener(OnScenarioSelectClicked);
        _view.OptionsButton.Button.onClick.AddListener(OnOptionsClicked);
        _view.ExitButton.Button.onClick.AddListener(OnExitClicked);

        _view.NormalDifficultyButton.Button.onClick.AddListener(OnNewGamePopupNormalClicked);
        _view.HardDifficultyButton.Button.onClick.AddListener(OnNewGamePopupHardClicked);
        _view.DifficultyBackButton.Button.onClick.AddListener(OnNewGamePopupBackClicked);

        _view.ContinueButton.OnHover += ChangeHint;
        _view.NewGameButton.OnHover += ChangeHint;
        _view.ScenarioSelectButton.OnHover += ChangeHint;
        _view.OptionsButton.OnHover += ChangeHint;
        _view.ExitButton.OnHover += ChangeHint;

        _inputActions = new PlayerActions();
        _inputActions.UI.Click.performed += GoToMainMenu;
        _inputActions.UI.Submit.performed += GoToMainMenu;
        _inputActions.Enable();
    }

    public void OpenMenu()
    {
        _view.gameObject.SetActive(true);
        _view.DescriptionText.text = string.Empty;
        _inputActions.Enable();
    }

    public void CloseMenu()
    {
        _view.gameObject.SetActive(false);
        _inputActions.Disable();
    }

    private void ChangeHint(string description)
    {
        _view.DescriptionText.text = description;
    }

    private void GoToMainMenu(CallbackContext _)
    {
        _view.MainMenuScreen.SetActive(true);
        _view.StartScreen.SetActive(false);

        _inputActions.UI.Click.performed -= GoToMainMenu;
        _inputActions.UI.Submit.performed -= GoToMainMenu;
    }

    private void OnContinueClicked()
    {
        _sceneManager.LoadScenario(_progressManager.LevelProgressData.CurrentLevelEnvironmentName, _progressManager.LevelProgressData.CurrentLevelAssetsName);
    }

    private void OnNewGameClicked()
    {
        _view.DifficultyPopup.SetActive(true);
        _view.BottomBar.SetActive(false);
    }

    private void OnNewGamePopupNormalClicked()
    {
        _sceneManager.LoadScenario(FIRST_LEVEL_ENVIRONMENT, NORMAL_FIRST_LEVEL_SCENARIO_ASSETS);
    }

    private void OnNewGamePopupHardClicked()
    {
        _sceneManager.LoadScenario(FIRST_LEVEL_ENVIRONMENT, HARD_FIRST_LEVEL_SCENARIO_ASSETS);
    }

    private void OnNewGamePopupBackClicked()
    {
        _view.DifficultyPopup.SetActive(false);
        _view.BottomBar.SetActive(true);
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
