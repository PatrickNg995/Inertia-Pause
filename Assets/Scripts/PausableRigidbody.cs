using UnityEngine;

// Get Rigidbody, then remove & add velocity depending
// on game state
public class PausableRigidbody : MonoBehaviour, IPausable
{
    private Rigidbody rb;
    public Vector3 savedVelocity;

    void Start()
    {
        // Get rigidbody
        rb = GetComponent<Rigidbody>();
    }

    // Save velocity & stop movement
    public void Pause()
    {
        savedVelocity = rb.linearVelocity;
        rb.isKinematic = true;
    }

    // Start movement & add back saved velocity
    public void Unpause()
    {
        rb.isKinematic = false;
        rb.linearVelocity = savedVelocity;
    }
}
