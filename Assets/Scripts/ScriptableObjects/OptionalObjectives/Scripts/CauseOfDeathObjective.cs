using System;
using System.Collections.Generic;
using UnityEngine;

// Used in the editor to select the way the objective should be evaluated.
public enum KillRequirementMode
{
    KillEqualTo,
    KillGreaterThanOrEqualTo,
    KillLessThanOrEqualTo
}

[Serializable]
[CreateAssetMenu(fileName = "NewCauseOfDeathObjective", menuName = "Inertia Pause/Cause of Death Objective")]
public class CauseOfDeathObjective : OptionalObjective
{
    [Tooltip("The tags of the object types to require for the objective (the tag should be plural, e.g. 'Bullets').")]
    [SerializeField] private List<string> _objectTypeTagsToRequire;

    [Tooltip("The number of kills with the specified object type required to complete the objective.")]
    [SerializeField] private int _killNumberRequirement;

    [Tooltip("Set if this objective should require you to kill equal or more than the requirement, " +
             "or if it should require you to kill equal or less than the requirement.")]
    [SerializeField] private KillRequirementMode _killRequirementMode = KillRequirementMode.KillGreaterThanOrEqualTo;

    // Complete objective if the kill count caused by objects of the specified type is greater than or equal to
    // the kill number requirement OR if the kill count is less than or equal to the kill number requirement.
    public override bool CheckCompletion()
    {
        // Count the number of kills caused by objects with the specified tag.
        int killCount = 0;
        foreach (GameObject gameObject in GameManager.Instance.ListOfCausesOfDeath)
        {
            // Note: since objectives are only counted as completed if the level is completed, we can assume that
            // all the deaths in ListOfCausesOfDeath are Enemy deaths, so we don't need to check for that here.
            // If any allies are killed, the objective will be counted as failed by the GameManager anyway.
            if (_objectTypeTagsToRequire.Contains(gameObject.transform.root.tag))
            {
                killCount++;
            }
        }

        // Switch based on the kill requirement mode to check if the objective is completed.
        switch (_killRequirementMode)
        {
            case KillRequirementMode.KillEqualTo:
                return killCount == _killNumberRequirement;
            case KillRequirementMode.KillGreaterThanOrEqualTo:
                return killCount >= _killNumberRequirement;
            case KillRequirementMode.KillLessThanOrEqualTo:
                return killCount <= _killNumberRequirement;
            default:
                Debug.LogError("Unsupported KillRequirementMode in CauseOfDeathObjective.");
                return false;
        }
    }
}
