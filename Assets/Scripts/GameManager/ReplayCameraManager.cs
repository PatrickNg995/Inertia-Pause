using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReplayCameraManager : MonoBehaviour
{
    /// <summary>
    /// Invoked when the replay sequence starts. The parameter is the number of cameras in the replay sequence.
    /// </summary>
    public Action<int> OnReplayStart;

    /// <summary>
    /// Invoked when the camera changes during the replay sequence. The parameter is the index of the camera in the sequence.
    /// </summary>
    public Action<int> OnCameraChange;

    /// <summary>
    /// Invoked when the replay sequence ends.
    /// </summary>
    public Action OnReplayEnd;

    [Header("TimePauseUnpause Reference")]
    [SerializeField] private TimePauseUnpause _timePauseUnpause;

    [Header("Cameras")]
    [SerializeField] private Camera _playerCamera;
    [Tooltip("Cameras to switch to during the replay sequence, will be switched to in the order they are listed.")]
    [SerializeField] private List<Camera> _replayCameras;

    [Header("Timings")]
    [Tooltip("The delay before unpausing each replay.")]
    [SerializeField] private float _preUnpauseDelay = 0.75f;
    [Tooltip("How long each camera gets to be active before switching to the next one.")]
    [SerializeField] private float _replayDurationPerCamera = 3.25f;

    // Cached wait time.
    private WaitForSeconds _preUnpauseDelayWait;

    // Whether the current replay has received a request to be skipped.
    private bool _isSkipRequested;

    // Input actions.
    private PlayerActions _inputActions;
    private InputAction _skipReplayAction;

    private void Awake()
    {
        _inputActions = new PlayerActions();
        _skipReplayAction = _inputActions.Spectator.Skip;
    }

    private void Start()
    {
        // Ensure only the player camera is active at the start.
        ResetCameras();

        // Cache wait time.
        _preUnpauseDelayWait = new WaitForSeconds(_preUnpauseDelay);

        // Disable skip action at start; only enable during replays.
        _skipReplayAction.Disable();
    }

    private void OnEnable()
    {
        _skipReplayAction.performed += OnSkipPerformed;
        _skipReplayAction.Enable();
    }
    private void OnDisable()
    {
        _skipReplayAction.performed -= OnSkipPerformed;
        _skipReplayAction.Disable();
    }

    private void OnSkipPerformed(InputAction.CallbackContext ctx)
    {
        _isSkipRequested = true;
    }

    public void ResetCameras()
    {
        _playerCamera.gameObject.SetActive(true);
            
        foreach (Camera cam in _replayCameras)
        {
            // Don't disable the camera if it's the player camera.
            if (cam == _playerCamera)
            {
                continue;
            }

            cam.gameObject.SetActive(false);
        }
    }

    public IEnumerator StartReplaySequence()
    {
        // Enable skip action for the duration of the replay.
        _skipReplayAction.Enable();

        OnReplayStart?.Invoke(_replayCameras.Count);

        Camera previousCam = _playerCamera;
        int cameraIndex = 0;

        foreach (Camera replayCam in _replayCameras)
        {
            // Switch camera: deactivate previous, activate current replay camera.
            previousCam.gameObject.SetActive(false);
            replayCam.gameObject.SetActive(true);

            // Update UI
            cameraIndex++;
            OnCameraChange?.Invoke(cameraIndex);

            // Rewind level to initial state for the replay.
            GameManager.Instance.RewindObjects();

            // Delay before unpausing.
            yield return _preUnpauseDelayWait;
            _timePauseUnpause.UnpauseAllObjects();

            // Wait for the replay duration OR until skip is requested.
            float elapsedDuration = 0f;
            _isSkipRequested = false;
            while (elapsedDuration < _replayDurationPerCamera)
            {
                // Skip this replay camera if requested and it is not the last one.
                if (_isSkipRequested && cameraIndex != _replayCameras.Count)
                {
                    break;
                }
                elapsedDuration += Time.deltaTime;
                yield return null;
            }

            // Keep this replay cam enabled only for its duration — next loop will disable it.
            previousCam = replayCam;
        }

        OnReplayEnd?.Invoke();
        _skipReplayAction.Disable();
    }
}
