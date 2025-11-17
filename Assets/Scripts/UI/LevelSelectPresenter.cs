using System;
using UnityEngine;

public class LevelSelectPresenter : MonoBehaviour
{
    public Action OnMenuClose;

    [Header("View")]
    [SerializeField] private LevelSelectView _view;

    private AdditiveSceneManager _sceneManager;

    private ScenarioInfo _normalScenarioToBeLoaded;
    private ScenarioInfo _hardScenarioToBeLoaded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sceneManager = AdditiveSceneManager.Instance;

        _view.gameObject.SetActive(false);
        _view.DifficultyPopup.gameObject.SetActive(false);

        _view.LevelSelectButtons[0].Button.Select();

        // Level select
        foreach (CustomLevelSelectButtonView button in _view.LevelSelectButtons)
        {
            button.OnHoverLevel += OnLevelSelectButtonHover;
            button.OnConfirmLevel += OnLevelSelectButtonClick;
        }

        _view.BackButton.Button.onClick.AddListener(CloseMenu);

        // Difficulty popup
        _view.DifficultyBackButton.Button.onClick.AddListener(CloseDifficultySelect);
        _view.NormalDifficultyButton.Button.onClick.AddListener(LoadNormalScenario);
        _view.HardDifficultyButton.Button.onClick.AddListener(LoadHardScenario);
    }

    public void OpenMenu()
    {
        _view.gameObject.SetActive(true);
        _view.DifficultyPopup.gameObject.SetActive(false);
    }

    public void CloseMenu()
    {
        _view.gameObject.SetActive(false);
        OnMenuClose?.Invoke();
    }

    private void LoadNormalScenario()
    {
        if (_normalScenarioToBeLoaded == null)
        {
            Debug.LogError("No scene is set to be loaded.");
            return;
        }
        ScenarioInfo sceneToLoad = _normalScenarioToBeLoaded;
        _sceneManager.LoadScenario(sceneToLoad.EnvironmentSceneName, sceneToLoad.ScenarioAssetsSceneName);
    }

    private void LoadHardScenario()
    {
        if (_hardScenarioToBeLoaded == null)
        {
            Debug.LogError("No scene is set to be loaded.");
            return;
        }
        ScenarioInfo sceneToLoad = _hardScenarioToBeLoaded;
        _sceneManager.LoadScenario(sceneToLoad.EnvironmentSceneName, sceneToLoad.ScenarioAssetsSceneName);
    }

    private void OnLevelSelectButtonHover(ScenarioInfo normalScenarioInfo, ScenarioInfo hardScenarioInfo)
    {
        _view.LevelNameText.text = normalScenarioInfo.ScenarioName;
        _view.LevelDescriptionText.text = normalScenarioInfo.Description;
        _view.BackgroundImage.sprite = normalScenarioInfo.Thumbnail;
    }

    private void OnLevelSelectButtonClick(ScenarioInfo normalScenarioInfo, ScenarioInfo hardScenarioInfo)
    {
        _normalScenarioToBeLoaded = normalScenarioInfo;
        _hardScenarioToBeLoaded = hardScenarioInfo;
        OpenDifficultySelect();
    }

    private void OpenDifficultySelect()
    {
        _view.DifficultyPopup.gameObject.SetActive(true);
    }

    private void CloseDifficultySelect()
    {
        _view.DifficultyPopup.gameObject.SetActive(false);
    }

    private void LoadBestRecord(ScenarioInfo scenarioInfo)
    {
        // TODO: Load best record and optional objectives here.
    }
}
