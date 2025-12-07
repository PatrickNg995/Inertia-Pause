using System.Collections;
using System.Net;
using TMPro;
using UnityEngine;

public class Bullet : MonoBehaviour, IPausable
{
    [Header("References")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private Transform _spawnPointTransform;

    [Header("Impact Effect Prefab")]
    [SerializeField] private GameObject _impactEffectPrefab;

    [Header("Bullet Settings")]
    [Tooltip("Whether this bullet can pierce through NPCs.")]
    [SerializeField] private bool _isPiercing = false;

    [Tooltip("Whether this bullet can pierce through anything, including boxes, shelves, etc.")]
    [SerializeField] private bool _isUnstoppable = false;

    [Tooltip("Speed at which the bullet travels.")]
    [SerializeField] private float _bulletSpeed = 20f;

    // Force applied to NPCs on hit.
    private const float HIT_FORCE = 10f;
    private const float UPWARD_FACTOR = 1f;

    // If the distance from spawn point to initial position is greater than this, use this distance as an offset from the initial position
    // for simulated prepause instead of starting at the spawn point.
    // If a bullet starts far away from the shooter, that shot was fired earlier and should be simualted further along its path.
    private const float SIMULATION_BREAKPOINT_DISTANCE = 6f;

    // Saved velocity.
    private Vector3 _savedVelocity;

    // Position before unpausing.
    private Vector3 _pausedPosition;

    // Initial position at spawn time.
    private Vector3 _initialPosition;

    // Used to toggle hit registration on bullets.
    private bool _isHitDetecting = true;

    public void Awake()
    {
        _initialPosition = transform.position;

        // Set the bullet's velocity to be in the forward direction.
        _rb.linearVelocity = transform.forward * _bulletSpeed;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!_isHitDetecting)
        {
            return;
        }

        // Create impact effect at closest point on bounds.
        Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
        OnImpactEffect(contactPoint);

        // If it was an NPC, apply hit.
        if (other.CompareTag("Ally") || other.CompareTag("Enemy"))
        {
            // Only hit if NPC is alive, prevents repeated hits with piercing bullets.
            NPC npc = other.GetComponentInParent<NPC>();
            if (npc.IsAlive)
            {
                HitNPC(npc, other);
            }
        }

        // For unstoppable bullets.
        if (_isUnstoppable)
        {
            // Never disable this bullet.
            return;
        }

        // Destroy the bullet on impact with anything if it isn't piercing, or if it hits an unpierceable object.
        if (!_isPiercing || other.CompareTag("Unpierceable"))
        {
            gameObject.SetActive(false);
        }

        // Check Extra Tag Component if object is unpierceable.
        if (other.TryGetComponent<ExtraTagComponent>(out var tags))
        {
            if (tags.HasTag("Unpierceable"))
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void HitNPC(NPC npc, Collider collider)
    {
        if (npc != null)
        {
            // Make the impact direction the forward direction of the bullet parent, plus a bit of upward force.
            Vector3 impactDir = transform.forward + Vector3.up * UPWARD_FACTOR;

            // Use closest point on collider as approximate hit point.
            Vector3 hitPoint = collider.ClosestPoint(transform.position);
            npc.ApplyHit(gameObject, impactDir * HIT_FORCE, hitPoint);
        }
    }

    public void Pause()
    {
        // Disable trail emission.
        _trailRenderer.emitting = false;

        // Disable hit detection.
        _isHitDetecting = false;

        // Save current velocity.
        if (_rb.linearVelocity == Vector3.zero)
        {
            _savedVelocity = transform.forward * _bulletSpeed;
        }
        else
        {
            _savedVelocity = _rb.linearVelocity;
        }

        // Make rigidbody kinematic to stop movement.
        _rb.isKinematic = true;
    }

    public void Unpause()
    {
        // Enable trail emission.
        _trailRenderer.emitting = true;

        // Enable hit detection.
        _isHitDetecting = true;

        // Save position before unpausing.
        _pausedPosition = transform.position;

        // Restore velocity.
        _rb.isKinematic = false;
        _rb.linearVelocity = _savedVelocity;
    }

    public void ResetStateBeforeUnpause()
    {
        // Reactivate the bullet.
        gameObject.SetActive(true);

        // Reset position to pre-unpause state.
        transform.position = _pausedPosition;

        // Clear the trail.
        _trailRenderer.Clear();
    }

    private void OnImpactEffect(Vector3 locationOnImmpact)
    {
        // Create impact effect with a rotation that faces outward from the surface.
        Quaternion impactRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
        Instantiate(_impactEffectPrefab, locationOnImmpact, impactRotation);
    }

    public void SimulatePrePauseBehaviour(float simulationDuration)
    {
        // If no spawn point, do nothing.
        if (_spawnPointTransform == null)
        {
            return;
        }

        // If the distance from spawn point to initial position exceeds the breakpoint distance, start the simulation relative
        // to the initial position instead.
        float spawnToInitialDistance = Vector3.Distance(_spawnPointTransform.position, _initialPosition);
        if (spawnToInitialDistance < SIMULATION_BREAKPOINT_DISTANCE)
        {
            transform.position = _spawnPointTransform.position;
        }
        else
        {
            transform.position = _initialPosition - (transform.forward * SIMULATION_BREAKPOINT_DISTANCE);
        }

        // Clear the trail and start emitting.
        _trailRenderer.Clear();
        _trailRenderer.emitting = true;

        // Start simulating movement towards the initial position.
        StartCoroutine(SimulateBulletMovement(transform.position, _initialPosition, simulationDuration));

        // After simulation, stop emitting trail.
        _trailRenderer.emitting = false;
    }

    private IEnumerator SimulateBulletMovement(Vector3 startPoint, Vector3 endPoint, float simulationDuration)
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
