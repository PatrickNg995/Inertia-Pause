using UnityEngine;

public class MoveCommand : ActionCommand
{
    // Transform to move.
    private Transform _transform;

    // Initial location and rotation of the object.
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    // Target location and rotation of the object.
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;

    public MoveCommand(InteractionObject interactionObject, Vector3 initialPosition, Quaternion initialRotation,
                       Vector3 targetPosition, Quaternion targetRotation, bool willCountAsAction) : base(interactionObject)
    {
        _transform = interactionObject.transform;

        _initialPosition = initialPosition;
        _initialRotation = initialRotation;

        _targetPosition = targetPosition;
        _targetRotation = targetRotation;

        WillCountAsAction = willCountAsAction;
    }

    public override void Execute()
    {
        // Move the object to the target location and rotation.
        _transform.position = _targetPosition;
        _transform.rotation = _targetRotation;

        // If this command counted as an action,
        // mark the interaction object as having taken an action.
        if (WillCountAsAction)
        {
            ActionObject.HasTakenAction = true;
        }
    }

    public override void Undo()
    {
        // Revert the object to its initial location and rotation.
        _transform.position = _initialPosition;
        _transform.rotation = _initialRotation;

        // If this command counted as an action,
        // mark the interaction object as not having taken an action.
        if (WillCountAsAction)
        {
            ActionObject.HasTakenAction = false;
        }
    }
}
