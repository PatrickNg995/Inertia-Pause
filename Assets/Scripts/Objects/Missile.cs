using UnityEngine;

public class Missile : MonoBehaviour, IPausable
{
    [Header("Missile Settings")]
    // Initial speed at which the missile travels.
    [SerializeField] private float _missileSpeed = 20f;

    private bool _canExplode = false; // Prevent multiple explosions.

    [Header("Components")]
    // Reference components.
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Explosion _explosionScript;

    // Saved velocity.
    private Vector3 _savedVelocity;

    // Position before unpausing.
    private Vector3 _pausedPosition;


    public void Awake()
    {
        _canExplode = false;
        _capsuleCollider.isTrigger = true;

        // Set the missile's initial velocity.
        _rb.linearVelocity = transform.forward * _missileSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Stop explosions if bool is not set.
        if (!_canExplode)
        {
            return;
        }

        // Otherwise, explode
        _canExplode = false;
        _capsuleCollider.enabled = false; // Stop collisions.
        _meshRenderer.enabled = false; // Remove visuals.
        _rb.isKinematic = true; // Stop movement.
        _explosionScript.StartExplosion();
    }

    // Stop movement & don't explode on pause.
    public void Pause()
    {
        _canExplode = false;

        if (_rb.linearVelocity == Vector3.zero)
        {
            _savedVelocity = transform.forward * _missileSpeed;
        }
        else
        {
            _savedVelocity = _rb.linearVelocity;
        }

        _rb.isKinematic = true;
        Debug.Log("Stopped");
    }

    // Start movement & can explode on unpause.
    public void Unpause()
    {
        _pausedPosition = transform.position;

        _canExplode = true;

        _rb.isKinematic = false;
        _rb.linearVelocity = _savedVelocity;
    }

    public void ResetStateBeforeUnpause()
    {
        // Reset the missile.
        _capsuleCollider.enabled = true;
        _meshRenderer.enabled = true;
        _explosionScript.ResetExplosion();

        transform.position = _pausedPosition;
    }
}
