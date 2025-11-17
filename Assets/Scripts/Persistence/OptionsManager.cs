using UnityEngine;
using System.IO;
using System;

public class OptionsManager : MonoBehaviour
{
    /// <summary>
    /// Invoked when the value of the "Show Metrics" option is applied.
    /// </summary>
    public Action<bool> OnShowMetricsApplied;

    /// <summary>
    /// Invoked when the value of the "Show Object Trajectory" option is applied.
    /// Useful if the player starts dragging an object, pauses the game, and changes this option.
    /// </summary>
    public Action<bool> OnShowObjectTrajectoryApplied;

    public OptionsModel Options => _currentOptions;

    // These fields can be null if this component is not added to a scenario scene (eg. main menu)
    [Header("Models")]
    [SerializeField] private HUDPresenter _hudPresenter;
    [SerializeField] private PlayerLook _playerLook;

    [Header("Settings")]
    [SerializeField] private bool _isCameraFOVApplied = true;

    private const string SAVE_FILE_NAME = "options.json";

    private OptionsModel _currentOptions;

    void Start()
    {
        _currentOptions = LoadOptions();
        ApplyOptions(_currentOptions);
    }

    /// <summary>
    /// Write a modified OptionsModel to a json file.
    /// </summary>
    /// <param name="options">The options to write.</param>
    public void SaveOptions(OptionsModel options)
    {
        string json = JsonUtility.ToJson(options);
        string path = $"{Application.persistentDataPath}/{SAVE_FILE_NAME}";

        try
        {
            File.WriteAllText(path, json);
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to save options to {path}");
            Debug.LogError(e);
        }

        Debug.Log($"Saved options to {path}");
        _currentOptions = options;
    }

    /// <summary>
    /// Load the OptionsModel from a json file, or a new OptionsModel if file does not exist.
    /// </summary>
    /// <returns>The loaded OptionsModel.</returns>
    public OptionsModel LoadOptions()
    {
        string path = $"{Application.persistentDataPath}/{SAVE_FILE_NAME}";

        try
        {
            string json = File.ReadAllText(path);
            OptionsModel options = JsonUtility.FromJson<OptionsModel>(json);
            _currentOptions = options;
            return options;
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning($"No options file found at {path}, returning new options file.");
            return new OptionsModel();
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to load options from {path}");
            Debug.LogError(e);
            return new OptionsModel();
        }
        catch (ArgumentException e)
        {
            Debug.LogError($"Failed to load options from {path} (probably a JSON parse error)");
            Debug.LogError(e);
            return new OptionsModel();
        }
    }

    /// <summary>
    /// Apply game options. This does NOT save game options, so if the OptionsModel is modified, call SaveOptions after.
    /// </summary>
    /// <param name="options"></param>
    public void ApplyOptions(OptionsModel options)
    {
        ApplySensitivityOptions(options.HorizontalSensitivity, options.VerticalSensitivity);

        if (_hudPresenter != null)
        {
            _hudPresenter.ShowTelemetry(options.IsMetricsShown);
        }

        if (Camera.main != null && _isCameraFOVApplied)
        {
            Camera.main.fieldOfView = options.FieldOfView;
        }

        Application.targetFrameRate = options.MaxFramerate;
        OnShowMetricsApplied?.Invoke(options.IsMetricsShown);
        OnShowObjectTrajectoryApplied?.Invoke(options.IsObjectTrajectoryShown);

        // TODO: Apply volume settings
    }

    private void ApplySensitivityOptions(int horizontalSensitivity, int verticalSensitivity)
    {
        if (_playerLook != null)
        {
            _playerLook.HorizontalSensitivity = SensitivityOptionToActualSensitivity(horizontalSensitivity);
            _playerLook.VerticalSensitivity = SensitivityOptionToActualSensitivity(verticalSensitivity);
        }
    }

    private float SensitivityOptionToActualSensitivity(int sensitivityOption)
    {
        return sensitivityOption switch
        {
            1 => 0.01f,
            2 => 0.05f,
            3 => 0.10f,
            4 => 0.15f,
            5 => 0.25f,
            6 => 0.50f,
            7 => 1.00f,
            _ => 0.15f,
        };
    }
}
