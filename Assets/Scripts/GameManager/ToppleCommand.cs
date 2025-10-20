using UnityEngine;

public class ToppleCommand : ActionCommand
{
    // References to the interaction object and its components.
    private Rigidbody _rb;
    private Transform _transform;

    // Torque and direction for the topple action.
    private float _torque;
    private Vector3 _direction;

    // Initial location and rotation of the object.
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    public ToppleCommand(InteractionObject interactionObject, Rigidbody rb,
                         Vector3 direction, float torque)
    {
        ActionObject = interactionObject;
        _rb = rb;
        _transform = interactionObject.transform;

        _torque = torque;
        _direction = direction;

        _initialPosition = _transform.position;
        _initialRotation = _transform.rotation;
    }

    public override void Execute()
    {
        // Apply torque to the rigidbody to topple the object.
        _rb.AddTorque(_direction * _torque);

        // Mark the interaction object as having taken an action.
        ActionObject.HasTakenAction = true;
    }

    public override void Undo()
    {
        // Revert the object to its initial location and rotation.
        _transform.position = _initialPosition;
        _transform.rotation = _initialRotation;

        // Mark the interaction object as not having taken an action.
        ActionObject.HasTakenAction = false;
    }
}
