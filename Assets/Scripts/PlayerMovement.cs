using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerActions inputActions;
    private InputAction movement;
    Rigidbody rb;

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
        Vector3 v3 = new Vector3(v2.x/2, 0, v2.y/2);

        //rb.AddForce(v3, ForceMode.VelocityChange);
        transform.Translate(v3);
    }
}
