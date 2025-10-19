using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NewScenarioInfo", menuName = "Inertia Pause/Scenario Info")]
public class ScenarioInfo : ScriptableObject
{
    public string ScenarioName;
    public string Description;
    public Sprite Thumbnail;
    public ScenarioDifficulty Difficulty;
    public ScenarioObjectives Objectives;

    public enum ScenarioDifficulty { Normal, Hard }
}

[Serializable]
public class ScenarioObjectives
{
    public List<string> PrimaryObjectives;
    public List<string> SecondaryObjectives;
    public int ActionLimit;
}
