using UnityEngine;

public class Grenade : MonoBehaviour, IPausable
{
    [Header("Grenade Settings")]
    [SerializeField] private float _grenadeInitialSpeed = 10f;

    [SerializeField] private float _grenadeInitialExplosionDelay = 1.5f;
    private float _grenadeCurrentExplosionDelay;

    [Header("Components")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Explosion _explosionScript;

    // NEW: spoon animator
    [Header("Animation")]
    [SerializeField] private Animator _animator;
    private static readonly int ArmParam = Animator.StringToHash("Arm");

    private bool _hasBeenThrown = false;
    private bool _canExplode;

    private Vector3 _savedVelocity;
    private Vector3 _pausedPosition;
    private Quaternion _pausedRotation;

    private void Awake()
    {
        _canExplode = false;
        _grenadeCurrentExplosionDelay = _grenadeInitialExplosionDelay;

        // While in hand
        _rb.isKinematic = true;
        _sphereCollider.isTrigger = true;
    }

    private void Update()
    {
        if (_canExplode)
        {
            _grenadeCurrentExplosionDelay -= Time.deltaTime;
            if (_grenadeCurrentExplosionDelay < 0f)
            {
                _canExplode = false;

                _sphereCollider.enabled = false;
                _meshRenderer.enabled = false;
                _rb.isKinematic = true;

                _explosionScript.StartExplosion();
            }
        }
    }

    // ----------------------------------------------------------
    //  THROWING LOGIC  (called by enemy animation event)
    // ----------------------------------------------------------
    public void ThrowGrenade()
    {
        if (_hasBeenThrown) return;
        _hasBeenThrown = true;

        // 1. Detach from the hand.
        transform.parent = null;

        // 2. Enable physics.
        _rb.isKinematic = false;
        _sphereCollider.enabled = true;
        _sphereCollider.isTrigger = false;

        // 3. Spoon animation trigger.
        if (_animator != null)
            _animator.SetTrigger(ArmParam);

        // 4. Apply initial velocity.
        _savedVelocity = transform.forward * _grenadeInitialSpeed;
        _rb.linearVelocity = _savedVelocity;

        // 5. Start fuse timer now.
        _grenadeCurrentExplosionDelay = _grenadeInitialExplosionDelay;
        _canExplode = true;
    }

    // ----------------------------------------------------------
    //  IPausable INTERFACE
    // ----------------------------------------------------------
    public void Pause()
    {
        _canExplode = false;

        if (_rb.linearVelocity == Vector3.zero)
            _savedVelocity = transform.forward * _grenadeInitialSpeed;
        else
            _savedVelocity = _rb.linearVelocity;

        _rb.isKinematic = true;
        _sphereCollider.isTrigger = true;

        _grenadeCurrentExplosionDelay = _grenadeInitialExplosionDelay;
    }

    public void Unpause()
    {
        _pausedPosition = transform.position;
        _pausedRotation = transform.rotation;

        _canExplode = true;

        _rb.isKinematic = false;
        _sphereCollider.isTrigger = false;

        _rb.linearVelocity = _savedVelocity;
    }

    public void ResetStateBeforeUnpause()
    {
        _sphereCollider.enabled = true;
        _meshRenderer.enabled = true;
        _grenadeCurrentExplosionDelay = _grenadeInitialExplosionDelay;

        _explosionScript.ResetExplosion();

        transform.SetPositionAndRotation(_pausedPosition, _pausedRotation);
    }
}
