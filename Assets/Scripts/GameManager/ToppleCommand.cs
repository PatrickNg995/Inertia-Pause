using UnityEngine;

public class ToppleCommand : ICommand
{
    // References to the interaction object and its components.
    private InteractionObject _interactionObject;
    private Rigidbody _rb;
    private Transform _transform;

    // Torque and direction for the topple action.
    private float _torque;
    private Vector3 _direction;

    // Initial location and rotation of the object.
    private Vector3 _initialLocation;
    private Quaternion _initialRotation;

    public ToppleCommand(InteractionObject interactionObject, Rigidbody rb,
                         Vector3 direction, float torque)
    {
        _interactionObject = interactionObject;
        _rb = rb;
        _transform = interactionObject.transform;

        _torque = torque;
        _direction = direction;

        _initialLocation = _transform.position;
        _initialRotation = _transform.rotation;
    }

    public void Execute()
    {
        // Apply torque to the rigidbody to topple the object.
        _rb.AddTorque(_direction * _torque);

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
