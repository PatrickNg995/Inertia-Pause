using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerActions inputActions;
    private InputAction movement;

    private Rigidbody rb;

    public Transform orientation;
    public float playerSpeed = 10f;
    public float climbSpeed = 4f;

    private bool isOnLadder = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerActions();
    }

    private void OnEnable()
    {
        movement = inputActions.Ingame.Movement;
        movement.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 v2 = movement.ReadValue<Vector2>();
        Vector3 verticalMovement = orientation.forward * v2.y;
        Vector3 horizontalMovement = orientation.right * v2.x;
        Vector3 v3 = verticalMovement + horizontalMovement;

        if (isOnLadder)
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
        rb.useGravity = true;
        rb.AddForce(v3 * playerSpeed, ForceMode.Force);
    }

    private void HandleLadderMovement(Vector2 v2)
    {
        rb.useGravity = false;

        // Move vertically when on ladder
        Vector3 climbDirection = new Vector3(0, v2.y, 0);
        rb.linearVelocity = climbDirection * climbSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
            rb.linearVelocity = Vector3.zero; // Stop momentum
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = false;
            rb.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.position.y < transform.position.y && isOnLadder)
        {
            isOnLadder = false;
            rb.useGravity = true;
        }
    }
}
