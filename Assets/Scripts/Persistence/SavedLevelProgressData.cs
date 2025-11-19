using System;

[Serializable]
public class SavedLevelProgressData
{
    public string CurrentLevelEnvironmentName;
    public string CurrentLevelAssetsName;
    public LevelProgressInfo[] CompletedLevelProgressInfoArray;

    private const string DEFAULT_LEVEL_ENVIRONMENT_NAME = "2-office";
    private const string DEFAULT_LEVEL_ASSETS_NAME = "2-office-easy";

    public SavedLevelProgressData()
    {
        CurrentLevelEnvironmentName = DEFAULT_LEVEL_ENVIRONMENT_NAME;
        CurrentLevelAssetsName = DEFAULT_LEVEL_ASSETS_NAME;
        CompletedLevelProgressInfoArray = new LevelProgressInfo[0];
    }
}
