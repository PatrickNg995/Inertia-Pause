﻿using UnityEngine;

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

    // References to components.
    private Rigidbody _rb;
    private Animator _animator;
    private Collider _collider;

    // Whether the NPC is alive or dead.
    public bool IsAlive { get; private set; } = true;

    private void Start()
    {
        // Required for time pausing; remove/rework when the time pausing system gets reworked.
        GetComponent<IPausable>().AddToTimePause(this);

        // Cache references to components.
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();

        // Record initial position for determining fall damage.
        _initialPosition = transform.position;

        // Start with ragdoll physics disabled.
        _animator.enabled = true;
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
        if (fallDistance >= LETHAL_FALL_THRESHOLD)
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
        _animator.enabled = false;
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
        _rb.isKinematic = !state;
    }

    private void SetColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }
        _collider.enabled = !state;
    }
}
