using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerMovement : MonoBehaviour
{
    // Get character controller to move
    [SerializeField] private CharacterController controller;
    [Space]

    // Movement variables
    [SerializeField] private float movementSpeed;
    [SerializeField] private float gravity = -9.81f;

    // Player input
    private PlayerActions inputActions;
    private InputAction movement;


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
        Vector3 velocity = new Vector3(v2.x, 0, v2.y);

        // Move player based on input & direction they are facing
        Vector3 moveVector = transform.TransformDirection(velocity);
        controller.Move(moveVector * movementSpeed * Time.unscaledDeltaTime);

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
}
