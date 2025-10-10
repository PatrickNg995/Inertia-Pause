using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public float sensitivity = 1f;
    public Transform playerBody;

    private PlayerActions inputActions;
    private InputAction look;

    // Because we don't want the capsule to rotate vertically, we have to store the rotation of the camera and modify it
    private float rotationY;

    private void Awake()
    {
        inputActions = new PlayerActions();
    }

    private void OnEnable()
    {
        look = inputActions.Ingame.Look;
        look.Enable();

        // cursor is stuck to the middle of the game window
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        look.Disable();

        Cursor.lockState = CursorLockMode.None;
    }


    private void Update()
    {
        Vector2 v2 = look.ReadValue<Vector2>();
        
        float rotationX = v2.x * sensitivity; // Rotation on the x axis
        
        playerBody.Rotate(Vector3.up * rotationX);

        rotationY -= v2.y * sensitivity;
        // prevent players from doing 360s with the camera
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationY, 0f, 0f);
    }
}
