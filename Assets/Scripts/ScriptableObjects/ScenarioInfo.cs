using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NewScenarioInfo", menuName = "Inertia Pause/Scenario Info")]
public class ScenarioInfo : ScriptableObject
{
    public string ScenarioName;
    [TextArea] public string Description;
    public Sprite Thumbnail;
    public ScenarioDifficulty Difficulty;
    public ScenarioObjectives Objectives;
    public int NumberOfCivilians;
    public int NumberOfAllies;
    public int NumberOfEnemies;

    public string NextEnvironmentSceneName;
    public string NextScenarioAssetsSceneName;

    public enum ScenarioDifficulty { Normal, Hard }
}

[Serializable]
public class ScenarioObjectives
{
    public List<string> MainObjectives;
    public List<string> OptionalObjectives;
    public int ActionLimit;
}
