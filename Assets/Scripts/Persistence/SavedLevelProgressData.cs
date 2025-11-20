using System;

[Serializable]
public class SavedLevelProgressData
{
    public string CurrentLevelEnvironmentName;
    public string CurrentLevelAssetsName;
    public LevelProgressInfo[] CompletedLevelProgressInfoArray;

    public SavedLevelProgressData()
    {
        CurrentLevelEnvironmentName = string.Empty;
        CurrentLevelAssetsName = string.Empty;
        CompletedLevelProgressInfoArray = new LevelProgressInfo[0];
    }
}
