using UnityEngine;

public class Grenade : MonoBehaviour, IPausable
{
    [Header("Grenade Settings")]
    // Initial speed at which the grenade travels.
    [SerializeField] private float _grenadeInitialSpeed = 10f;

    // Time before grenade explodes.
    [SerializeField] private float _grenadeInitialExplosionDelay = 1.5f;
    private float _grenadeCurrentExplosionDelay;

    [Header("Components")]
    // Reference components.
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Explosion _explosionScript;

    // Check if timer can start counting down before exploding grenade.
    private bool _canExplode;

    // Saved velocity.
    private Vector3 _savedVelocity;

    // Position and rotation before unpausing.
    private Vector3 _pausedPosition;
    private Quaternion _pausedRotation;

    public void Awake()
    {
        // Stop explosions immediately.
        _canExplode = false;

        // Set the grenade's initial velocity to be in the forward direction.
        _rb.linearVelocity = transform.forward * _grenadeInitialSpeed;

        _grenadeCurrentExplosionDelay = _grenadeInitialExplosionDelay;
    }

    void Update()
    {
        // Explode grenade after countdown after unpausing.
        if (_canExplode)
        {
            _grenadeCurrentExplosionDelay -= Time.deltaTime;
            if (_grenadeCurrentExplosionDelay < 0)
            {
                _canExplode = false;
                _sphereCollider.enabled = false; // Stop collisions.
                _meshRenderer.enabled = false; // Remove visuals.
                _rb.isKinematic = true; // Stop movement.
                _explosionScript.StartExplosion();
            }
        }
    }

    // Stop timer countdown on pause.
    public void Pause()
    {
        _canExplode = false;

        _savedVelocity = transform.forward * _grenadeInitialSpeed;

        _rb.isKinematic = true;
        _sphereCollider.isTrigger = true;

        _grenadeCurrentExplosionDelay = _grenadeInitialExplosionDelay;
    }

    // Start grenade explosion countdown on unpause.
    public void Unpause()
    {
        _pausedPosition = transform.position;
        _pausedRotation = transform.rotation;

        _canExplode = true;

        _rb.isKinematic = false;
        _sphereCollider.isTrigger = false;

        _rb.linearVelocity = _savedVelocity;
    }

    public void ResetStateBeforeUnpause()
    {
        // Reset the grenade.
        _sphereCollider.enabled = true;
        _meshRenderer.enabled = true;
        _grenadeCurrentExplosionDelay = _grenadeInitialExplosionDelay;
        _explosionScript.ResetExplosion();

        transform.SetPositionAndRotation(_pausedPosition, _pausedRotation);
    }
}
