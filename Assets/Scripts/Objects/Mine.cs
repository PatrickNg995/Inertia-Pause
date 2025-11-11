using UnityEngine;

public class Mine : MonoBehaviour, IPausable
{
    // Reference components.
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private BoxCollider _collisionCollider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Explosion _explosionScript;

    // Save the layer the mine can interact with.
    [SerializeField] private LayerMask _interactableLayer;

    // Saved velocity for pause / unpause.
    private Vector3 _savedVelocity;

    // Bool for exploding the mine.
    private bool _canExplode = false;

    // Save position & rotation for rewinding.
    private Vector3 _pausedPosition;
    private Quaternion _pausedRotation;

    public void Awake()
    {
        // Set collider properly.
        _collisionCollider.isTrigger = false;

        // Make sure the mine can't explode
        _canExplode = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_canExplode)
        {
            return;
        }

        // Get the layer mask of the other object with bitwise left shift.
        LayerMask collisionLayer = 1 << collision.gameObject.layer;

        // Check if the other object is in the correct layer or tag to explode.
        if (collisionLayer == _interactableLayer || collision.gameObject.CompareTag("Lethal"))
        {
            TriggerExplosion();
        }
    }

    private void TriggerExplosion()
    {
        // Cleanup mine without removing it & start explosion.
        _canExplode = false;
        _collisionCollider.enabled = false;
        _meshRenderer.enabled = false;

        _rb.isKinematic = true;
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
    }

    public void ResetStateBeforeUnpause()
    {
        // Reset the mine.
        _collisionCollider.enabled = true;
        _meshRenderer.enabled = true;
        _explosionScript.ResetExplosion();

        transform.SetPositionAndRotation(_pausedPosition, _pausedRotation);
    }
}
