using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IPausable
{
    // Reference components.
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CapsuleCollider _triggerCollider;
    [SerializeField] private CapsuleCollider _collisionCollider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Explosion _explosionScript;

    // Distance in meters needed to fall to explode.
    private const float LETHAL_FALL_THRESHOLD = 3f;

    // Saved velocity for pause / unpause.
    private Vector3 _savedVelocity;

    // Initial position for falling.
    private Vector3 _initialPosition;

    // Bool for exploding the barrel
    private bool _canExplode = false;

    private Vector3 _pausedPosition;
    private Quaternion _pausedRotation;

    public void Awake()
    {
        // Set colliders properly.
        _triggerCollider.isTrigger = true;
        _collisionCollider.isTrigger = false;

        // Make sure the barrel can't explode
        _canExplode = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canExplode)
        {
            return;
        }

        // Explode on contact with a lethal object.
        if (other.CompareTag("Lethal"))
        {
            TriggerExplosion();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!_canExplode)
        {
            return;
        }

        // Explode on contact with a lethal object.
        if (collision.gameObject.CompareTag("Lethal"))
        {
            TriggerExplosion();
        }

        // Explode after falling from high enough.
        float fallHeight = _initialPosition.y - transform.position.y;
        if (fallHeight >= LETHAL_FALL_THRESHOLD)
        {
            TriggerExplosion();
        }
    }

    private void TriggerExplosion()
    {
        // Cleanup barrel without removing it & start explosion.
        _canExplode = false;
        _triggerCollider.enabled = false;
        _collisionCollider.enabled = false;
        _meshRenderer.enabled = false;
        //Destroy(_rb);
        _explosionScript.StartExplosion();
    }

    // Save velocity & stop movement.
    public void Pause()
    {
        _canExplode = false;
        _rb.isKinematic = true;
        _savedVelocity = _rb.linearVelocity;
    }

    // Start movement & add back saved velocity
    public void Unpause()
    {
        _pausedPosition = transform.position;
        _pausedRotation = transform.rotation;

        _canExplode = true;
        _rb.isKinematic = false;
        _rb.linearVelocity = _savedVelocity;
        _initialPosition = transform.position;
    }

    public void ResetStateBeforeUnpause()
    {
        // Reset the barrel.
        _triggerCollider.enabled = true;
        _collisionCollider.enabled = true;
        _meshRenderer.enabled = true;
        _explosionScript.ResetExplosion();

        transform.SetPositionAndRotation(_pausedPosition, _pausedRotation);
    }

    public void SimulatePrePauseBehaviour()
    {
        // No pre-pause behaviour to simulate.
    }
}
