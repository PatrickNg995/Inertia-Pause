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

    // The index of the currently active replay camera.
    private int _currentCameraIndex = 0;

    // Input actions.
    private PlayerActions _inputActions;
    private InputAction _skipReplayAction;
    private InputAction _cycleCameraNextAction;
    private InputAction _cycleCameraPreviousAction;

    private void Awake()
    {
        _inputActions = new PlayerActions();
        _skipReplayAction = _inputActions.Spectator.Skip;
        _cycleCameraNextAction = _inputActions.Spectator.CycleNext;
        _cycleCameraPreviousAction = _inputActions.Spectator.CyclePrevious;
    }

    private void Start()
    {
        // Ensure only the player camera is active at the start.
        ResetCameras();

        // Cache wait time.
        _preUnpauseDelayWait = new WaitForSeconds(_preUnpauseDelay);
    }

    private void OnEnable()
    {
        _skipReplayAction.performed += OnSkipPerformed;
        _cycleCameraNextAction.performed += CycleCameraNext;
        _cycleCameraPreviousAction.performed += CycleCameraPrevious;
    }
    private void OnDisable()
    {
        _skipReplayAction.performed -= OnSkipPerformed;
        _cycleCameraNextAction.performed -= CycleCameraNext;
        _cycleCameraPreviousAction.performed -= CycleCameraPrevious;

        _skipReplayAction.Disable();
        _cycleCameraNextAction.Disable();
        _cycleCameraPreviousAction.Disable();
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

        // Disable camera cycling actions.
        _cycleCameraNextAction.Disable();
        _cycleCameraPreviousAction.Disable();
    }

    public IEnumerator StartReplaySequence()
    {
        // Enable skip action for the duration of the replay.
        _skipReplayAction.Enable();

        OnReplayStart?.Invoke(_replayCameras.Count);

        // Start with the player camera as the previous camera, effectively camera index -1.
        Camera previousCam = _playerCamera;
        _currentCameraIndex = -1;
        int lastIndex = _replayCameras.Count - 1;

        foreach (Camera replayCam in _replayCameras)
        {
            // Switch camera: deactivate previous, activate current replay camera.
            previousCam.gameObject.SetActive(false);
            replayCam.gameObject.SetActive(true);

            // Update UI
            _currentCameraIndex++;
            OnCameraChange?.Invoke(_currentCameraIndex + 1);

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
                if (_isSkipRequested && _currentCameraIndex != lastIndex)
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

        // Disable skip action and enable cycling actions after the replay.
        _skipReplayAction.Disable();
        _cycleCameraNextAction.Enable();
        _cycleCameraPreviousAction.Enable();
    }

    private void CycleCameraNext(InputAction.CallbackContext ctx)
    {
        // Increment camera index with wrap-around.
        int nextCameraIndex = WrapAroundIndex(_currentCameraIndex + 1, _replayCameras.Count);

        // Deactivate current camera.
        _replayCameras[_currentCameraIndex].gameObject.SetActive(false);

        // Activate next camera.
        _replayCameras[nextCameraIndex].gameObject.SetActive(true);

        // Set new camera index.
        _currentCameraIndex = nextCameraIndex;
    }

    private void CycleCameraPrevious(InputAction.CallbackContext ctx)
    {
        // Decrement camera index with wrap-around.
        int nextCameraIndex = WrapAroundIndex(_currentCameraIndex - 1, _replayCameras.Count);

        // Deactivate current camera.
        _replayCameras[_currentCameraIndex].gameObject.SetActive(false);

        // Activate previous camera.
        _replayCameras[nextCameraIndex].gameObject.SetActive(true);

        // Set new camera index.
        _currentCameraIndex = nextCameraIndex;
    }

    private int WrapAroundIndex(int index, int maxCount)
    {
        int wrappedIndex = index % maxCount;
        if (wrappedIndex < 0)
        {
           wrappedIndex = maxCount - 1;
        }

        return wrappedIndex;
    } 
}
