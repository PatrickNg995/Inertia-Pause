using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Collider to hit other objects.
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField] private ParticleSystem _ps;

    // Force applied to NPCs on hit.
    private const float HIT_FORCE = 15f;
    private const float UPWARD_FACTOR = 0.4f;

    public void Awake()
    {
        _sphereCollider.enabled = false;
    }

    public void StartExplosion()
    {
        // Enable collider & play particle system effect.
        _sphereCollider.enabled = true;
        _ps.Play();
    }

    public void ResetExplosion()
    {
        // Disable collider & stop particle system effect.
        _sphereCollider.enabled = false;
        _ps.Stop();
    }

    public void OnTriggerEnter(Collider other)
    {
        // If it was an NPC, apply hit.
        if (other.CompareTag("Ally") || other.CompareTag("Enemy"))
        {
            // Only hit if NPC is alive.
            NPC npc = other.GetComponentInParent<NPC>();
            if (npc.IsAlive)
            {
                HitNPC(npc, other);
            }
        }
    }
    public void HitNPC(NPC npc, Collider collider)
    {
        if (npc != null)
        {
            // Send enemy away from the middle of the explosion, plus a bit of upward force.
            Vector3 directionVector = collider.transform.position - transform.position;
            Vector3 normalizedVector = directionVector.normalized;
            Vector3 impactDir = normalizedVector + Vector3.up * UPWARD_FACTOR;

            // Use closest point on collider as approximate hit point.
            Vector3 hitPoint = collider.ClosestPoint(transform.position);
            npc.ApplyHit(impactDir * HIT_FORCE, hitPoint);
        }
    }
}
