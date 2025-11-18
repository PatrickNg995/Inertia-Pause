using TMPro;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Missile : MonoBehaviour, IPausable
{
    [Header("Missile Settings")]
    // Initial speed at which the missile travels.
    [SerializeField] private float _missileSpeed = 20f;

    // Prevent multiple explosions.
    private bool _canExplode = false; 

    [Header("Components")]
    // Reference components.
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Explosion _explosionScript;
    [SerializeField] private ParticleSystem _trail;

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

        // Otherwise, explode.
        _canExplode = false;

        // Stop collisions.
        _capsuleCollider.enabled = false;

        // Remove visuals.
        _meshRenderer.enabled = false;
        _trail.gameObject.SetActive(false);

        // Stop movement.
        _rb.isKinematic = true;

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
        _trail.gameObject.SetActive(true);
        _explosionScript.ResetExplosion();

        transform.position = _pausedPosition;
    }
}
