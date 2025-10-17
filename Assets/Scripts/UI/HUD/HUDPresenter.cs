using System;
using System.Collections;
using UnityEngine;

public class HUDPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private HUDView View;

    // Add models here
    [Header("Models")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private FramerateChecker _framerateChecker;
    [SerializeField] private PlayerInteract _playerInteractModel;

    [Header("Settings")]
    [SerializeField] private Color _interactionNameDefaultColor;
    [SerializeField] private Color _interactionNameInteractingColor;
    [SerializeField] private float _redoUnavailableAlpha;
    [SerializeField] private float _redoAvailableAlpha;

    private const float OBJECTIVE_FADE_DELAY = 10f;
    private const float OBJECTIVE_FADE_DURATION = 2f;

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

        View.ObjectivesElements.alpha = 0;

        if (_framerateChecker != null )
        {
            _framerateChecker.OnFramerateUpdate += OnFramerateUpdate;
        }

        _gameManager.OnLevelStart += OnLevelStart;
        _gameManager.OnActionUpdate += OnActionCounterUpdate;
        _gameManager.OnRedoAvailable += OnRedoAvailable;
        _gameManager.OnRedoUnavailable += OnRedoUnavailable;

        _playerInteractModel.OnLookAtInteractable += OnPlayerLookAtInteractable;
        _playerInteractModel.OnLookAwayFromInteractable += OnPlayerLookAwayFromInteractable;
        _playerInteractModel.OnInteract += OnPlayerInteract;
        _playerInteractModel.OnEndInteraction += OnPlayerEndInteraction;

        View.InteractionPrompts.SetActive(false);
    }

    private void OnPlayerLookAtInteractable(InteractableObjectInfo interactable)
    {
        if (_isInteracting)
        {
            return;
        }

        View.InteractionPrompts.SetActive(true);
        View.InteractableNameText.color = _interactionNameDefaultColor;
        View.InteractableNameText.text = interactable.ObjectName;
        View.InteractableActionText.text = interactable.ActionName;

        SetCrosshair((int)interactable.Type);
    }

    private void OnPlayerLookAwayFromInteractable()
    {
        if (!_isInteracting)
        {
            View.InteractionPrompts.SetActive(false);
            SetCrosshair(crosshairIndex: 0);
        }
    }

    private void OnPlayerInteract(InteractableObjectInfo interactable)
    {
        HideObjectives();
        _isInteracting = true;

        View.InteractionPrompts.SetActive(true);
        View.InteractableNameText.text = interactable.ObjectName;
        View.InteractableNameText.color = _interactionNameInteractingColor;
        View.InteractableActionText.text = interactable.ActionName;

        View.PromptsLooking.SetActive(false);
        View.PromptsInteracting.SetActive(true);
    }

    private void OnPlayerEndInteraction(InteractableObjectInfo interactable)
    {
        _isInteracting = false;
        OnPlayerLookAtInteractable(interactable);

        View.PromptsLooking.SetActive(true);
        View.PromptsInteracting.SetActive(false);
    }

    private void OnLevelStart()
    {
        View.ObjectivesElements.alpha = 1;
        _objectivesFadeCoroutine = StartCoroutine(DelayAndFadeObjectives());
    }

    private void OnActionCounterUpdate(int actionsTaken)
    {
        View.ActionsTakenText.text = actionsTaken.ToString();
    }

    private void OnRedoAvailable()
    {
        View.RedoPrompt.alpha = _redoAvailableAlpha;
    }

    private void OnRedoUnavailable()
    {
        View.RedoPrompt.alpha = _redoUnavailableAlpha;
    }

    private void OnFramerateUpdate(float framerate, float latency)
    {
        _lastFramerate = framerate;
        _lastLatency = latency;

        float framerateFloored = Mathf.Floor(_lastFramerate);
        float latencyMilliseconds = _lastLatency * 1000;
        float latencyMsRounded = (int)(latencyMilliseconds * 10) / 10f;

        View.TelemetryText.text = string.Format(TELEMETRY_FORMAT, _platform, _buildVersion, Mathf.Floor(_lastFramerate), latencyMsRounded);
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
        View.Crosshairs[_currentCrosshair].SetActive(false);
        View.Crosshairs[crosshairIndex].SetActive(true);
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
            View.ObjectivesElements.alpha = 1.0f - (time / OBJECTIVE_FADE_DURATION);
            yield return null;
        }

        View.ObjectivesElements.alpha = 0;
    }
}
