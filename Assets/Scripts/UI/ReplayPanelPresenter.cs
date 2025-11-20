using UnityEngine;

public class ReplayPanelPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private ReplayPanelView _view;

    [Header("Models")]
    [SerializeField] private ReplayCameraManager _replayCameraManager;

    private const string CAMERA_NUMBER_FORMAT = "{0}/{1}";
    private int _numberOfCameras;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _view.gameObject.SetActive(false);

        _replayCameraManager.OnReplayStart += OnReplayStart;
        _replayCameraManager.OnCameraChange += OnCameraChange;
        _replayCameraManager.OnReplayEnd += CloseMenu;
    }

    public void OpenMenu()
    {
        _view.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        _view.gameObject.SetActive(false);
    }

    private void OnReplayStart(int numberOfCameras)
    {
        _numberOfCameras = numberOfCameras;
        _view.CameraText.text = string.Format(CAMERA_NUMBER_FORMAT, 1, _numberOfCameras);
        OpenMenu();
    }

    private void OnCameraChange(int currentCameraIndex)
    {
        _view.CameraText.text = string.Format(CAMERA_NUMBER_FORMAT, currentCameraIndex, _numberOfCameras);
    }
}
