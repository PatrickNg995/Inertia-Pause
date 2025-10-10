using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerActions inputActions;
    private InputAction movement;
    Rigidbody rb;

    public Transform orientation;
    public float playerSpeed;

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

    private void Update()
    {
     
    }

    private void FixedUpdate()
    {
        Vector2 v2 = movement.ReadValue<Vector2>();
        // Vector3 v3 = new Vector3(v2.x * playerSpeed * Time.fixedDeltaTime, 0, v2.y * playerSpeed * Time.fixedDeltaTime);
        Vector3 verticalMovement = orientation.forward * v2.y;
        Vector3 horizontalMovement = orientation.right * v2.x;
        Vector3 v3 = verticalMovement + horizontalMovement;
        v3.y = 0;

        rb.AddForce(v3 * playerSpeed, ForceMode.Force);
        // transform.Translate(v3);
        // rb.linearVelocity = v3 * playerSpeed;
    }
}
