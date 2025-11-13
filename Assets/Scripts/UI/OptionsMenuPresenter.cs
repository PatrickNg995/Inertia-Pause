using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private OptionsMenuView _view;

    [Header("Models")]
    [SerializeField] private OptionsManager _optionsManager;
    [SerializeField] private GameManager _gameManager;

    private OptionsModel _dirtyOptionsModel;
    private bool _isSettingHorizontalSensitivity = false;
    private bool _isSettingMusicVolume = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _view.gameObject.SetActive(false);

        if (_gameManager.ScenarioInfo != null)
        {
            _view.LevelNameText.text = _gameManager.ScenarioInfo.ScenarioName;
        }

        SetupFoldouts();
        SetupToggles();
    }

    public void OpenMenu()
    {
        _view.gameObject.SetActive(true);
        _dirtyOptionsModel = _optionsManager.Options.Clone();
        ShowInitialValues(_dirtyOptionsModel);
    }

    public void CloseMenu()
    {
        CloseFoldouts();
        _optionsManager.ApplyOptions(_dirtyOptionsModel);
        _optionsManager.SaveOptions(_dirtyOptionsModel);
        _view.gameObject.SetActive(false);
    }

    private void SetupFoldout(CustomOptionButton button)
    {
        button.Button.onClick.AddListener(() =>
        {
            CloseFoldouts();
            button.OpenFoldout();
        });
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

    private void ShowInitialValues(OptionsModel options)
    {
        // TODO: Show initial option values
    }

    private void SetupFoldouts()
    {
        // Foldout setup
        SetupFoldout(_view.HorizontalSensitivityButton);
        _view.HorizontalSensitivityButton.Button.onClick.AddListener(() => _isSettingHorizontalSensitivity = true);
        SetupFoldout(_view.VerticalSensitivityButton);
        _view.VerticalSensitivityButton.Button.onClick.AddListener(() => _isSettingHorizontalSensitivity = false);

        SetupFoldout(_view.FOVButton);
        SetupFoldout(_view.MaxFramerateButton);
        SetupFoldout(_view.MusicVolumeButton);

        _view.MusicVolumeButton.Button.onClick.AddListener(() => _isSettingMusicVolume = true);
        SetupFoldout(_view.SFXVolumeButton);
        _view.SFXVolumeButton.Button.onClick.AddListener(() => _isSettingMusicVolume = false);

        // Sensitivity buttons
        for (int i = 0; i < _view.SensitivityOptions.Count; i++)
        {
            Button sensitivityButton = _view.SensitivityOptions[i];
            sensitivityButton.onClick.AddListener(() => {
                ChangeSensitivity(i + 1);
                CloseFoldouts();
            });
        }

        // FOV buttons
        for (int i = 0; i < _view.FOVOptions.Count; i++)
        {
            Button fovButton = _view.FOVOptions[i];
            fovButton.onClick.AddListener(() => {
                ChangeFieldOfView(i);
                CloseFoldouts();
            });
        }

        // Framerate buttons
        for (int i = 0; i < _view.FramerateOptions.Count; i++)
        {
            Button framerateButton = _view.FramerateOptions[i];
            framerateButton.onClick.AddListener(() => {
                ChangeFramerate(i);
                CloseFoldouts();
            });
        }

        // Volume buttons
        for (int i = 0; i < _view.VolumeOptions.Count; i++)
        {
            Button volumeButton = _view.VolumeOptions[i];
            volumeButton.onClick.AddListener(() =>
            {
                ChangeVolume(i + 1);
                CloseFoldouts();
            });
        }
    }

    private void SetupToggles()
    {
        _view.ShowMetricsButton.Button.onClick.AddListener(ToggleShowMetrics);
        _view.ShowObjectTrajectoryButton.Button.onClick.AddListener(ToggleShowObjectTrajectory);
    }

    private void ToggleShowMetrics()
    {
        _dirtyOptionsModel.IsMetricsShown = !_dirtyOptionsModel.IsMetricsShown;
    }

    private void ToggleShowObjectTrajectory()
    {
        _dirtyOptionsModel.IsObjectTrajectoryShown = !_dirtyOptionsModel.IsObjectTrajectoryShown;
    }

    private void ChangeSensitivity(int sensitivity)
    {
        if (_isSettingHorizontalSensitivity)
        {
            _dirtyOptionsModel.HorizontalSensitivity = sensitivity;
        }
        else
        {
            _dirtyOptionsModel.VerticalSensitivity = sensitivity;
        }
    }

    private void ChangeFramerate(int framerateOptionIndex)
    {
        int framerateOption = framerateOptionIndex switch
        {
            0 => 30,
            1 => 60,
            2 => 120,
            3 => 180,
            4 => 240,
            5 => -1,
            _ => -1,
        };
        _dirtyOptionsModel.MaxFramerate = framerateOption;
    }

    private void ChangeFieldOfView(int fieldOfViewOptionIndex)
    {
        int fieldOfViewOption = fieldOfViewOptionIndex switch
        {
            0 => 60,
            1 => 70,
            2 => 80,
            3 => 90,
            4 => 100,
            5 => 110,
            6 => 120,
            _ => 80,
        };
        _dirtyOptionsModel.FieldOfView = fieldOfViewOption;
    }

    private void ChangeVolume(int volume)
    {
        if (_isSettingMusicVolume)
        {
            _dirtyOptionsModel.MusicVolume = volume;
        }
        else
        {
            _dirtyOptionsModel.SoundVolume = volume;
        }
    }
}
