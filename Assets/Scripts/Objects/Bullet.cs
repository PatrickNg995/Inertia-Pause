using UnityEngine;

public class Bullet : MonoBehaviour, IPausable
{
    [Header("Bullet Settings")]
    // Whether this bullet can pierce through NPCs.
    [SerializeField] private bool _isPiercing = false;

    // Speed at which the bullet travels.
    [SerializeField] private float _bulletSpeed = 20f;

    // Force applied to NPCs on hit.
    private const float HIT_FORCE = 15f;
    private const float UPWARD_FACTOR = 0.4f;

    // Reference rigidbody.
    [SerializeField] private Rigidbody _rb;
    private bool _canKill = true;

    // Saved velocity.
    private Vector3 _savedVelocity;

    public void Awake()
    {
        // Set the bullet's velocity to be in the forward direction.
        _rb.linearVelocity = transform.forward * _bulletSpeed;
    }

    // Handle collisions with other objects.
    public void OnTriggerEnter(Collider other)
    {

        if (!_canKill) return;

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

        // Destroy the bullet on impact with anything if it isn't piercing.
        if (!_isPiercing)
        {
            Destroy(gameObject);
        }
    }

    // Apply hit to NPC.
    public void HitNPC(NPC npc, Collider collider)
    {
        if (npc != null && _canKill)
        {
            // Make the impact direction the forward direction of the bullet parent, plus a bit of upward force.
            Vector3 impactDir = transform.forward + Vector3.up * UPWARD_FACTOR;

            // Use closest point on collider as approximate hit point.
            Vector3 hitPoint = collider.ClosestPoint(transform.position);
            npc.ApplyHit(impactDir * HIT_FORCE, hitPoint);
        }
    }

    // Disable collider on pause.
    public void Pause()
    {
        _canKill = false;

        if (_rb.linearVelocity == Vector3.zero)
        {
            _savedVelocity = transform.forward * _bulletSpeed;
        }

        else
        {
            _savedVelocity = _rb.linearVelocity;
        }

        _rb.isKinematic = true;
    }

    // Enable collider on unpause.
    public void Unpause()
    {
        _canKill = true;

        _rb.isKinematic = false;
        _rb.linearVelocity = _savedVelocity;
    }
}
