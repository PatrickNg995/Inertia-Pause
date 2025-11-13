using System;
using UnityEngine;

// Used to make the mode selection in the editor make more sense.
public enum ActionCompletionMode
{
    NoActionsAllowed,
    AllActionsRequired
}

[Serializable]
[CreateAssetMenu(fileName = "NewRestrictObjectActionObjective", menuName = "Inertia Pause/Restrict Object Action Objective")]
public class RestrictObjectActionObjective : OptionalObective
{
    [SerializeField] private string _objectTypeTagToRestrict;

    [Tooltip("Set if this objective should have no actions with the object type allowed, " +
             "or if it should have only actions with the object type allowed.")]
    [SerializeField] private ActionCompletionMode _actionsAllowed = ActionCompletionMode.NoActionsAllowed;

    // Boolean array to map ActionCompletionMode to a bool for easier checking.
    private bool[] _actionCompletionMode = { true, false };

    // Complete objective if no actions have been performed on objects of the specified type.
    // OR Complete objective if all actions have been performed on objects of the specified type.
    public override bool CheckCompletion()
    {
        GameManager gameManager = GameManager.Instance;
        bool actionCompletionMode = _actionCompletionMode[(int)_actionsAllowed];

        foreach (ActionCommand command in gameManager.UndoCommandList)
        {
            GameObject actionGameObject = command.ActionObject.gameObject;
            if (actionGameObject.transform.root.CompareTag(_objectTypeTagToRestrict) && actionCompletionMode)
            {
                IsCompleted = false;
                return IsCompleted;
            }
        }
        IsCompleted = true;
        return IsCompleted;
    }
}
