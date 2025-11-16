using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] public float HorizontalSensitivity = 0.15f;
    [SerializeField] public float VerticalSensitivity = 0.15f;

    [SerializeField] private Transform _playerBody;

    private const float GAMEPAD_SENSITIVITY_MODIFIER = 1000f;

    private PlayerActions _inputActions;
    private InputAction _look;

    private Vector2 _lookInput = Vector2.zero;

    private bool _isUsingGamepad = false;

    private float _xRotation = 0f;

    private void Awake()
    {
        _inputActions = new PlayerActions();
    }

    private void OnEnable()
    {
        _look = _inputActions.Ingame.Look;
        _look.performed += OnLook;
        _look.canceled += OnLook;
        _look.Enable();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        _look.performed -= OnLook;
        _look.canceled -= OnLook;
        _look.Disable();

        Cursor.lockState = CursorLockMode.None;
    }

    private void LateUpdate()
    {
        // Use Time.deltaTime if using a gamepad.
        float timeScale = _isUsingGamepad ? Time.deltaTime : 1f;

        // Gamepad sensitivity has to be much higher; multiply the sensitivity picked in options by the modifier for gamepad.
        float gamepadHorizontalSensitivity = GAMEPAD_SENSITIVITY_MODIFIER * HorizontalSensitivity;
        float gamepadVerticalSensitivity = GAMEPAD_SENSITIVITY_MODIFIER * VerticalSensitivity;

        // Choose sensitivity based on input device.
        float horizontalSensitivity = _isUsingGamepad ? gamepadHorizontalSensitivity : HorizontalSensitivity;
        float verticalSensitivity = _isUsingGamepad ? gamepadVerticalSensitivity : VerticalSensitivity;

        // Apply look input.
        float yaw = _lookInput.x * horizontalSensitivity * timeScale;
        float pitch = _lookInput.y * verticalSensitivity * timeScale;

        _xRotation -= pitch;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        _playerBody.Rotate(Vector3.up * yaw);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();

        // Determine if input came from gamepad.
        var device = context.control?.device;
        _isUsingGamepad = device is Gamepad;
    }
}
