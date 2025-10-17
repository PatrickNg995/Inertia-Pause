using UnityEngine;

public class NPC : MonoBehaviour, IPausable
{
    [Header("NPC Look Target")]
    // The object the NPC should be facing.
    [SerializeField] private GameObject LookTarget;

    // Distance in meters needed to fall to die.
    private const float LETHAL_FALL_THRESHOLD = 3f;

    // Force applied when hitting the ground from a fall to simulate impact.
    private const float FALL_HIT_FORCE = 15f;

    // Initial position to determine fall distance.
    private Vector3 _initialPosition;

    // Reference to the Rigidbody.
    private Rigidbody _rb;

    // Whether the NPC is alive or dead.
    public bool IsAlive { get; private set; } = true;

    private void Start()
    {
        // Record initial position for determining fall damage.
        _initialPosition = transform.position;

        _rb = GetComponent<Rigidbody>();
        GetComponent<IPausable>().AddToTimePause(this);

        // Start with ragdoll physics disabled.
        GetComponent<Animator>().enabled = true;
        SetRigidbodyState(true);
        SetColliderState(false);

        // If there is a look target, make NPC face it.
        if (LookTarget != null)
        {
            transform.LookAt(LookTarget.transform);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float fallDistance = _initialPosition.y - transform.position.y;

        // Check for lethal collisions.
        if (collision.gameObject.CompareTag("Lethal"))
        {
            Die();
        }

        // Check if NPC fell a lethal distance.
        if (fallDistance > LETHAL_FALL_THRESHOLD)
        {
            // Apply downward hit force to simulate impact.
            ApplyHit(Vector3.down * FALL_HIT_FORCE, transform.position);
        }
    }

    public void Pause()
    {
        // Make rigidbody kinematic to pause physics.
        _rb.isKinematic = true;
    }

    public void Unpause()
    {
        // Restore rigidbody physics.
        _rb.isKinematic = false;
    }

    public void Die()
    {
        // Disable animator, enable ragdoll physics.
        GetComponent<Animator>().enabled = false;
        SetRigidbodyState(false);
        SetColliderState(true);

        IsAlive = false;
    }

    public void ApplyHit(Vector3 impulse, Vector3 hitPoint)
    {
        // Enable ragdoll
        Die();

        // Apply impulse to each child rigidbody at the hit point so the ragdoll reacts naturally.
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb != null && !rb.isKinematic)
            {
                rb.AddForceAtPosition(impulse, hitPoint, ForceMode.Impulse);
            }
        }

        // Also apply to root rigidbody if present and non-kinematic.
        Rigidbody rootRb = GetComponent<Rigidbody>();
        if (rootRb != null && !rootRb.isKinematic)
        {
            rootRb.AddForce(impulse, ForceMode.Impulse);
        }
    }

    private void SetRigidbodyState(bool state)
    {

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }
        GetComponent<Rigidbody>().isKinematic = !state;
    }

    private void SetColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }
        GetComponent<Collider>().enabled = !state;
    }
}
