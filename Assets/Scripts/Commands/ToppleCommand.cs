using UnityEngine;

public class ToppleCommand : ActionCommand
{
    // References to the interaction object and its components.
    private Rigidbody _rb;
    private Transform _transform;

    // final position and rotation, and direction for the topple action.
    private Vector3 _finalPosition;
    private Quaternion _finalRotation;
    private Vector3 _direction;

    // Initial location and rotation of the object.
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    public ToppleCommand(InteractionObject interactionObject, Rigidbody rb,
                         Vector3 direction, float deltaForward, float deltaUp, float finalRotationScale) : base(interactionObject)
    {
        _rb = rb;
        _transform = interactionObject.transform;

        _direction = direction;
        _finalPosition = _rb.transform.position + _rb.transform.forward * deltaForward + _rb.transform.up * deltaUp;
        //Quaternion rotationDirection = Quaternion.Euler(_direction * finalRotationScale);
        //_finalRotation = _rb.transform.rotation * rotationDirection;
        _finalRotation = Quaternion.AngleAxis(finalRotationScale, _direction) * _rb.transform.rotation;


        _initialPosition = _transform.position;
        _initialRotation = _transform.rotation;
    }

    public override void Execute()
    {
        _rb.transform.position = _finalPosition;
        _rb.transform.rotation = _finalRotation;
        

        // Mark the interaction object as having taken an action.
        ActionObject.HasTakenAction = true;

        ActionObject.OnCommandRedo();
    }

    public override void Undo()
    {
        // Revert the object to its initial location and rotation.
        _transform.position = _initialPosition;
        _transform.rotation = _initialRotation;

        // Mark the interaction object as not having taken an action.
        ActionObject.HasTakenAction = false;
        ActionObject.OnCommandUndo();
    }
}
