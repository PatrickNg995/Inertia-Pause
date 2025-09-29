using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Whether this bullet can pierce through NPCs
    public bool isPiercing = false;

    // Speed at which the bullet travels
    public float bulletSpeed = 10f; // NOTE this has been slowed for easier observation testing

    // reference time pause script, rigidbody & collider
    private TimePauseUnpause timePauseScript;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        // Disable collisions if this is a piercing bullet
        if (isPiercing)
        {
            GetComponent<CapsuleCollider>().isTrigger = true;
        }

        // Set the bullet's velocity to be in the forward direction of its parent
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.parent.forward * bulletSpeed;

        // Get collider
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Connect to pause & unpause functions
        timePauseScript = GameObject.FindGameObjectWithTag("TimePause").GetComponent<TimePauseUnpause>();
        timePauseScript.pauseTime.AddListener(pauseBullet);
        timePauseScript.unpauseTime.AddListener(unpauseBullet);
    }

    // This should only be called if this is a piercing bullet, destroy any NPC it hits
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NPC")
        {
            Destroy(other.gameObject);
        }
    }

    // This should only be called if this isn't a piercing bullet, destroy bullet on impact
    public void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
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
