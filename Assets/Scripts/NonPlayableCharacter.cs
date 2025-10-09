using System;
using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour
{
    // Whether the NPC is alive or dead
    private bool isAlive = true;

    public void Start()
    {
        // Start with ragdoll physics disabled
        GetComponent<Animator>().enabled = true;
        setRigidbodyState(true);
        //setColliderState(false);
    }

    // If the NPC collides with a lethal object, it dies
    public void OnCollisionEnter(Collision collision)
    {
        // TODO This is redundant with the new bullet hit detection, but leaving it in for now
        // This is okay for other generic objects but should be replaced by a new generic system for interactable objects
        if (collision.gameObject.tag == "Lethal")
        {
            Die();
        }
    }

    public void Die()
    {
        // Disable animator, enable ragdoll physics
        GetComponent<Animator>().enabled = false;
        setRigidbodyState(false);
        //setColliderState(true);

        isAlive = false;
    }

    // Apply an impulse to the ragdoll.
    public void ApplyHit(Vector3 impulse, Vector3 hitPoint)
    {
        // Enable ragdoll
        Die();

        // Apply impulse to each child rigidbody at the hit point so the ragdoll reacts naturally
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb != null && !rb.isKinematic)
            {
                rb.AddForceAtPosition(impulse, hitPoint, ForceMode.Impulse);
            }
        }

        // Also apply to root rigidbody if present and non-kinematic
        Rigidbody rootRb = GetComponent<Rigidbody>();
        if (rootRb != null && !rootRb.isKinematic)
        {
            rootRb.AddForce(impulse, ForceMode.Impulse);
        }
    }

    void setRigidbodyState(bool state)
    {

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }

        // May or may not need this, depending on how the NPC model is set up
        //GetComponent<Rigidbody>().isKinematic = !state;
    }

    // TODO remove if not needed after implementing final models
    void setColliderState(bool state)
    {

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }

        // May or may not need this, depending on how the NPC model is set up
        //GetComponent<Collider>().enabled = !state;
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
