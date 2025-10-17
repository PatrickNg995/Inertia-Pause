using UnityEngine;

public class Bullet : MonoBehaviour, IPausable
{
    // Whether this bullet can pierce through NPCs
    [SerializeField] private bool IsPiercing = false;

    // Speed at which the bullet travels
    // (NOTE this has been slowed for easier observation testing)
    [SerializeField] private float BulletSpeed = 10f;

    // Force applied to NPCs on hit
    private float _hitForce = 15f;
    private float _upwardFactor = 0.4f;

    // reference time pause script, rigidbody & collider
    private TimePauseUnpause timePauseScript;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private void Start()
    {
        // Set the bullet's velocity to be in the forward direction of its parent
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.parent.forward * BulletSpeed;

        capsuleCollider = GetComponent<CapsuleCollider>();

        GetComponent<IPausable>().AddToTimePause(this);
    }

    // Handle collisions with other objects
    public void OnTriggerEnter(Collider other)
    {
        // Get root object of whatever was hit
        GameObject rootObject = other.transform.root.gameObject;

        // If it was an NPC, apply hit
        if (rootObject.CompareTag("Ally") || rootObject.CompareTag("Enemy"))
        {
            // Only hit if NPC is alive, prevents repeated hits with piercing bullets
            NPC npc = rootObject.GetComponentInParent<NPC>();
            if (npc.IsAlive)
            {
                HitNPC(npc, other);
            }    
        }

        // Destroy the bullet on impact with anything if it isn't piercing
        if (!IsPiercing)
        {
            Destroy(gameObject);
        }
    }

    // Apply hit to NPC
    public void HitNPC(NPC npc, Collider collider)
    {
        if (npc != null)
        {
            // Make the impact direction the forward direction of the bullet parent, plus a bit of upward force
            Vector3 impactDir = transform.parent.forward + Vector3.up * _upwardFactor;

            // Use closest point on collider as approximate hit point
            Vector3 hitPoint = collider.ClosestPoint(transform.position);
            npc.ApplyHit(impactDir * _hitForce, hitPoint);
        }
    }

    // Disable collider on pause
    public void Pause()
    {
        capsuleCollider.enabled = false;
    }

    // Enable collider on unpause
    public void Unpause()
    {
        capsuleCollider.enabled = true;
    }

}
