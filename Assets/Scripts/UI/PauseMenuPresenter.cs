using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PauseMenuPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private PauseMenuView _view;
    [SerializeField] private OptionsMenuView _optionsView;

    [Header("Models")]
    [SerializeField] private GameManager _gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CloseMenu();

        if (_gameManager.ScenarioInfo != null)
        {
            DisplayScenarioInfo(_gameManager.ScenarioInfo);
        }

        // Pause menu
        _view.ResumeButton.onClick.AddListener(OnResumePressed);
        _view.RestartButton.onClick.AddListener(OnRestartPressed);
        _view.OptionsButton.onClick.AddListener(OnOptionsPressed);
        _view.QuitScenarioButton.onClick.AddListener(OnQuitPressed);
        _view.BackButton.onClick.AddListener(OnResumePressed);

        // Options menu
        _optionsView.BackButton.onClick.AddListener(OnBackFromOptionsPressed);

        _gameManager.OnGamePause += OpenMenu;
    }

    public void OpenMenu()
    {
        _view.gameObject.SetActive(true);
        _optionsView.gameObject.SetActive(false);

        _view.ActionsTakenText.text = _gameManager.ActionCount.ToString();
    }

    public void CloseMenu()
    {
        _view.gameObject.SetActive(false);
        _optionsView.gameObject.SetActive(false);
    }

    private void DisplayScenarioInfo(ScenarioInfo scenarioInfo)
    {
        _view.LevelNameText.text = scenarioInfo.ScenarioName;
        _optionsView.LevelNameText.text = scenarioInfo.ScenarioName;

        IEnumerable<string> scenarioObjectivesBulletPoints = scenarioInfo.Objectives.MainObjectives.Select(objective => $"- {objective}");
        _view.ScenarioObjectivesText.text = string.Join("\n", scenarioObjectivesBulletPoints);

        IEnumerable<string> optionalObjectivesBulletPoints = scenarioInfo.Objectives.OptionalObjectives.Select(objective => $"- {objective}");
        _view.OptionalObjectivesText.text = string.Join("\n", optionalObjectivesBulletPoints);
    }

    private void OnResumePressed()
    {
        CloseMenu();
        _gameManager.ResumeFromPauseMenu();
    }

    private void OnRestartPressed()
    {
        // TODO: Add a popup here.
        AdditiveSceneManager.Instance.ReloadScenario();
    }

    private void OnOptionsPressed()
    {
        _view.gameObject.SetActive(false);
        _optionsView.gameObject.SetActive(true);
    }

    private void OnBackFromOptionsPressed()
    {
        _view.gameObject.SetActive(true);
        _optionsView.gameObject.SetActive(false);
    }

    private void OnQuitPressed()
    {
        // TODO: Add a popup here.
        AdditiveSceneManager.Instance.LoadMainMenu();
    }
}
