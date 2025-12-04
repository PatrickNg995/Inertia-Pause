using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShelfBehaviour : InteractionObject, IPausable
{
    [SerializeField] private float _torque = 2000f;
    // the amount of time to let the shelf to rotate while paused, so the player can see what will happen
    [SerializeField] private float _timeToTilt = 0.1f;

    private Rigidbody _rb;
    private Vector3 _pausedPosition;
    private Quaternion _pausedRotation;
    private bool _paused = false;
    private Vector3 _rotationalVelocity;
    private Vector3 _velocity;
    private float _timeSincePause;
    private bool _isToppled;

    private void Start()
    {
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        if (_paused && HasTakenAction && _timeSincePause >= 0)
        {
            _timeSincePause += Time.deltaTime;
        }

        if (_paused && _timeSincePause >= _timeToTilt)
        {
            if (_rotationalVelocity == Vector3.zero || _velocity == Vector3.zero)
            {
                _rotationalVelocity = _rb.angularVelocity;
                _velocity = _rb.linearVelocity;
            }

            _rb.isKinematic = true;
            // for the guard position to not modify the velocities
            _timeSincePause = -1;
        }
    }

    public override void OnCancelInteract()
    {
        // this should only be thrown due to a logic error in PlayerInteract
        throw new System.NotImplementedException();
    }

    public override bool OnStartInteract()
    {
        if (HasTakenAction)
        {
            return false;
        }

        // Set up and execute the topple command, in the right direction.
        ActionCommand = new ToppleCommand(this, _rb, transform.right, _torque);
        _rb.isKinematic = false;
        GameManager.Instance.RecordAndExecuteCommand(ActionCommand);
        return true;
    }

    public override void OnHoldInteract()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEndInteract()
    {
        throw new System.NotImplementedException();
    }

    public override void OnResetInteract()
    {
        if (!HasTakenAction) return;

        GameManager.Instance.UndoSpecificCommand(ActionCommand);
        _timeSincePause = 0f;
    }

    public override void OnCommandRedo()
    {
        _isToppled = true;
    }

    public override void OnCommandUndo()
    {
        _timeSincePause = 0f;
        _isToppled = false;
    }

    public void Pause()
    {
        // necessity found empirically
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
        }

        _rb.isKinematic = true;

        _timeSincePause = 0f;

        _paused = true;
    }

    public void Unpause()
    {
        _pausedPosition = transform.position;
        _pausedRotation = transform.rotation;

        _rb.isKinematic = false;

        if (_isToppled)
        {
            _rb.linearVelocity = _velocity;
            _rb.angularVelocity = _rotationalVelocity;
        }

        _paused = false;
    }

    public void ResetStateBeforeUnpause()
    {
        // Reset position and rotation to pre-unpause state.
        transform.SetPositionAndRotation(_pausedPosition, _pausedRotation);
    }
}
