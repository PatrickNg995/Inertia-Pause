using System.Collections;
using UnityEngine;

public class HUDPresenter : MonoBehaviour
{
    [SerializeField] private HUDView _view;

    // Add models here

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
    private int _actionsTaken;

    // Telemetry
    private string _platform;
    private string _buildVersion;
    private float _lastFramerate;
    private float _lastLatency;

    void Start()
    {
        // TODO: Add listeners here when we have game manager working

        _platform = Application.platform.ToString();
        _buildVersion = Application.version;
        _lastFramerate = 0;
        _lastLatency = 0;

        // _levelStartController.OnLevelStart += HideObjectives;

        // _playerController.OnLookAtInteractable += OnPlayerLookAtInteractable;
        // ...
        // _playerController.OnInteract += OnPlayerInteract;
    }

    private void OnPlayerLookAtInteractable(InteractableObjectInfo interactable)
    {
        if (!_isInteracting)
        {
            _view.InteractionPrompts.SetActive(true);
            _view.InteractableNameText.color = _interactionNameDefaultColor;
            _view.InteractableNameText.text = interactable.ObjectName;
            _view.InteractableActionText.text = interactable.ActionName;
        }
    }

    private void OnPlayerLookAwayFromInteractable()
    {
        if (!_isInteracting)
        {
            _view.InteractionPrompts.SetActive(false);
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
    }

    private void OnPlayerEndInteraction(InteractableObjectInfo interactable)
    {
        _isInteracting = false;
        OnPlayerLookAtInteractable(interactable);
    }

    private void OnLevelStart()
    {
        _objectivesFadeCoroutine = StartCoroutine(DelayAndFadeObjectives());
    }

    private void OnActionTaken()
    {
        _actionsTaken++;
        _view.ActionsTakenText.text = _actionsTaken.ToString();
    }

    private void OnActionUndo()
    {
        _actionsTaken--;
        _view.ActionsTakenText.text = _actionsTaken.ToString();
    }

    private void OnRedoAvailable()
    {
        _view.RedoPrompt.alpha = _redoAvailableAlpha;
    }

    private void OnRedoUnavailable()
    {
        _view.RedoPrompt.alpha = _redoUnavailableAlpha;
    }

    private void OnTelemetryUpdate(float framerate, float latency)
    {
        _lastFramerate = framerate;
        _lastLatency = latency;
        _view.TelemetryText.text = string.Format(TELEMETRY_FORMAT, _platform, _buildVersion, _lastFramerate, _lastLatency);
    }


    private void HideObjectives()
    {
        if (_objectivesFadeCoroutine != null)
        {
            StopCoroutine(_objectivesFadeCoroutine);
        }

        StartCoroutine(FadeObjectives());
    }

    private IEnumerator DelayAndFadeObjectives()
    {
        yield return _objectiveFadeDelay;
        yield return FadeObjectives();
    }

    private IEnumerator FadeObjectives()
    {
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
