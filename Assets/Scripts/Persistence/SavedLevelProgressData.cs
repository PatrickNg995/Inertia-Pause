using System;

[Serializable]
public class SavedLevelProgressData
{
    public string CurrentLevelEnvironmentName;
    public string CurrentLevelAssetsName;
    public LevelProgressInfo[] CompletedLevelProgressInfoArray;

    public SavedLevelProgressData()
    {
        CurrentLevelEnvironmentName = "";
        CurrentLevelAssetsName = "";
        CompletedLevelProgressInfoArray = new LevelProgressInfo[0];
    }
}
