using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerActions _inputActions;
    private InputAction _movement;

    private Rigidbody _rb;

    public Transform Orientation;
    public float PlayerSpeed = 10f;
    public float ClimbSpeed = 4f;

    private bool _isOnLadder = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inputActions = new PlayerActions();
    }

    private void OnEnable()
    {
        _movement = _inputActions.Ingame.Movement;
        _movement.Enable();
    }

    private void OnDisable()
    {
        _movement.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 v2 = _movement.ReadValue<Vector2>();
        Vector3 verticalMovement = Orientation.forward * v2.y;
        Vector3 horizontalMovement = Orientation.right * v2.x;
        Vector3 v3 = verticalMovement + horizontalMovement;

        if (_isOnLadder)
        {
            HandleLadderMovement(v2);
        }
        else
        {
            HandleGroundMovement(v3);
        }
    }

    private void HandleGroundMovement(Vector3 v3)
    {
        v3.y = 0;
        _rb.useGravity = true;
        _rb.AddForce(v3 * PlayerSpeed, ForceMode.Force);
    }

    private void HandleLadderMovement(Vector2 v2)
    {
        _rb.useGravity = false;

        // Move vertically when on ladder
        Vector3 climbDirection = new Vector3(0, v2.y, 0);
        _rb.linearVelocity = climbDirection * ClimbSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            _isOnLadder = true;
            _rb.linearVelocity = Vector3.zero; // Stop momentum
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            _isOnLadder = false;
            _rb.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.position.y < transform.position.y && _isOnLadder)
        {
            _isOnLadder = false;
            _rb.useGravity = true;
        }
    }
}
