using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class NewPlayerMovement : MonoBehaviour
{
    // Get character controller to move
    [SerializeField] private CharacterController controller;
    [Space]

    // Movement variables
    [SerializeField] private float movementSpeed = 5;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float climbSpeed = 4f;
    [SerializeField] private float ladderTopJump = 4; // seems to be the magic number with movementSpeed 5

    // Player input
    private PlayerActions inputActions;
    private InputAction movement;

    private bool isOnLadder = false;

    // Get character controller & player inputs
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerActions();
    }

    // Enable & disable input
    private void OnEnable()
    {
        movement = inputActions.Ingame.Movement;
        movement.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
    }


    // Always update to move player, regardless of Timescale
    void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        // Get input & put it into vector 3
        Vector2 v2 = movement.ReadValue<Vector2>();
        if (isOnLadder)
        {
            HandleLadderMovement(v2);
        }
        else
        {
            HandleGroundMovement(v2);
        }
    }

    private void HandleGroundMovement(Vector2 v2)
    {
        Vector3 velocity = new Vector3(v2.x, 0, v2.y);

        // Move player based on input & direction they are facing
        Vector3 moveVector = transform.TransformDirection(velocity);
        controller.Move(movementSpeed * Time.unscaledDeltaTime * moveVector);

        // Zero out velocity & check for gravity
        velocity = Vector3.zero;
        if (controller.isGrounded)
        {
            velocity.y = -1f;
        }
        else
        {
            velocity.y -= gravity * -2f * Time.unscaledDeltaTime;
        }

        // Move player down by gravity
        controller.Move(velocity);
    }

    private void HandleLadderMovement(Vector2 v2)
    {
        // Move vertically when on ladder
        Vector3 climbDirection = new Vector3(0, v2.y, 0);
        controller.Move(climbSpeed * Time.unscaledDeltaTime * climbDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isOnLadder = false;
        }

        // assumes that the player isn't grounded when they reach the top of a ladder
        // this should always be the case with how the ladder logic and collision detection works
        if (!controller.isGrounded)
        {
            // Jumps to prevent jittering at the top of the ladder while trying to go from in front of the ladder to above and behind it
            controller.Move((ladderTopJump * movementSpeed * transform.forward + Vector3.up) * Time.unscaledDeltaTime);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit _)
    {
        if (controller.isGrounded && isOnLadder)
        {
            isOnLadder = false;
        }
    }
}
