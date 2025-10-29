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

    private bool _hasFinalRotation;
    private Vector3 _finalPosition;
    private Quaternion _finalRotation;

    public ToppleCommand(InteractionObject interactionObject, Rigidbody rb,
                         Vector3 direction, float torque) : base(interactionObject)
    {
        _rb = rb;
        _transform = interactionObject.transform;

        _torque = torque;
        _direction = direction;

        _initialPosition = _transform.position;
        _initialRotation = _transform.rotation;
    }

    public override void RelinkActionObjectReference()
    {
        base.RelinkActionObjectReference();
        _transform = ActionObject.transform;
    }

    public override void Execute()
    {
        // Apply torque to the rigidbody to topple the object.
        //if (!_hasFinalRotation)
        //{
        //    _rb.AddTorque(_direction * _torque);
        //}
        //else
        //{
        //    _transform.position = _finalPosition;
        //    _transform.rotation = _finalRotation;
        //}
        
        // Mark the interaction object as having taken an action.
        ActionObject.HasTakenAction = true;
        _transform.Rotate(Vector3.right, 5f);
        _transform.Translate(_direction * 0.15f);

        if (ActionObject is ShelfBehaviour behaviour)
        {
            behaviour.IsToppled = true;
        }

    }

    public override void Undo()
    {
        //if ((_transform.position != _initialPosition || _transform.rotation != _initialRotation) && !_hasFinalRotation)
        //{
        //    _hasFinalRotation = true;
        //    _finalPosition = _transform.position;
        //    _finalRotation = _transform.rotation;
        //}

        // Revert the object to its initial location and rotation.
        _transform.position = _initialPosition;
        _transform.rotation = _initialRotation;

        if (ActionObject is ShelfBehaviour behaviour)
        {
            behaviour.IsToppled = false;
        }

        // Mark the interaction object as not having taken an action.
        ActionObject.HasTakenAction = false;
        ActionObject.OnCommandUndo();
    }
}
