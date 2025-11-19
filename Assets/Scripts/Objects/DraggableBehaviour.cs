using UnityEngine;

public class DraggableBehaviour : InteractionObject
{
    [SerializeField] private float _dragSpeed = 10f;
    [SerializeField] private float _maxDragDistance = 1f;
    [SerializeField] private DragBoundary _boundary;
    [SerializeField] private DragIndicator _indicator;

    private Transform _playerCamera;
    private Vector3 _resetPosition;
    private Vector3 _moveStartPosition;
    private bool _dragging;
    private float _dragDistance;
    private float _yPosition;

    private void Awake()
    {
        IsContinuousUpdate = true;

        _playerCamera = Camera.main.transform;
        _resetPosition = transform.position;
    }

    public override void OnStartInteract()
    {
        if (_dragging == true) return;

        _dragging = true;
        
        _dragDistance = Vector3.Distance(_playerCamera.position, transform.position);
        
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
        if (!_dragging) return;

        // Calculate position in front of camera
        Vector3 targetPosition = _playerCamera.position + _playerCamera.forward * _dragDistance;
        targetPosition.y = _yPosition;

        Vector3 nextPosition = Vector3.Lerp(transform.position, targetPosition, _dragSpeed * Time.unscaledDeltaTime);


        // Move object to next position if less than the max distance away from the initial position
        if (Mathf.Abs(Vector3.Distance(_resetPosition, nextPosition)) <= _maxDragDistance)
        {
            transform.position = nextPosition;
        }

        if (_indicator != null) _indicator.DrawLine();
    }

    public override void OnEndInteract()
    {
        if (!_dragging) return;


        // Record and execute the command
        ActionCommand = new DragCommand(this, _moveStartPosition, transform.position, !HasTakenAction);
        GameManager.Instance.RecordAndExecuteCommand(ActionCommand);
        
        HasTakenAction = true;
        _dragging = false;
        if (_indicator != null) _indicator.Disable();
        _boundary.ShowCircle(false);
    }

    public override void OnCancelInteract()
    {
        if (!_dragging) return;

        _dragging = false;

        transform.position = _moveStartPosition;
        if (_indicator != null) _indicator.Disable();
        _boundary.ShowCircle(false);
    }

    public override void OnResetInteract()
    {
        if (_resetPosition == transform.position) return;
        // command added sets the position the current, before the above assignment, on execute, and to _initialPosition on undo
        ActionCommand = new DragCommand(this, _resetPosition, transform.position, true);
        GameManager.Instance.ResetObjectCommands(this, ActionCommand);
        transform.position = _resetPosition;
        HasTakenAction = false;
        if (_indicator != null) _indicator.Disable();
        _boundary.ShowCircle(false);
    }
}