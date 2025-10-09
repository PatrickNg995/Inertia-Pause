using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShelfBehaviour : MonoBehaviour, IInteractable
{
    public float torque;

    private Rigidbody rb;
    private Vector3 initialLocation;
    private Quaternion initialRotation;
    private bool hasBeenInteracted = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialLocation = transform.position;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        // TODO: notification logic to show how / if it'll fall
    }

    public void OnCancelInteract()
    {
        // this should only be thrown due to a logic error in PlayerInteract
        throw new System.NotImplementedException();
    }

    public void OnInteract()
    {
        if (hasBeenInteracted) { return; }

        // right is the direction the object is facing
        rb.AddTorque(transform.right * torque);
        hasBeenInteracted = true;
    }

    public void OnResetInteract()
    {
        transform.position = initialLocation;
        transform.rotation = initialRotation;
        hasBeenInteracted = false;  
    }
}
