using UnityEngine;

public class DragCommand : ActionCommand
{
    private Transform _transform;
    private Transform _playerCamera;
    private Vector3 _initialPosition;
    private Vector3 _finalPosition;

    private float _dragSpeed;
    private float _dragDistance;
    private float _yPosition;
    private bool _isActiveDrag;
    private float _maxDragDistance;

    public DragCommand(InteractionObject interactionObject, Transform playerCamera, float dragSpeed, float maxDistance)
    {
        ActionObject = interactionObject;
        _transform = interactionObject.transform;
        _playerCamera = playerCamera;
        _dragSpeed = dragSpeed;
        _maxDragDistance = maxDistance;

        _initialPosition = _transform.position;
        _yPosition = _transform.position.y;

        _dragDistance = Vector3.Distance(_playerCamera.position, _transform.position);

        _isActiveDrag = true;
    }

    // to allow for a reset command to be made
    public DragCommand(InteractionObject interactionObject, Vector3 initialPosition, Vector3 finalPosition)
    {
        ActionObject = interactionObject;
        _transform = interactionObject.transform;
        _initialPosition = initialPosition;
        _finalPosition = finalPosition;
    }

    public void UpdateDrag()
    {
        if (!_isActiveDrag) return;

        // Calculate position in front of camera
        Vector3 targetPosition = _playerCamera.position + _playerCamera.forward * _dragDistance;
        targetPosition.y = _yPosition;

        Vector3 nextPosition = Vector3.Lerp(_transform.position, targetPosition, _dragSpeed * Time.unscaledDeltaTime);


        // Move object to next position if less than the max distance away from the initial position
        if (Mathf.Abs(Vector3.Distance(_initialPosition, nextPosition)) <= _maxDragDistance)
        {
            _transform.position = nextPosition;
        }
    }

    public void FinalizeDrag()
    {
        _isActiveDrag = false;
        _finalPosition = _transform.position;
    }

    public void CancelDrag()
    {
        _isActiveDrag = false;
        _transform.position = _initialPosition;
    }

    public override void Execute()
    {
        _transform.position = _finalPosition;
    }

    public override void Undo()
    {
        // Revert the object to its initial position before the drag
        _transform.position = _initialPosition;

        // Mark the interaction object as not having taken an action
        ActionObject.HasTakenAction = false;
    }
}