using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShelfBehaviour : InteractionObject, IPausable
{
    [SerializeField] private float _torque = 2000f;
    // the amount of time to let the shelf to rotate while paused, so the player can see what will happen
    [SerializeField] private float _timeToTilt = 0.1f;

    private Rigidbody _rb;
    private bool _paused = false;
    private Vector3 _rotationalVelocity;
    private Vector3 _velocity;
    private float _timeSincePause;

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
            _rotationalVelocity = _rb.angularVelocity;
            _velocity = _rb.linearVelocity;

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

    public override void OnStartInteract()
    {
        if (HasTakenAction) { return; }

        // Set up and execute the topple command, in the right direction.
        ActionCommand = new ToppleCommand(this, _rb, transform.right, _torque);
        GameManager.Instance.RecordAndExecuteCommand(ActionCommand);
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
        _rb.isKinematic = false;
    }

    public void Pause()
    {
        // necessity found empirically
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
        }

        _timeSincePause = 0f;

        _paused = true;
    }

    public void Unpause()
    {
        _rb.isKinematic = false;

        _rb.linearVelocity = _velocity;
        _rb.angularVelocity = _rotationalVelocity;

        _paused = false;
    }
}
