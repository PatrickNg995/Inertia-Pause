using UnityEngine;

// Get Rigidbody, then remove & add velocity depending
// on game state
public class PausableRigidbody : MonoBehaviour, IPausable
{
    [SerializeField] private Rigidbody _rb;
    private Vector3 _savedVelocity;

    private Vector3 _pausedPosition;
    private Quaternion _pausedRotation;

    // Save velocity & stop movement
    public void Pause()
    {
        _rb.isKinematic = true;
        _savedVelocity = _rb.linearVelocity;
    }

    // Start movement & add back saved velocity
    public void Unpause()
    {
        _pausedPosition = transform.position;
        _pausedRotation = transform.rotation;   

        _rb.isKinematic = false;
        _rb.linearVelocity = _savedVelocity;
    }

    public void ResetStateBeforeUnpause()
    {
        // Reset position and rotation to pre-unpause state.
        transform.SetPositionAndRotation(_pausedPosition, _pausedRotation);
    }
}
