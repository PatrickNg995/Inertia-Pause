using UnityEngine;

public class DragCommand : ActionCommand
{
    private Transform _transform;
    private Vector3 _initialPosition;
    private Vector3 _finalPosition;

    // to allow for a reset command to be made
    public DragCommand(InteractionObject interactionObject, Vector3 initialPosition, Vector3 finalPosition, bool willCountAsAction) : base(interactionObject)
    {
        _transform = interactionObject.transform;
        _initialPosition = initialPosition;
        _finalPosition = finalPosition;
        WillCountAsAction = willCountAsAction;
    }

    public override void RelinkActionObjectReference()
    {
        base.RelinkActionObjectReference();
        _transform = ActionObject.transform;
    }

    public override void Execute()
    {
        _transform.position = _finalPosition;

        // If this command counted as an action,
        // mark the interaction object as having taken an action.
        if (WillCountAsAction)
        {
            ActionObject.HasTakenAction = true;
        }
    }

    public override void Undo()
    {
        _transform.position = _initialPosition;

        // If this command counted as an action,
        // mark the interaction object as not having taken an action.
        if (WillCountAsAction)
        {
            ActionObject.HasTakenAction = false;
        }
    }
}
