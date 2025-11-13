using System;
using UnityEngine;

// Used to make the mode selection in the editor make more sense.
public enum KillRequirementMode
{
    KillGreaterThanOrEqualTo,
    KillLessThanOrEqualTo
}

[Serializable]
[CreateAssetMenu(fileName = "NewCauseOfDeathRequirementObjective", menuName = "Inertia Pause/Cause of Death Requirement Objective")]
public class CauseOfDeathRequirementObjective : OptionalObective
{
    [SerializeField] private string _objectTypeToRequireTag;

    [SerializeField] private int _killNumberRequirement;

    [Tooltip("Set if this objective should require you to kill equal or more than the requirement, " +
             "or if it should require you to kill equal or less than the requirement.")]
    [SerializeField] private KillRequirementMode _killRequirementMode = KillRequirementMode.KillGreaterThanOrEqualTo;

    // Complete objective if the kill count caused by objects of the specified type is greater than or equal to
    // the kill number requirement OR if the kill count is less than or equal to the kill number requirement.
    public override bool CheckCompletion()
    {
        GameManager gameManager = GameManager.Instance;
        int killCount = 0;

        foreach (GameObject gameObject in gameManager.ListOfCausesOfDeath)
        {
            if (gameObject.transform.root.CompareTag(_objectTypeToRequireTag))
            {
                killCount++;
            }
        }

        // Switch based on the kill requirement mode to check if the objective is completed.
        switch (_killRequirementMode)
        {
            case KillRequirementMode.KillGreaterThanOrEqualTo:
                if (killCount >= _killNumberRequirement)
                {
                    IsCompleted = true;
                    break;
                }
                else
                {
                    IsCompleted = false;
                    break;
                }
            case KillRequirementMode.KillLessThanOrEqualTo:
                if (killCount <= _killNumberRequirement)
                {
                    IsCompleted = true;
                    break;
                }
                else
                {
                    IsCompleted = false;
                    break;
                }
        }
        return IsCompleted;
    }
}
