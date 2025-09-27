using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Whether this bullet can pierce through NPCs
    public bool isPiercing = false;

    // Speed at which the bullet travels
    public float bulletSpeed = 10f; // NOTE this has been slowed for easier observation testing

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        // Disable collisions if this is a piercing bullet
        if (isPiercing)
        {
            GetComponent<CapsuleCollider>().isTrigger = true;
        }

        // Set the bullet's velocity to be in the forward direction of its parent
        GetComponent<Rigidbody>().linearVelocity = transform.parent.forward * bulletSpeed;
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
}
