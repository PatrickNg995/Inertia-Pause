using System;
using UnityEngine;

public class NPC : MonoBehaviour
{
    // The velocity required to kill the NPC on impact
    public float VelocityRequiredToKill = 10f; 

    // Check if something collides with the NPC at a high enough velocity to kill it
    public void OnCollisionEnter(Collision collision)
    {
        // Debug for collision detection and velocity
        Debug.Log("Collision with " + collision.gameObject.name + "; velocity: " + collision.relativeVelocity.magnitude 
            + ", " + VelocityRequiredToKill + " needed to kill");

        // If the collisions had sufficient velocity, destroy the NPC
        if (collision.relativeVelocity.magnitude >= VelocityRequiredToKill)
        {
            Destroy(gameObject);
        }
    }
}
