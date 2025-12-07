using System.Collections;
using UnityEngine;

public class Missile : MonoBehaviour, IPausable
{
    [Header("Missile Settings")]
    // Initial speed at which the missile travels.
    [SerializeField] private float _missileSpeed = 20f;

    [Header("Missile Spawn Point")]
    [SerializeField] private Transform _spawnPointTransform;

    [Header("Components")]
    // Reference components.
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Explosion _explosionScript;
    [SerializeField] private ParticleSystem _trail;

    // Prevent multiple explosions.
    private bool _canExplode = false;

    // Saved velocity.
    private Vector3 _savedVelocity;

    // Position before unpausing.
    private Vector3 _pausedPosition;

    // Initial position at spawn time.
    private Vector3 _initialPosition;

    public void Awake()
    {
        _canExplode = false;
        _capsuleCollider.isTrigger = true;

        // Set the missile's initial velocity.
        _rb.linearVelocity = transform.forward * _missileSpeed;

        _initialPosition = transform.position;
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
        _trail.Stop();

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
        _explosionScript.ResetExplosion();

        transform.position = _pausedPosition;
    }

    public void SimulatePrePauseBehaviour(float simulationDuration)
    {
        // If no spawn point, do nothing.
        if (_spawnPointTransform == null)
        {
            return;
        }

        // Start the simulation halfway between the spawn point and the initial position.
        float spawnToInitialDistance = Vector3.Distance(_spawnPointTransform.position, _initialPosition);
        transform.position = _spawnPointTransform.position + (transform.forward * (spawnToInitialDistance / 2f));

        StartCoroutine(SimulateMissileMovement(transform.position, _initialPosition, simulationDuration));
    }

    private IEnumerator SimulateMissileMovement(Vector3 startPoint, Vector3 endPoint, float simulationDuration)
    {
        // Calculate speed needed to reach end point in the given duration.
        float speed = Vector3.Distance(startPoint, endPoint) / simulationDuration;
        float elapsedTime = 0f;

        // Move the bullet towards the end point over the simulation duration.
        while (elapsedTime < simulationDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint, speed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final position is set accurately.
        transform.position = endPoint;
    }
}
