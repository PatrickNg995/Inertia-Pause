using UnityEngine;

public class MoveCommand : ICommand
{
    // References to the interaction object being moved and its transform.
    private InteractionObject _interactionObject;
    private Transform _transform;

    // Initial location and rotation of the object.
    private Vector3 _initialLocation;
    private Quaternion _initialRotation;

    // Target location and rotation of the object.
    private Vector3 _targetLocation;
    private Quaternion _targetRotation;

    public MoveCommand(InteractionObject interactionObject,
                       Vector3 targetLocation, Quaternion targetRotation)
    {
        _interactionObject = interactionObject;
        _transform = interactionObject.transform;

        _initialLocation = _transform.position;
        _initialRotation = _transform.rotation;

        _targetLocation = targetLocation;
        _targetRotation = targetRotation;   
    }
    public void Execute()
    {
        // Move the object to the target location and rotation.
        _transform.position = _targetLocation;
        _transform.rotation = _targetRotation;

        // Mark the interaction object as having taken an action.
        _interactionObject.HasTakenAction = true;
    }

    public void Undo()
    {
        // Revert the object to its initial location and rotation.
        _transform.position = _initialLocation;
        _transform.rotation = _initialRotation;

        // Mark the interaction object as not having taken an action.
        _interactionObject.HasTakenAction = false;
    }
}
