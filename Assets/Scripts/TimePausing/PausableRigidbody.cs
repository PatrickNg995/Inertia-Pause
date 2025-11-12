using UnityEngine;

public class PausableRigidbody : MonoBehaviour, IPausable
{
    [SerializeField] private Rigidbody _rb;

    private Vector3 _savedVelocity;
    private Vector3 _savedAngularVelocity;
    private bool _originalIsKinematic;

    private void Awake()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody>();

        _originalIsKinematic = _rb.isKinematic;
    }

    // Pause: store velocity and make kinematic ONLY if it was dynamic
    public void Pause()
    {
        if (_rb == null) return;

        if (!_rb.isKinematic)
        {
            _savedVelocity = _rb.linearVelocity;
            _savedAngularVelocity = _rb.angularVelocity;
            _rb.isKinematic = true;
        }
    }

    // Unpause: restore original mode; if it was dynamic, restore velocity
    public void Unpause()
    {
        if (_rb == null) return;

        // Go back to whatever this body was originally
        _rb.isKinematic = _originalIsKinematic;

        if (!_rb.isKinematic)
        {
            _rb.linearVelocity = _savedVelocity;
            _rb.angularVelocity = _savedAngularVelocity;
        }
    }

    // Only needed if you really want to snap back a dynamic body
    public void ResetStateBeforeUnpause(Vector3 position, Quaternion rotation)
    {
        if (_rb == null) return;
        _rb.position = position;
        _rb.rotation = rotation;
    }

    public void ResetStateBeforeUnpause()
    {
        throw new System.NotImplementedException();
    }
}
