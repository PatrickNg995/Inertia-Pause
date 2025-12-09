using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class NewPlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;

    [Header("Movement")]
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _gravity = -0.4f;
    [SerializeField] private float _maxFallSpeed = -0.4f;
    [SerializeField] private float _climbSpeed = 4f;
    [SerializeField] private float _ladderTopJump = 4f;

    private float _fallingVelocity = 0f;

    private PlayerActions _inputActions;
    private InputAction _movement;

    private bool _isOnLadder = false;

    private Vector3 _initialPlayerPosition;
    private Quaternion _initialPlayerRotation;

    [Header("Footsteps")]
    [SerializeField] private float _minStepSpeed = 0.15f;
    [SerializeField] private float _footstepVolume = 1f;

    [Header("Climbsteps")]
    [SerializeField] private float _minClimbSpeed = 0.05f;
    [SerializeField] private float _climbVolume = 1f;

    private AudioSource _footstepAudio;
    private AudioSource _climbAudio;

    private void Awake()
    {
        _initialPlayerPosition = transform.position;
        _initialPlayerRotation = transform.rotation;

        _controller = GetComponent<CharacterController>();
        _inputActions = new PlayerActions();
        _fallingVelocity = 0f;
    }

    private void Start()
    {
        CreateFootstepSource();
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

    private void Update()
    {
        MovePlayer();
        HandleFootsteps();
    }

    public void ResetPlayerPosition()
    {
        _controller.enabled = false;
        transform.SetPositionAndRotation(_initialPlayerPosition, _initialPlayerRotation);
        _controller.enabled = true;
    }

    private void MovePlayer()
    {
        Vector2 v2 = _movement.ReadValue<Vector2>();

        if (_isOnLadder)
            HandleLadderMovement(v2);
        else
            HandleGroundMovement(v2);
    }

    private void HandleGroundMovement(Vector2 v2)
    {
        Vector3 velocity = new Vector3(v2.x, 0f, v2.y);
        velocity = transform.TransformDirection(velocity);

        velocity.x *= _movementSpeed * Time.unscaledDeltaTime;
        velocity.z *= _movementSpeed * Time.unscaledDeltaTime;

        if (_controller.isGrounded)
        {
            velocity.y = -0.05f;
            _fallingVelocity = 0f;
        }
        else
        {
            _fallingVelocity += _gravity * Time.unscaledDeltaTime;

            if (_fallingVelocity < _maxFallSpeed)
            {
                _fallingVelocity = _maxFallSpeed;
            }

            velocity.y = _fallingVelocity;
        }

        _controller.Move(velocity);
    }

    private void HandleLadderMovement(Vector2 v2)
    {
        Vector3 climbDirection = new Vector3(0f, v2.y, 0f);
        _controller.Move(_climbSpeed * Time.unscaledDeltaTime * climbDirection);

        // Handle Climb Audio.
        float climbAmount = Mathf.Abs(v2.y);

        if (_climbAudio != null)
        {
            if (climbAmount > _minClimbSpeed)
            {
                _climbAudio.volume = _climbVolume;
            }
            else
            {
                _climbAudio.volume = 0f;
            }
        }
    }

    private void CreateFootstepSource()
    {
        _footstepAudio = SFXPlayer.Instance.PlayAttached(SfxId.Walking, transform, loop: true);

        if (_footstepAudio != null)
        {
            _footstepAudio.volume = 0f;
            _footstepAudio.spatialBlend = 0f;
        }
    }


    private void HandleFootsteps()
    {
        if (!_controller.isGrounded || _isOnLadder)
        {
            _footstepAudio.volume = 0f;
            return;
        }

        Vector3 horizontal = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z);
        float speed = horizontal.magnitude;

        if (speed < _minStepSpeed)
        {
            _footstepAudio.volume = 0f;
            return;
        }

        _footstepAudio.volume = _footstepVolume;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            _isOnLadder = true;

            if (_climbAudio == null)
            {
                _climbAudio = SFXPlayer.Instance.PlayAttached(SfxId.ClimbLadder, transform, loop: true);

                if (_climbAudio != null)
                {
                    _climbAudio.volume = _footstepVolume;
                    _climbAudio.spatialBlend = 0f;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            _isOnLadder = false;

            if (_climbAudio != null)
            {
                Destroy(_climbAudio.gameObject);
                _climbAudio = null;
            }
        }

        if (!_controller.isGrounded)
        {
            _controller.Move((_ladderTopJump * _movementSpeed * transform.forward + Vector3.up) * Time.unscaledDeltaTime);
        }
    }


    private void OnControllerColliderHit(ControllerColliderHit _)
    {
        if (_controller.isGrounded && _isOnLadder)
        {
            _isOnLadder = false;

            if (_climbAudio != null)
            {
                Destroy(_climbAudio.gameObject);
                _climbAudio = null;
            }
        }
    }
}
