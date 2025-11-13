using System;

[Serializable]
public class OptionsModel
{
    public int HorizontalSensitivity;
    public int VerticalSensitivity;
    public int FieldOfView;

    public int MaxFramerate;
    public bool IsMetricsShown;

    public int MusicVolume;
    public int SoundVolume;

    public bool IsObjectTrajectoryShown;

    public OptionsModel()
    {
        HorizontalSensitivity = 4;
        VerticalSensitivity = 4;
        FieldOfView = 80;
        MaxFramerate = -1;
        IsMetricsShown = true;
        MusicVolume = 8;
        SoundVolume = 8;
        IsObjectTrajectoryShown = true;
    }
}

public static class OptionsModelUtil
{
    public static OptionsModel Clone(this OptionsModel model)
    {
        return new OptionsModel()
        {
            HorizontalSensitivity = model.HorizontalSensitivity,
            VerticalSensitivity = model.VerticalSensitivity,
            FieldOfView = model.FieldOfView,
            MaxFramerate = model.MaxFramerate,
            IsMetricsShown = model.IsMetricsShown,
            MusicVolume = model.MusicVolume,
            SoundVolume = model.SoundVolume,
            IsObjectTrajectoryShown = model.IsObjectTrajectoryShown,
        };
    }
}
