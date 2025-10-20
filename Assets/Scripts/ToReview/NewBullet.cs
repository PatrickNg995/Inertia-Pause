using UnityEngine;

public class NewBullet : MonoBehaviour, IPausable
{
    // Whether this bullet can pierce through NPCs.
    public bool IsPiercing = false;

    // Speed at which the bullet travels.
    public float BulletSpeed = 10f; // NOTE this has been slowed for easier observation testing.

    // Force applied to NPCs on hit.
    private float _hitForce = 15f;
    private float _upwardFactor = 0.4f;

    // reference rigidbody & collider.
    private Rigidbody _rb;
    private CapsuleCollider _capsuleCollider;

    // save velocity.
    private Vector3 _savedVelocity;

    public void Awake()
    {
        // Set the bullet's velocity to be in the forward direction of its parent.
        _rb = GetComponent<Rigidbody>();
        _rb.linearVelocity = transform.parent.forward * BulletSpeed;

        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public void Start()
    {
        // Make sure bullet is separated from any parents.
        if (transform.parent.parent != null)
        {
            transform.parent.parent = null;
        }
    }

    // Handle collisions with other objects.
    public void OnTriggerEnter(Collider other)
    {
        // Get root object of whatever was hit.
        GameObject rootObject = other.transform.root.gameObject;

        // If it was an NPC, apply hit.
        if (rootObject.CompareTag("Ally") || rootObject.CompareTag("Enemy"))
        {
            // Only hit if NPC is alive, prevents repeated hits with piercing bullets.
            NPC npc = rootObject.GetComponentInParent<NPC>();
            if (npc.IsAlive)
            {
                HitNPC(npc, other);
            }
        }

        // Destroy the bullet on impact with anything if it isn't piercing.
        if (!IsPiercing)
        {
            Destroy(gameObject);
        }
    }

    // Apply hit to NPC.
    public void HitNPC(NPC npc, Collider collider)
    {
        if (npc != null)
        {
            // Make the impact direction the forward direction of the bullet parent, plus a bit of upward force.
            Vector3 impactDir = transform.parent.forward + Vector3.up * _upwardFactor;

            // Use closest point on collider as approximate hit point.
            Vector3 hitPoint = collider.ClosestPoint(transform.position);
            npc.ApplyHit(impactDir * _hitForce, hitPoint);
        }
    }

    // Disable collider on pause.
    public void Pause()
    {
        if (_rb.linearVelocity == Vector3.zero)
        {
            _savedVelocity = transform.parent.forward * BulletSpeed;
        }

        else
        {
            _savedVelocity = _rb.linearVelocity;
        }

        _rb.isKinematic = true;
        _capsuleCollider.enabled = false;
    }

    // Enable collider on unpause.
    public void Unpause()
    {
        _capsuleCollider.enabled = true;
        _rb.isKinematic = false;
        _rb.linearVelocity = _savedVelocity;
    }

}
