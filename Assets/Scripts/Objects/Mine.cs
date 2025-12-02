using UnityEngine;

public class Mine : MonoBehaviour, IPausable
{
    // Reference components.
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private BoxCollider _triggerCollider;
    [SerializeField] private BoxCollider _collisionCollider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Explosion _explosionScript;
    [SerializeField] private GameObject _lightObj;

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
        // Set colliders properly.
        _triggerCollider.isTrigger = true;
        _collisionCollider.isTrigger = false;

        // Make sure the mine can't explode at start.
        _canExplode = false;

        // Ensure the light object starts enabled.
        if (_lightObj != null)
            _lightObj.SetActive(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_canExplode)
            return;

        LayerMask collisionLayer = 1 << collision.gameObject.layer;

        if (collisionLayer == _interactableLayer)
            TriggerExplosion();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canExplode)
            return;

        if (other.CompareTag("Lethal"))
            TriggerExplosion();
    }

    private void TriggerExplosion()
    {
        _canExplode = false;

        _collisionCollider.enabled = false;
        _triggerCollider.enabled = false;
        _meshRenderer.enabled = false;

        // Disable the child's Light GameObject.
        if (_lightObj != null)
            _lightObj.SetActive(false);

        _rb.isKinematic = true;
        _explosionScript.StartExplosion();
    }

    public void Pause()
    {
        _canExplode = false;
        _rb.isKinematic = true;
        _savedVelocity = _rb.linearVelocity;
    }

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
        _collisionCollider.enabled = true;
        _triggerCollider.enabled = true;
        _meshRenderer.enabled = true;

        // Reactivate light object.
        if (_lightObj != null)
            _lightObj.SetActive(true);

        _explosionScript.ResetExplosion();

        transform.SetPositionAndRotation(_pausedPosition, _pausedRotation);
    }
}
