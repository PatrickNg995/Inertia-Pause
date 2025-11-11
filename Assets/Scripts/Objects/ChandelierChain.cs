using UnityEngine;

public class ChandelierChain : MonoBehaviour, IPausable
{
    // Reference Components.
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CapsuleCollider _collisionCollider;
    [SerializeField] private HingeJoint _hingeJoint;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Rigidbody _connectedChain;

    // Position before unpausing.
    private Vector3 _pausedPosition;
    private Quaternion _pausedRotation;

    private Vector3 _anchor;
    private Vector3 _connectedAnchor;

    private void Awake()
    {
        // Set up components.
        _rb = GetComponent<Rigidbody>();
        _collisionCollider = GetComponent<CapsuleCollider>();
        _hingeJoint = GetComponent<HingeJoint>();
        _meshRenderer = GetComponent<MeshRenderer>();

        // Save configuration of hinge joint.
        _anchor = _hingeJoint.anchor;
        _connectedChain = _hingeJoint.connectedBody;
        _connectedAnchor = _hingeJoint.connectedAnchor;
        _hingeJoint.autoConfigureConnectedAnchor = false;

        // Set up collider.
        _collisionCollider.isTrigger = true;
        _collisionCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // For bottom chain, return if colliding with the lethal
        // part of the chandelier.
        if (other.transform.parent == transform.parent)
        {
            return;
        }

        // Remove hinge joint if hit by a lethal object.
        if (other.gameObject.CompareTag("Lethal"))
        {
            Destroy(_hingeJoint);
            _meshRenderer.enabled = false;
        }
    }


    // Disable collider & rigidbody on pause.
    public void Pause()
    {
        _collisionCollider.enabled = false;

        _rb.isKinematic = true;
    }

    // Enable collider & rigidbody on unpause.
    public void Unpause()
    {
        _pausedPosition = transform.position;
        _pausedRotation = transform.rotation;

        _collisionCollider.enabled = true;

        _rb.isKinematic = false;
    }

    public void ResetStateBeforeUnpause()
    {
        // Make chain visible again.
        _meshRenderer.enabled = true;

        // Reset position & rotation to pre-unpause state.
        transform.SetPositionAndRotation(_pausedPosition, _pausedRotation);

        // Setup hinge joint again.
        if (_hingeJoint != null)
        {
            Destroy(_hingeJoint);
        }

        _hingeJoint = gameObject.AddComponent<HingeJoint>();
        _hingeJoint.anchor = _anchor;
        _hingeJoint.autoConfigureConnectedAnchor = false;
        _hingeJoint.connectedAnchor = _connectedAnchor;
        _hingeJoint.connectedBody = _connectedChain;

        // Wake up rigidbody.
        _rb.WakeUp();
    }
}
