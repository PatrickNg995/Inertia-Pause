using UnityEngine;

public class OptionsMenuPresenter : MonoBehaviour
{
    [SerializeField] private OptionsMenuView _view;

    private bool _isVerticalSensitivity = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMenu()
    {
        _view.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        CloseFoldouts();
        _view.gameObject.SetActive(false);
    }

    private void CloseFoldouts()
    {
        _view.HorizontalSensitivityButton.CloseFoldout();
        _view.VerticalSensitivityButton.CloseFoldout();
        _view.FOVButton.CloseFoldout();
        _view.MaxFramerateButton.CloseFoldout();
        _view.ShowMetricsButton.CloseFoldout();
        _view.MusicVolumeButton.CloseFoldout();
        _view.SFXVolumeButton.CloseFoldout();
        _view.ShowObjectTrajectoryButton.CloseFoldout();
    }
}
