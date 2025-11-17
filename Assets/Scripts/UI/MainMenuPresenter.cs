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

    private const string BUILD_NUMBER_FORMAT = "{0} V{1}";
    private const string FIRST_LEVEL_ENVIRONMENT = "2-office";
    private const string FIRST_LEVEL_SCENARIO_ASSETS = "2-office-s1";

    private AdditiveSceneManager _sceneManager;
    private PlayerActions _inputActions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sceneManager = AdditiveSceneManager.Instance;

        _view.gameObject.SetActive(true);
        _view.MainMenuScreen.SetActive(false);
        _view.StartScreen.SetActive(true);

        _view.BuildText.text = string.Format(BUILD_NUMBER_FORMAT, Application.platform, Application.version);
        _view.DescriptionText.text = string.Empty;

        // TODO: Hide Continue button for first time players.
        _view.ContinueButton.gameObject.SetActive(false);

        _optionsMenuPresenter.OnMenuClose += () => OpenMenu();
        _levelSelectPresenter.OnMenuClose += () => OpenMenu();

        _view.ContinueButton.Button.onClick.AddListener(OnContinueClicked);
        _view.NewGameButton.Button.onClick.AddListener(OnNewGameClicked);
        _view.ScenarioSelectButton.Button.onClick.AddListener(OnScenarioSelectClicked);
        _view.OptionsButton.Button.onClick.AddListener(OnOptionsClicked);
        _view.ExitButton.Button.onClick.AddListener(OnExitClicked);

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
