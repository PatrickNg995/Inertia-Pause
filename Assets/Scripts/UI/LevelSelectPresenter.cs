using System;
using UnityEngine;

public class LevelSelectPresenter : MonoBehaviour
{
    public Action OnMenuClose;

    [Header("View")]
    [SerializeField] private LevelSelectView _view;

    [Header("Settings")]
    [SerializeField] private Color _completeOptionalObjectiveColor;
    [SerializeField] private Color _incompleteOptionalObjectiveColor;

    private const string BEST_RECORD_SINGULAR_FORMAT = "Best Record: {0} Action";
    private const string BEST_RECORD_PLURAL_FORMAT = "Best Record: {0} Actions";

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

        if (sceneToLoad.EnvironmentSceneName == string.Empty || sceneToLoad.ScenarioAssetsSceneName == string.Empty)
        {
            Debug.LogError("Scene to be loaded is empty string.");
            return;
        }

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

        if (sceneToLoad.EnvironmentSceneName == string.Empty || sceneToLoad.ScenarioAssetsSceneName == string.Empty)
        {
            Debug.LogError("Scene to be loaded is empty string.");
            return;
        }

        _sceneManager.LoadScenario(sceneToLoad.EnvironmentSceneName, sceneToLoad.ScenarioAssetsSceneName);
    }

    private void OnLevelSelectButtonHover(ScenarioInfo normalScenarioInfo, ScenarioInfo hardScenarioInfo)
    {
        _view.LevelNameText.text = normalScenarioInfo.ScenarioName;
        _view.LevelDescriptionText.text = normalScenarioInfo.Description;
        _view.BackgroundImage.sprite = normalScenarioInfo.Thumbnail;

        LoadBestRecord(normalScenarioInfo, hardScenarioInfo);
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

    private void LoadBestRecord(ScenarioInfo normalScenarioInfo, ScenarioInfo hardScenarioInfo)
    {
        // TODO: Load best record and optional objectives here.

        RemoveObjectiveRows();

        if (normalScenarioInfo != null)
        {
            int normalBestRecord = 1;
            string normalBestRecordFormat = normalBestRecord == 1 ? BEST_RECORD_SINGULAR_FORMAT : BEST_RECORD_PLURAL_FORMAT;
            _view.NormalBestRecordText.text = string.Format(normalBestRecordFormat, normalBestRecord);

            foreach (string optionalObjective in normalScenarioInfo.Objectives.OptionalObjectives)
            {
                bool isNormalComplete = false;
                Color normalTextColor = isNormalComplete ? _completeOptionalObjectiveColor : _incompleteOptionalObjectiveColor;
                AddObjectiveRow(optionalObjective, isNormalComplete, isNormalDifficulty: true, normalTextColor);
            }
        }
        else
        {
            _view.NormalBestRecordText.text = string.Empty;
        }

        if (hardScenarioInfo != null)
        {
            int hardBestRecord = 3;
            string hardBestRecordFormat = hardBestRecord == 1 ? BEST_RECORD_SINGULAR_FORMAT : BEST_RECORD_PLURAL_FORMAT;
            _view.HardBestRecordText.text = string.Format(hardBestRecordFormat, hardBestRecord);

            foreach (string optionalObjective in hardScenarioInfo.Objectives.OptionalObjectives)
            {
                bool isHardComplete = true;
                Color hardtextColor = isHardComplete ? _completeOptionalObjectiveColor : _incompleteOptionalObjectiveColor;
                AddObjectiveRow(optionalObjective, isHardComplete, isNormalDifficulty: true, hardtextColor);
            }
        }
        else
        {
            _view.HardBestRecordText.text = string.Empty;
        }
    }

    private void AddObjectiveRow(string objectiveName, bool isComplete, bool isNormalDifficulty, Color textColor)
    {
        Transform container = isNormalDifficulty ? _view.NormalOptionalObjectivesContainer.transform : _view.HardOptionalObjectivesContainer.transform;

        LevelSelectObjectiveRowView rowView = Instantiate(_view.OptionalObjectivesPrefab, container);
        rowView.ObjectiveNameText.text = objectiveName;
        rowView.ObjectiveNameText.color = textColor;

        rowView.Check.gameObject.SetActive(isComplete);
        rowView.Cross.gameObject.SetActive(!isComplete);

        rowView.gameObject.SetActive(true);
    }

    private void RemoveObjectiveRows()
    {
        Transform normalContainer = _view.NormalOptionalObjectivesContainer.transform;
        Transform hardContainer = _view.HardOptionalObjectivesContainer.transform;

        // Index 0 is the template row so destroy children after index 0.
        for (int i = normalContainer.childCount - 1; i > 0; i--)
        {
            Destroy(normalContainer.GetChild(i).gameObject);
        }

        // Empty all children from hard container.
        for (int i = hardContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(hardContainer.GetChild(i).gameObject);
        }
    }
}
