using UnityEngine;

public class ResultMenuPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private ResultMenuView _view;

    [Header("Models")]
    [SerializeField] private GameManager _gameManager;

    [Header("Settings")]
    [SerializeField] private Color _completeColor;
    [SerializeField] private Color _failedColorMain;
    [SerializeField] private Color _failedColorOptional;

    private const string LEVEL_COMPLETE_TEXT = "Complete";
    private const string LEVEL_FAILED_TEXT = "Failed";

    private const string CIVILIANS_OBJECTIVE_TEXT = "Civilians Rescued";
    private const string ALLIES_OBJECTIVE_TEXT = "Allies Saved";
    private const string ENEMIES_OBJECTIVE_TEXT = "Enemies Killed";
    private const string COMPLETE_TEXT = "Complete";
    private const string MISSED_TEXT = "Missed";
    private const string OBJECTIVE_COUNT_FORMAT = "{0}/{1}";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScenarioInfo scenarioInfo = _gameManager.ScenarioInfo;

        _view.RestartButton.onClick.AddListener(() => AdditiveSceneManager.Instance.ReloadScenario());
        _view.NextButton.onClick.AddListener(() => OnNextButtonClicked(scenarioInfo));
        _view.MainMenuButton.onClick.AddListener(() => AdditiveSceneManager.Instance.LoadMainMenu());

        _gameManager.OnLevelComplete += OnLevelComplete;

        CloseMenu();
    }

    private void OpenMenu()
    {
        gameObject.SetActive(true);
    }

    private void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    private void OnLevelComplete(LevelResults results)
    {
        OpenMenu();
        DisplayLevelStats(results);
    }

    private void DisplayLevelStats(LevelResults results)
    {
        ScenarioInfo scenarioInfo = _gameManager.ScenarioInfo;
        bool isLevelComplete = _gameManager.LevelWon;

        _view.LevelNameText.text = _gameManager.ScenarioInfo.ScenarioName;
        _view.CompletionText.text = isLevelComplete ? LEVEL_COMPLETE_TEXT : LEVEL_FAILED_TEXT;
        _view.ActionCountText.text = results.ActionsTaken.ToString();

        _view.NewRecord.SetActive(false);
        _view.NextButton.gameObject.SetActive(_gameManager.LevelWon);

        if (scenarioInfo.NumberOfCivilians > 0)
        {
            bool isComplete = results.CiviliansRescued >= scenarioInfo.NumberOfCivilians;
            Color textColor = isComplete ? _completeColor : _failedColorMain;
            AddObjectiveRow(CIVILIANS_OBJECTIVE_TEXT, string.Format(OBJECTIVE_COUNT_FORMAT, results.CiviliansRescued, scenarioInfo.NumberOfCivilians), textColor);
        }
        if (scenarioInfo.NumberOfAllies > 0)
        {
            bool isComplete = results.AlliesSaved >= scenarioInfo.NumberOfAllies;
            Color textColor = isComplete ? _completeColor : _failedColorMain;
            AddObjectiveRow(ALLIES_OBJECTIVE_TEXT, string.Format(OBJECTIVE_COUNT_FORMAT, results.AlliesSaved, scenarioInfo.NumberOfAllies), textColor);
        }
        if (scenarioInfo.NumberOfEnemies > 0)
        {
            bool isComplete = results.EnemiesKilled >= scenarioInfo.NumberOfEnemies;
            Color textColor = isComplete ? _completeColor : _failedColorMain;
            AddObjectiveRow(ENEMIES_OBJECTIVE_TEXT, string.Format(OBJECTIVE_COUNT_FORMAT, results.EnemiesKilled, scenarioInfo.NumberOfEnemies), textColor);
        }

        for (int i = 0; i < scenarioInfo.Objectives.OptionalObjectives.Count; i++)
        {
            string objective = scenarioInfo.Objectives.OptionalObjectives[i];
            bool isComplete = results.OptionalObjectivesComplete[i];
            string completionText = results.OptionalObjectivesComplete[i] ? COMPLETE_TEXT : MISSED_TEXT;
            Color textColor = isComplete ? _completeColor : _failedColorOptional;
            AddObjectiveRow(objective, completionText, textColor);
        }
    }

    private void AddObjectiveRow(string objectiveName, string objectiveStatus, Color textColor)
    {
        ObjectiveRowView rowView = Instantiate(_view.ObjectiveRowPrefab, _view.ObjectiveRowContainer.transform);
        rowView.ObjectiveNameText.text = objectiveName;
        rowView.ObjectiveStatusText.text = objectiveStatus;
        rowView.ObjectiveNameText.color = textColor;
        rowView.ObjectiveStatusText.color = textColor;
        rowView.gameObject.SetActive(true);
    }

    private void OnNextButtonClicked(ScenarioInfo scenarioInfo)
    {
        if (scenarioInfo.NextEnvironmentSceneName != string.Empty && scenarioInfo.NextScenarioAssetsSceneName != string.Empty)
        {
            AdditiveSceneManager.Instance.LoadScenario(scenarioInfo.NextEnvironmentSceneName, scenarioInfo.NextScenarioAssetsSceneName);
        }
        else
        {
            AdditiveSceneManager.Instance.LoadMainMenu();
        }
    }
}
