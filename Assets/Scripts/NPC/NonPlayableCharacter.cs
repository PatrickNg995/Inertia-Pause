using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class NPC : MonoBehaviour, IPausable
{
    [Header("NPC Look Target")]
    // The object the NPC should be facing.
    [SerializeField] private GameObject _lookTarget;

    [Header("NPC Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Collider _collider;
    [SerializeField] private BillboardIconController _billboardIconController;

    [Header("Accessory References")]
    // Constraints of accessories the NPC may have, used to make the NPC let go of them on death.
    [SerializeField] private List<ParentConstraint> _accessoryConstraints;

    // Rigidbodies of accessories the NPC may have, used to include them in ragdoll physics on death.
    [SerializeField] private List<Rigidbody> _accessoryRigidbodies;

    [Header("Armoured Enemies")]
    // For armoured enemies to take an additional hit from bullets.
    [SerializeField] private bool _isArmoured = false;
    private bool _hasArmour = false;

    // Distance in meters needed to fall to die.
    private const float LETHAL_FALL_THRESHOLD = 3f;

    // Force applied when hitting the ground from a fall to simulate impact.
    private const float FALL_HIT_FORCE = 15f;

    // Initial position of the NPC, for determining fall death.
    private Vector3 _initialPosition;

    // Position and rotation of the NPC before unpausing.
    private Vector3 _pausedPosition;
    private Quaternion _pausedRotation;

    // References to children components.
    private List<Rigidbody> _rigidbodies;
    private List<Collider> _colliders;

    // Whether the NPC is alive or dead.
    public bool IsAlive { get; private set; } = true;

    // Whether the NPC died in the last attempt.
    public bool HasDiedLastAttempt { get; private set; } = false;

    private void Awake()
    {
        // Cache references to all children components.
        _rigidbodies = GetComponentsInChildren<Rigidbody>().ToList();
        _rigidbodies.AddRange(_accessoryRigidbodies);
        _colliders = GetComponentsInChildren<Collider>().ToList();
        
        // Record initial position for determining fall death.
        _initialPosition = transform.position;

        // Start with ragdoll physics disabled.
        _animator.enabled = true;
        SetRigidbodyState(true);
        SetColliderState(false);

        // If there is a look target, make NPC face it.
        if (_lookTarget != null)
        {
            transform.LookAt(_lookTarget.transform);
        }

        // Armoured enemies get armour.
        if (_isArmoured)
        {
            _hasArmour = true;
        }
    }

    // Temp function for Debugging
    private void OnDrawGizmos()
    {
        if (_lookTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _lookTarget.transform.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        float fallDistance = _initialPosition.y - transform.position.y;

        // Check for lethal collisions.
        if (collisionObject.CompareTag("Lethal"))
        {
            Die(collisionObject);
        }

        // Check if NPC fell a lethal distance.
        if (fallDistance >= LETHAL_FALL_THRESHOLD)
        {
            // Apply downward hit force to simulate impact.
            ApplyHit(collisionObject, Vector3.down * FALL_HIT_FORCE, transform.position);
        }
    }

    public void Pause()
    {
        // Make rigidbody kinematic to pause physics.
        _rb.isKinematic = true;
    }

    public void Unpause()
    {
        // Save position and rotation before unpausing.
        _pausedPosition = transform.position;
        _pausedRotation = transform.rotation;

        // Restore rigidbody physics.
        _rb.isKinematic = false;

        // Clear death on last attempt on unpause.
        HasDiedLastAttempt = false;

        // Disable billboard icon on unpause.
        _billboardIconController.DisableBillboardIcon();
    }

    public void ResetStateBeforeUnpause()
    {
        // Reset position and rotation to pre-unpause state.
        transform.SetPositionAndRotation(_pausedPosition, _pausedRotation);

        // Disable ragdoll physics.
        _animator.enabled = true;
        SetRigidbodyState(true);
        SetColliderState(false);

        // Enable accessory constraints.
        SetConstraintState(true);

        // Revive NPC if dead.
        if (!IsAlive)
        {
            IsAlive = true;
        }

        // Give armoured enemies their armour.
        if (_isArmoured)
        {
            _hasArmour = true;
        }
    }

    public void SimulatePrePauseBehaviour()
    {
        // No pre-pause behaviour to simulate.
    }

    public void UpdateBillboardIconState()
    {
        _billboardIconController.UpdateBillboardIconState();
    }

    public void Die(GameObject cause)
    {
        GameManager.Instance.RecordNpcDeath(cause);

        // Disable animator, enable ragdoll physics.
        _animator.enabled = false;
        SetRigidbodyState(false);
        SetColliderState(true);

        // Disable accessory constraints.
        SetConstraintState(false);

        IsAlive = false;
        HasDiedLastAttempt = true;
    }

    public void ApplyHit(GameObject cause, Vector3 impulse, Vector3 hitPoint)
    {
        // Remove armour if hit by a bullet.
        if (_hasArmour)
        {
            if(cause.transform.root.CompareTag("Bullets"))
            {
                _hasArmour = false;
                return;
            }
        }
        // Other hits will immediately cause death.

        // Enable ragdoll
        Die(cause);

        // Apply impulse to each child rigidbody at the hit point so the ragdoll reacts naturally.
        foreach (Rigidbody rb in _rigidbodies)
        {
            if (rb != null && !rb.isKinematic)
            {
                rb.AddForceAtPosition(impulse, hitPoint, ForceMode.Impulse);
            }
        }
    }

    private void SetRigidbodyState(bool state)
    {
        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.isKinematic = state;
        }
        _rb.isKinematic = !state;
    }

    private void SetColliderState(bool state)
    {
        foreach (Collider collider in _colliders)
        {
            collider.enabled = state;
        }
        _collider.enabled = !state;
    }

    private void SetConstraintState(bool state)
    {
        foreach (ParentConstraint constraint in _accessoryConstraints)
        {
            constraint.enabled = state;
        }
    }
}
