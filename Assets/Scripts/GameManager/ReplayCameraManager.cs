using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayCameraManager : MonoBehaviour
{
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

    // Cached wait times.
    private WaitForSeconds _preUnpauseDelayWait;
    private WaitForSeconds _replayDelayWait;

    private void Start()
    {
        // Ensure only the player camera is active at the start.
        ResetCameras();

        // Cache wait times.
        _preUnpauseDelayWait = new WaitForSeconds(_preUnpauseDelay);
        _replayDelayWait = new WaitForSeconds(_replayDurationPerCamera);
    }

    public void ResetCameras()
    {
        _playerCamera.gameObject.SetActive(true);

        foreach (Camera cam in _replayCameras)
        {
            cam.gameObject.SetActive(false);
        }
    }

    public IEnumerator StartReplaySequence()
    {
        Camera previousCam = _playerCamera;

        foreach (Camera replayCam in _replayCameras)
        {
            // Switch camera: deactivate previous, activate current replay camera.
            previousCam.gameObject.SetActive(false);
            replayCam.gameObject.SetActive(true);

            // Rewind level to initial state for the replay.
            GameManager.Instance.RewindObjects();

            // Delay before unpausing.
            yield return _preUnpauseDelayWait;
            _timePauseUnpause.UnpauseAllObjects();

            // Delay for the duration of this replay camera.
            yield return _replayDelayWait;

            // Keep this replay cam enabled only for its duration — next loop will disable it.
            previousCam = replayCam;
        }
    }
}
