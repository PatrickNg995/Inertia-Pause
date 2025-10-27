using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IPausable
{
    // Reference components
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Explosion _explosionScript;

    // Saved velocity for pause / unpause
    private Vector3 _savedVelocity;

    public void Awake()
    {
        // Get components in case they are not set
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _explosionScript = GetComponentInChildren<Explosion>();

        _capsuleCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Explode on contact with a lethal object
        if (other.CompareTag("Lethal"))
        {
            _capsuleCollider.enabled = false;
            _meshRenderer.enabled = false;
            _rb.isKinematic = true;
            _explosionScript.StartExplosion();
        }
    }

    // Save velocity & stop movement
    public void Pause()
    {
        _rb.isKinematic = true;
        _savedVelocity = _rb.linearVelocity;
    }

    // Start movement & add back saved velocity
    public void Unpause()
    {
        _rb.isKinematic = false;
        _rb.linearVelocity = _savedVelocity;
    }
}
