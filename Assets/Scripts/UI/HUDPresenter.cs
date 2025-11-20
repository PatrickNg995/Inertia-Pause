using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HUDPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private HUDView _view;

    // Add models here
    [Header("Models")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private FramerateChecker _framerateChecker;
    [SerializeField] private PlayerInteract _playerInteractModel;

    [Header("Settings")]
    [SerializeField] private Color _interactionNameDefaultColor;
    [SerializeField] private Color _interactionNameInteractingColor;
    [SerializeField] private float _promptUnavailableAlpha;
    [SerializeField] private float _promptAvailableAlpha;

    private const float OBJECTIVE_FADE_DELAY = 5f;
    private const float OBJECTIVE_FADE_DURATION = 1f;

    private const string TELEMETRY_FORMAT = "{0} V{1} - {2} FPS - {3} ms";

    private Coroutine _objectivesFadeCoroutine;
    private WaitForSeconds _objectiveFadeDelay = new (OBJECTIVE_FADE_DELAY);

    private bool _isInteracting;
    private bool _isObjectivesHidden;
    private int _currentCrosshair;

    // Telemetry
    private string _platform;
    private string _buildVersion;
    private float _lastFramerate;
    private float _lastLatency;

    void Start()
    {
        _platform = Application.platform.ToString();
        _buildVersion = Application.version;
        OnFramerateUpdate(0, 0);

        _view.ObjectivesElements.alpha = 0;

        if (_gameManager.ScenarioInfo != null)
        {
            DisplayScenarioInfo(_gameManager.ScenarioInfo);
        }

        if (_framerateChecker != null)
        {
            _framerateChecker.OnFramerateUpdate += OnFramerateUpdate;
        }

        _gameManager.OnLevelStart += OnLevelStart;
        _gameManager.OnActionUpdate += OnActionCounterUpdate;
        _gameManager.OnUndoAvailable += OnUndoAvailable;
        _gameManager.OnUndoUnavailable += OnUndoUnavailable;
        _gameManager.OnRedoAvailable += OnRedoAvailable;
        _gameManager.OnRedoUnavailable += OnRedoUnavailable;
        _gameManager.OnAnyBlockingMenuOpen += CloseMenu;
        _gameManager.OnAnyBlockingMenuClose += OpenMenu;
        _gameManager.OnLevelComplete += _ => CloseMenu();

        _playerInteractModel.OnLookAtInteractable += OnPlayerLookAtInteractable;
        _playerInteractModel.OnLookAwayFromInteractable += OnPlayerLookAwayFromInteractable;
        _playerInteractModel.OnContinuousInteract += OnPlayerInteract;
        _playerInteractModel.OnOneShotInteract += _ => HideObjectives();
        _playerInteractModel.OnEndInteraction += OnPlayerEndInteraction;

        _view.InteractionPrompts.SetActive(false);
        OnUndoUnavailable();
        OnRedoUnavailable();
    }

    public void ShowTelemetry(bool show)
    {
        _view.TelemetryText.enabled = show;
    }

    private void OpenMenu()
    {
        _view.MainCanvas.enabled = true;
    }

    private void CloseMenu()
    {
        _view.MainCanvas.enabled = false;
        _view.ObjectivesElements.alpha = 0;
    }

    private void DisplayScenarioInfo(ScenarioInfo scenarioInfo)
    {
        _view.LevelNameText.text = $"\"{scenarioInfo.ScenarioName}\"";

        IEnumerable<string> scenarioObjectivesBulletPoints = scenarioInfo.Objectives.MainObjectives.Select(objective => $"- {objective}");
        _view.ScenarioObjectivesText.text = string.Join("\n", scenarioObjectivesBulletPoints);

        IEnumerable<string> optionalObjectivesBulletPoints = scenarioInfo.Objectives.OptionalObjectives.Select(objective => $"- {objective.Description}");
        _view.OptionalObjectivesText.text = string.Join("\n", optionalObjectivesBulletPoints);
    }

    private void OnPlayerLookAtInteractable(InteractableObjectInfo interactable)
    {
        if (_isInteracting)
        {
            return;
        }

        _view.InteractionPrompts.SetActive(true);
        _view.InteractableNameText.color = _interactionNameDefaultColor;
        _view.InteractableNameText.text = interactable.ObjectName;
        _view.InteractableActionText.text = interactable.ActionName;

        SetCrosshair((int)interactable.Type);
    }

    private void OnPlayerLookAwayFromInteractable()
    {
        if (!_isInteracting)
        {
            _view.InteractionPrompts.SetActive(false);
            SetCrosshair(crosshairIndex: 0);
        }
    }

    private void OnPlayerInteract(InteractableObjectInfo interactable)
    {
        HideObjectives();
        _isInteracting = true;

        _view.InteractionPrompts.SetActive(true);
        _view.InteractableNameText.text = interactable.ObjectName;
        _view.InteractableNameText.color = _interactionNameInteractingColor;
        _view.InteractableActionText.text = interactable.ActionName;

        _view.PromptsLooking.SetActive(false);
        _view.PromptsInteracting.SetActive(true);

        _view.ButtonPrompts.SetActive(false);
    }

    private void OnPlayerEndInteraction(InteractableObjectInfo interactable)
    {
        _isInteracting = false;
        OnPlayerLookAtInteractable(interactable);

        _view.PromptsLooking.SetActive(true);
        _view.PromptsInteracting.SetActive(false);

        _view.ButtonPrompts.SetActive(true);
    }

    private void OnLevelStart()
    {
        _view.ObjectivesElements.alpha = 1;
        _objectivesFadeCoroutine = StartCoroutine(DelayAndFadeObjectives());
    }

    private void OnActionCounterUpdate(int actionsTaken)
    {
        _view.ActionsTakenText.text = actionsTaken.ToString();
    }

    private void OnRedoAvailable()
    {
        _view.RedoPrompt.alpha = _promptAvailableAlpha;
    }

    private void OnRedoUnavailable()
    {
        _view.RedoPrompt.alpha = _promptUnavailableAlpha;
    }

    private void OnUndoAvailable()
    {
        _view.UndoPrompt.alpha = _promptAvailableAlpha;
    }

    private void OnUndoUnavailable()
    {
        _view.UndoPrompt.alpha = _promptUnavailableAlpha;
    }

    private void OnFramerateUpdate(float framerate, float latency)
    {
        _lastFramerate = framerate;
        _lastLatency = latency;

        float framerateFloored = Mathf.Floor(_lastFramerate);
        float latencyMilliseconds = _lastLatency * 1000;
        float latencyMsRounded = (int)(latencyMilliseconds * 10) / 10f;

        _view.TelemetryText.text = string.Format(TELEMETRY_FORMAT, _platform, _buildVersion, Mathf.Floor(_lastFramerate), latencyMsRounded);
    }


    private void HideObjectives()
    {
        if (_isObjectivesHidden)
        {
            return;
        }

        if (_objectivesFadeCoroutine != null)
        {
            StopCoroutine(_objectivesFadeCoroutine);
        }

        StartCoroutine(FadeObjectives());
    }

    private void SetCrosshair(int crosshairIndex)
    {
        _view.Crosshairs[_currentCrosshair].SetActive(false);
        _view.Crosshairs[crosshairIndex].SetActive(true);
        _currentCrosshair = crosshairIndex;
    }

    private IEnumerator DelayAndFadeObjectives()
    {
        yield return _objectiveFadeDelay;
        yield return FadeObjectives();
    }

    private IEnumerator FadeObjectives()
    {
        _isObjectivesHidden = true;
        float time = 0;

        while (time < OBJECTIVE_FADE_DURATION)
        {
            time += Time.deltaTime;
            _view.ObjectivesElements.alpha = 1.0f - (time / OBJECTIVE_FADE_DURATION);
            yield return null;
        }

        _view.ObjectivesElements.alpha = 0;
    }
}
