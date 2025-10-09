using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Whether this bullet can pierce through NPCs
    public bool isPiercing = false;

    // Speed at which the bullet travels
    public float bulletSpeed = 10f; // NOTE this has been slowed for easier observation testing

    // Force applied to NPCs on hit
    private float hitForce = 15f;
    private float upwardFactor = 0.4f;

    // reference time pause script, rigidbody & collider
    private TimePauseUnpause timePauseScript;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Disable collisions if this is a piercing bullet
        if (isPiercing)
        {
            GetComponent<CapsuleCollider>().isTrigger = true;
        }

        // Set the bullet's velocity to be in the forward direction of its parent
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.parent.forward * bulletSpeed;

        // Connect to pause & unpause functions
        timePauseScript = GameObject.FindGameObjectWithTag("TimePause").GetComponent<TimePauseUnpause>();
        timePauseScript.pauseTime.AddListener(pauseBullet);
        timePauseScript.unpauseTime.AddListener(unpauseBullet);
    }

    // Handle collisions with other objects
    public void OnTriggerEnter(Collider other)
    {
        // Get root object of whatever was hit
        GameObject rootObject = other.transform.root.gameObject;

        // If it was an NPC, apply hit
        if (rootObject.CompareTag("NPC"))
        {
            // Only hit if NPC is alive, prevents repeated hits with piercing bullets
            NPC npc = rootObject.GetComponentInParent<NPC>();
            if (npc.IsAlive)
            {
                HitNPC(npc, other);
            }    
        }

        // Destroy the bullet on impact with anything if it isn't piercing
        if (!isPiercing)
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
            Vector3 impactDir = transform.parent.forward + Vector3.up * upwardFactor;

            // Use closest point on collider as approximate hit point
            Vector3 hitPoint = collider.ClosestPoint(transform.position);
            npc.ApplyHit(impactDir * hitForce, hitPoint);
        }
    }

    // Disable collider on pause
    private void pauseBullet()
    {
        capsuleCollider.enabled = false;
    }

    // Enable collider on unpause
    private void unpauseBullet()
    {
        capsuleCollider.enabled = true;
    }

}
