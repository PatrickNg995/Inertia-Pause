using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShelfBehaviour : InteractionObject, IPausable
{
    [SerializeField] private float _torque = 2000f;
    [SerializeField] private Indicator _indicator;
    [SerializeField] private float _deltaForward;
    [SerializeField] private float _deltaUp;
    [SerializeField] private float _finalRotationScale = 3f;

    private Rigidbody _rb;
    private Vector3 _pausedPosition;
    private Quaternion _pausedRotation;
    private bool _isToppled;

    private void Start()
    {
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
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
        ActionCommand = new ToppleCommand(this, _rb, transform.right, _deltaForward, _deltaUp, _finalRotationScale);
        _rb.isKinematic = true;
        _isToppled = true;
        GameManager.Instance.RecordAndExecuteCommand(ActionCommand);
        _indicator.Draw();
        _indicator.Enable();
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
        if (!HasTakenAction)
        {
            return;
        }

        GameManager.Instance.UndoSpecificCommand(ActionCommand);
        _indicator.Disable();
    }

    public override void OnCommandRedo()
    {
        _isToppled = true;
    }

    public override void OnCommandUndo()
    {
        _isToppled = false;
    }

    public void Pause()
    {
        // Found empirically by necessity.
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
        }

        _rb.isKinematic = true;
    }

    public void Unpause()
    {
        _pausedPosition = transform.position;
        _pausedRotation = transform.rotation;

        _rb.isKinematic = false;

        Debug.Log($"{transform.gameObject.name}: is toppled? {_isToppled}");
        if (_isToppled)
        {
            _rb.AddTorque(_torque * transform.right);
        }
    }

    public void ResetStateBeforeUnpause()
    {
        // Reset position and rotation to pre-unpause state.
        transform.SetPositionAndRotation(_pausedPosition, _pausedRotation);
    }

    public override void OnHoverStart()
    {
        if (_isToppled)
        {
            _indicator.Draw();
            _indicator.Enable();
        }
    }

    public override void OnHoverEnd()
    {
        _indicator.Disable();
    }

    public void SimulatePrePauseBehaviour()
    {
        // No pre-pause behaviour to simulate.
    }
}
