using System;
using System.Collections.Generic;
using UnityEngine;

// Used in the editor to select the way the objective should be evaluated.
public enum ActionRequirementMode
{
    ActionsEqualTo,
    ActionsGreaterThanOrEqualTo,
    ActionsLessThanOrEqualTo
}

[Serializable]
[CreateAssetMenu(fileName = "NewObjectActionObjective", menuName = "Inertia Pause/Object Action Objective")]
public class ObjectActionObjective : OptionalObjective
{
    [Tooltip("The tags of the object types that will be used for the objective (the tag should be plural, e.g. 'Bullets').")]
    [SerializeField] private List<string> _objectTypeTagsToRestrict;

    [Tooltip("The number of actions with the specified object type that are required for the objective.")]
    [SerializeField] private int _actionTarget = 0;

    [Tooltip("Set if this objective should require you to =, >= or <= the action target.")]
    [SerializeField] private ActionRequirementMode _actionRequirementMode = ActionRequirementMode.ActionsEqualTo;

    // Complete objective if no actions have been performed on objects of the specified type.
    // OR Complete objective if all actions have been performed on objects of the specified type.
    public override bool CheckCompletion()
    {
        int actionCount = 0;
        // Check if the object type was interacted with by going through the executed command list.
        foreach (ActionCommand command in GameManager.Instance.UndoCommandList)
        {
            GameObject actionGameObject = command.ActionObject.gameObject;

            // Check if the action was performed on an object of the specified type and only count it if the command
            // counted as an action.
            if (command.WillCountAsAction && _objectTypeTagsToRestrict.Contains(actionGameObject.transform.root.tag))
            {
                actionCount++;
            }
        }

        // Switch based on action requirement mode to determine if objective is completed.
        switch (_actionRequirementMode)
        {
            case ActionRequirementMode.ActionsEqualTo:
                return actionCount == _actionTarget;
            case ActionRequirementMode.ActionsGreaterThanOrEqualTo:
                return actionCount >= _actionTarget;
            case ActionRequirementMode.ActionsLessThanOrEqualTo:
                return actionCount <= _actionTarget;
            default:
                Debug.LogError("Unsupported ActionRequirementMode in ObjectActionObjective.");
                return false;
        }
    }
}
