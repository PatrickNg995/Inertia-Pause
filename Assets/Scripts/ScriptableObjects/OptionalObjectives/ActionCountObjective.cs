using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NewActionCountObjective", menuName = "Inertia Pause/Action Count Objective")]
public class ActionCountObjective : OptionalObective
{
    [SerializeField] private int _maxActions = 0;

    // Complete objective if the player's action count is less than or equal to the specified
    // maximum number of actions.
    public override bool CheckCompletion()
    {
        GameManager gameManager = GameManager.Instance;
        IsCompleted = gameManager.ActionCount <= _maxActions;

        return IsCompleted;
    }
}
