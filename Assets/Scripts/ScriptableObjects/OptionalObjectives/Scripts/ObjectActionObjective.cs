using System;
using UnityEngine;

// Used to make the mode selection in the editor more readable.
public enum ActionCompletionMode
{
    NoActionsAllowed,
    AllActionsRequired
}

[Serializable]
[CreateAssetMenu(fileName = "NewObjectActionObjective", menuName = "Inertia Pause/Object Action Objective")]
public class ObjectActionObjective : OptionalObective
{
    [Tooltip("The tag of the object type to restrict for the objective (the tag should be plural, e.g. 'Bullets').")]
    [SerializeField] private string _objectTypeTagToRestrict;

    [Tooltip("Set if this objective should have NO actions with the object type allowed, " +
             "or if it should have ONLY actions with the object type allowed.")]
    [SerializeField] private ActionCompletionMode _actionsAllowed = ActionCompletionMode.NoActionsAllowed;

    // Boolean array to map ActionCompletionMode to a bool for easier checking.
    private bool[] _actionCompletionMode = { true, false };

    // Complete objective if no actions have been performed on objects of the specified type.
    // OR Complete objective if all actions have been performed on objects of the specified type.
    public override bool CheckCompletion()
    {
        // Map the enum to a bool to determine completion mode.
        bool actionCompletionMode = _actionCompletionMode[(int)_actionsAllowed];

        // Check if the object type was interacted with by going through the executed command list.
        foreach (ActionCommand command in GameManager.Instance.UndoCommandList)
        {
            GameObject actionGameObject = command.ActionObject.gameObject;
            if (actionGameObject.transform.root.CompareTag(_objectTypeTagToRestrict) && actionCompletionMode)
            {
                return false;
            }
        }
        return true;
    }
}
