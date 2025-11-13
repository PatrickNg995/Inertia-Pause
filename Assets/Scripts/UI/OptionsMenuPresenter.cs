using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuPresenter : MonoBehaviour
{
    /// <summary>
    /// Invoked when this menu is closed.
    /// </summary>
    public Action OnMenuClose;

    [Header("View")]
    [SerializeField] private OptionsMenuView _view;

    [Header("Models")]
    [SerializeField] private OptionsManager _optionsManager;
    [SerializeField] private GameManager _gameManager;

    private const string OPTION_TRUE_TEXT = "On";
    private const string OPTION_FALSE_TEXT = "Off";

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

        _view.BackButton.onClick.AddListener(CloseMenu);

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
        OnMenuClose?.Invoke();
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
        _view.MusicVolumeButton.CloseFoldout();
        _view.SFXVolumeButton.CloseFoldout();
    }

    private void ShowInitialValues(OptionsModel options)
    {
        _view.HorizontalSensitivityButton.OptionText.text = options.HorizontalSensitivity.ToString();
        _view.VerticalSensitivityButton.OptionText.text = options.VerticalSensitivity.ToString();
        _view.FOVButton.OptionText.text = options.FieldOfView.ToString();
        _view.MaxFramerateButton.OptionText.text = options.MaxFramerate.ToString();
        _view.ShowMetricsButton.OptionText.text = options.IsMetricsShown ? OPTION_TRUE_TEXT : OPTION_FALSE_TEXT;
        _view.MusicVolumeButton.OptionText.text = options.MusicVolume.ToString();
        _view.SFXVolumeButton.OptionText.text = options.SoundVolume.ToString();
        _view.ShowObjectTrajectoryButton.OptionText.text = options.IsObjectTrajectoryShown ? OPTION_TRUE_TEXT : OPTION_FALSE_TEXT;
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
            int index = i;
            Button sensitivityButton = _view.SensitivityOptions[index];
            sensitivityButton.onClick.AddListener(() => {
                ChangeSensitivity(index + 1);
                CloseFoldouts();
            });
        }

        // FOV buttons
        for (int i = 0; i < _view.FOVOptions.Count; i++)
        {
            int index = i;
            Button fovButton = _view.FOVOptions[index];
            fovButton.onClick.AddListener(() => {
                ChangeFieldOfView(index);
                CloseFoldouts();
            });
        }

        // Framerate buttons
        for (int i = 0; i < _view.FramerateOptions.Count; i++)
        {
            int index = i;
            Button framerateButton = _view.FramerateOptions[index];
            framerateButton.onClick.AddListener(() => {
                ChangeFramerate(index);
                CloseFoldouts();
            });
        }

        // Volume buttons
        for (int i = 0; i < _view.VolumeOptions.Count; i++)
        {
            int index = i;
            Button volumeButton = _view.VolumeOptions[index];
            volumeButton.onClick.AddListener(() =>
            {
                ChangeVolume(index + 1);
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
        _view.ShowMetricsButton.OptionText.text = _dirtyOptionsModel.IsMetricsShown ? OPTION_TRUE_TEXT : OPTION_FALSE_TEXT;
    }

    private void ToggleShowObjectTrajectory()
    {
        _dirtyOptionsModel.IsObjectTrajectoryShown = !_dirtyOptionsModel.IsObjectTrajectoryShown;
        _view.ShowObjectTrajectoryButton.OptionText.text = _dirtyOptionsModel.IsObjectTrajectoryShown ? OPTION_TRUE_TEXT : OPTION_FALSE_TEXT;
    }

    private void ChangeSensitivity(int sensitivity)
    {
        if (_isSettingHorizontalSensitivity)
        {
            _dirtyOptionsModel.HorizontalSensitivity = sensitivity;
            _view.HorizontalSensitivityButton.OptionText.text = _dirtyOptionsModel.HorizontalSensitivity.ToString();
        }
        else
        {
            _dirtyOptionsModel.VerticalSensitivity = sensitivity;
            _view.VerticalSensitivityButton.OptionText.text = _dirtyOptionsModel.VerticalSensitivity.ToString();
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
        _view.MaxFramerateButton.OptionText.text = _dirtyOptionsModel.MaxFramerate.ToString();
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
        _view.FOVButton.OptionText.text = _dirtyOptionsModel.FieldOfView.ToString();
    }

    private void ChangeVolume(int volume)
    {
        if (_isSettingMusicVolume)
        {
            _dirtyOptionsModel.MusicVolume = volume;
            _view.MusicVolumeButton.OptionText.text = _dirtyOptionsModel.MusicVolume.ToString();
        }
        else
        {
            _dirtyOptionsModel.SoundVolume = volume;
            _view.SFXVolumeButton.OptionText.text = _dirtyOptionsModel.SoundVolume.ToString();
        }
    }
}
