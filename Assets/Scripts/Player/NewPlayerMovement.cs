using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class NewPlayerMovement : MonoBehaviour
{
    // Get character controller to move
    [SerializeField] private CharacterController _controller;
    [Space]

    // Movement variables
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _climbSpeed = 4f;
    [SerializeField] private float _ladderTopJump = 4f; // seems to be the magic number with movementSpeed 5
    private float fallingVelocity = 0f; // Save variable for when player falls

    // Player input
    private PlayerActions _inputActions;
    private InputAction _movement;

    private bool _isOnLadder = false;

    // Get character controller & player inputs
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _inputActions = new PlayerActions();
        fallingVelocity = 0f;
    }

    // Enable & disable input
    private void OnEnable()
    {
        _movement = _inputActions.Ingame.Movement;
        _movement.Enable();
    }

    private void OnDisable()
    {
        _movement.Disable();
    }


    // Always update to move player, regardless of Timescale
    void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        // Get input & put it into vector 3
        Vector2 v2 = _movement.ReadValue<Vector2>();
        if (_isOnLadder)
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
        _controller.Move(_movementSpeed * Time.unscaledDeltaTime * moveVector);

        // Zero out velocity & check for gravity
        velocity = Vector3.zero;
        if (_controller.isGrounded)
        {
            // Reset falling velocity
            velocity.y = -2f;
            fallingVelocity = 0f;
        }
        else
        {
            // Accumulate falling velocity & set the current velocity
            fallingVelocity += _gravity * Time.unscaledDeltaTime;
            velocity.y = fallingVelocity;
        }

        // Move player down by gravity
        _controller.Move(velocity);
    }

    private void HandleLadderMovement(Vector2 v2)
    {
        // Move vertically when on ladder
        Vector3 climbDirection = new Vector3(0, v2.y, 0);
        _controller.Move(_climbSpeed * Time.unscaledDeltaTime * climbDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            _isOnLadder = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            _isOnLadder = false;
        }

        // assumes that the player isn't grounded when they reach the top of a ladder
        // this should always be the case with how the ladder logic and collision detection works
        if (!_controller.isGrounded)
        {
            // Jumps to prevent jittering at the top of the ladder while trying to go from in front of the ladder to above and behind it
            _controller.Move((_ladderTopJump * _movementSpeed * transform.forward + Vector3.up) * Time.unscaledDeltaTime);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit _)
    {
        if (_controller.isGrounded && _isOnLadder)
        {
            _isOnLadder = false;
        }
    }
}
