using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Grenade : MonoBehaviour, IPausable
{
    [Header("Grenade Settings")]
    // Initial speed at which the grenade travels.
    [SerializeField] private float _grenadeInitialSpeed = 10f;

    // Time before grenade explodes.
    [SerializeField] private float _grenadeExplosionDelay = 1.5f;

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

    public void Awake()
    {
        // Stop explosions immediately.
        _canExplode = false;

        // Set the grenade's initial velocity to be in the forward direction.
        _rb.linearVelocity = transform.forward * _grenadeInitialSpeed;
    }

    void Update()
    {
        // Explode grenade after countdown after unpausing.
        if (_canExplode)
        {
            _grenadeExplosionDelay -= Time.deltaTime;
            if (_grenadeExplosionDelay < 0)
            {
                _canExplode = false;
                _sphereCollider.enabled = false;
                _meshRenderer.enabled = false;
                Destroy(_rb);
                _explosionScript.StartExplosion();
            }
        }
    }

    // Stop timer countdown on pause.
    public void Pause()
    {
        _canExplode = false;

        if (_rb.linearVelocity == Vector3.zero)
        {
            _savedVelocity = transform.forward * _grenadeInitialSpeed;
        }
        else
        {
            _savedVelocity = _rb.linearVelocity;
        }

        _rb.isKinematic = true;
        _sphereCollider.isTrigger = true;
    }

    // Start grenade explosion countdown on unpause.
    public void Unpause()
    {
        _canExplode = true;

        _rb.isKinematic = false;
        _sphereCollider.isTrigger = false;

        _rb.linearVelocity = _savedVelocity;
    }
}
