using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavedLevelProgressManager : MonoBehaviour
{
    public SavedLevelProgressData LevelProgressData => _currentSavedLevelProgressData;

    private const string SAVE_FILE_NAME = "level-progress.json";

    private SavedLevelProgressData _currentSavedLevelProgressData;

    private void Start()
    {
        _currentSavedLevelProgressData = LoadLevelResults();
    }

    public SavedLevelProgressData LoadLevelResults()
    {
        string path = $"{Application.persistentDataPath}/{SAVE_FILE_NAME}";

        try
        {
            string json = File.ReadAllText(path);
            SavedLevelProgressData loadedLevelProgress = JsonUtility.FromJson<SavedLevelProgressData>(json);
            _currentSavedLevelProgressData = loadedLevelProgress;
            return loadedLevelProgress;
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning($"No level progress file found at {path}, returning new level progress file.");
            return new SavedLevelProgressData();
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to load level progress from {path}");
            Debug.LogError(e);
            return new SavedLevelProgressData();
        }
        catch (ArgumentException e)
        {
            Debug.LogError($"Failed to load level progress from {path} (probably a JSON parse error)");
            Debug.LogError(e);
            return new SavedLevelProgressData();
        }
    }

    public void SaveLevelProgress(SavedLevelProgressData levelProgressData)
    {
        string json = JsonUtility.ToJson(levelProgressData);
        string path = $"{Application.persistentDataPath}/{SAVE_FILE_NAME}";

        try
        {
            File.WriteAllText(path, json);
        }
        catch (IOException e)
        {
            Debug.LogError($"Failed to save level results to {path}");
            Debug.LogError(e);
        }

        Debug.Log($"Saved level results to {path}");
        _currentSavedLevelProgressData = levelProgressData;
    }

    public void UpdateLevelProgress(LevelProgressInfo newLevelInfo)
    {
        // Update the current level environment and asset names.
        _currentSavedLevelProgressData.CurrentLevelEnvironmentName = newLevelInfo.LevelEnvironmentName;
        _currentSavedLevelProgressData.CurrentLevelAssetsName = newLevelInfo.LevelAssetsName;

        // Convert the array to a list for easier manipulation.
        List<LevelProgressInfo> levelInfoList = new List<LevelProgressInfo>(_currentSavedLevelProgressData.CompletedLevelProgressInfoArray);

        // Find existing level info and update it, or add a new one if it doesn't exist.
        LevelProgressInfo currentLevelInfo = levelInfoList.Find(levelInfo => levelInfo.LevelAssetsName == newLevelInfo.LevelAssetsName);
        if (currentLevelInfo != null)
        {
            currentLevelInfo.UpdateProgress(newLevelInfo);
        }
        else
        {
            levelInfoList.Add(newLevelInfo);
            _currentSavedLevelProgressData.CompletedLevelProgressInfoArray = levelInfoList.ToArray();
            Debug.Log($"Added new level info for level assets: {newLevelInfo.LevelAssetsName}");
        }

        SaveLevelProgress(_currentSavedLevelProgressData);
    }
}
