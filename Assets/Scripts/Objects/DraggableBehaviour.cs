using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableBehaviour : InteractionObject
{
    [Header("Component References")]
    [SerializeField] private Collider _collider;
    [SerializeField] private DragBoundary _boundary;
    [SerializeField] private DragIndicator _indicator;

    [Header("Drag Settings")]
    [SerializeField] private float _dragSpeed = 10f;
    [SerializeField] private float _maxDragDistance = 1f;
    [SerializeField] private float _maxDistanceFromCamera = 2f;
    [SerializeField] private float _minDistanceFromCamera = 1f;

    private Transform _playerCamera;
    private Vector3 _resetPosition;
    private Vector3 _moveStartPosition;
    private bool _dragging;
    private float _dragDistance;
    private float _yPosition;

    private float _capsuleCheckRadius;
    private float _capsuleEndpointOffset;

    private PlayerActions _inputActions;
    private InputAction _scroll;

    private void Awake()
    {
        _inputActions = new PlayerActions();

        IsContinuousUpdate = true;

        _playerCamera = Camera.main.transform;
        _resetPosition = transform.position;

        if (_collider != null)
        {
            // Precompute capsule parameters for collision checking.
            Vector3 colliderExtents = _collider.bounds.extents;
            float halfHeight = colliderExtents.y;

            _capsuleCheckRadius = Mathf.Max(colliderExtents.x, colliderExtents.z);
            _capsuleEndpointOffset = Mathf.Max(0f, halfHeight - _capsuleCheckRadius);
        }
    }
    private void OnEnable()
    {
        _scroll = _inputActions.Ingame.Scroll;
        _scroll.performed += OnScroll;
        _scroll.Enable();
    }

    private void OnDisable()
    {
        _scroll.performed -= OnScroll;
        _scroll.Disable();
    }

    private void OnScroll(InputAction.CallbackContext context)
    {
        _dragDistance = Math.Clamp(_dragDistance + context.ReadValue<Vector2>().y,
                                   _minDistanceFromCamera, _maxDistanceFromCamera);
    }

    public override void OnStartInteract()
    {
        if (_dragging == true)
        {
            return;
        }

        // Disable time unpause while dragging.
        GameManager.Instance.DisableTimeUnpause();

        _dragging = true;
        _dragDistance = Vector3.Distance(_playerCamera.position, transform.position);
        _dragDistance = Math.Clamp(_dragDistance, _minDistanceFromCamera, _maxDistanceFromCamera);

        _moveStartPosition = transform.position;
        _yPosition = transform.position.y;

        if (_indicator != null)
        {
            _indicator.Enable();
            _indicator.DrawLine();
        }
        _boundary.ShowCircle(true);
    }

    public override void OnHoldInteract()
    {
        if (!_dragging)
        {
            return;
        }

        // Calculate position in front of camera.
        Vector3 targetPosition = _playerCamera.position + _playerCamera.forward * _dragDistance;
        targetPosition.y = _yPosition;

        Vector3 nextPosition = Vector3.Lerp(transform.position, targetPosition, _dragSpeed * Time.unscaledDeltaTime);

        // Move object to next position if it doesn't collide with anything and move is less than the
        // max distance away from the initial position.
        if (CanMoveTo(nextPosition) && Mathf.Abs(Vector3.Distance(_resetPosition, nextPosition)) <= _maxDragDistance)
        {
            transform.position = nextPosition;
        }
        // If the object was blocked from moving to the next position, check if it can be moved directly to the target position
        // to prevent snagging when dragging past an object.
        else if (CanMoveTo(targetPosition) && Mathf.Abs(Vector3.Distance(_resetPosition, targetPosition)) <= _maxDragDistance)
        {
            transform.position = targetPosition;
        }

        if (_indicator != null)
        {
            _indicator.DrawLine();
        }
    }

    public override void OnEndInteract()
    {
        if (!_dragging) 
        { 
            return; 
        }

        // Re-enable time unpause after dragging.
        GameManager.Instance.EnableTimeUnpause();

        // Record and execute the command.
        ActionCommand = new DragCommand(this, _moveStartPosition, transform.position, !HasTakenAction);
        GameManager.Instance.RecordAndExecuteCommand(ActionCommand);
        
        HasTakenAction = true;
        _dragging = false;
        if (_indicator != null) 
        { 
            _indicator.Disable(); 
        }
        _boundary.ShowCircle(false);
    }

    public override void OnCancelInteract()
    {
        if (!_dragging)
        {
            return;
        }

        _dragging = false;

        transform.position = _moveStartPosition;
        
        if (_indicator != null) 
        { 
            _indicator.Disable(); 
        }
        
        _boundary.ShowCircle(false);
    }

    public override void OnResetInteract()
    {
        if (_resetPosition == transform.position) 
        { 
            return; 
        }
        // Command added sets the position the current, before the above assignment, on execute, and to _initialPosition on undo.
        ActionCommand = new DragCommand(this, _resetPosition, transform.position, true);
        GameManager.Instance.ResetObjectCommands(this, ActionCommand);
        
        transform.position = _resetPosition;
        
        HasTakenAction = false;
        
        if (_indicator != null) 
        { 
            _indicator.Disable(); 
        }
        
        _boundary.ShowCircle(false);
    }

    private bool CanMoveTo(Vector3 targetPos)
    {
        if (_collider == null)
        {
            return true;
        }
            
        // Compute the world-space center for the collider at the target position.
        Vector3 localCenterOffset = _collider.bounds.center - transform.position;
        Vector3 center = targetPos + localCenterOffset;

        // Check for collisions at the target position with a capsule overlap.
        Vector3 pointA = center + transform.up * _capsuleEndpointOffset;
        Vector3 pointB = center - transform.up * _capsuleEndpointOffset;
        Collider[] hits = Physics.OverlapCapsule(pointA, pointB, _capsuleCheckRadius);

        if (hits.Length > 0)
        {
            foreach (Collider hit in hits)
            {
                // Ignore self-collision.
                if (hit == _collider)
                {
                    continue;
                }
                // Ignore collisions with mines.
                if (hit.transform.root.CompareTag("Mines"))
                {
                    continue;
                }
                // Collision detected with another object.
                return false;
            }
        }
        return true;
    }
}
