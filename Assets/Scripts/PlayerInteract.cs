using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    public float interactionDistance = 2;

    private Transform pivot;
    private LayerMask layerMask;
    private PlayerActions actions;
    private InputAction interact;
    private InputAction cancelInteract;
    private InputAction resetInteract;
    private bool isInteracting = false;

    private void Awake()
    {
        pivot = GetComponent<Transform>();
        layerMask = LayerMask.GetMask("InteractionObjects");
        actions = new PlayerActions();
    }

    private void OnEnable()
    {
        interact = actions.Ingame.StartInteract;
        cancelInteract = actions.Ingame.CancelInteract;
        resetInteract = actions.Ingame.ResetInteract;

        interact.Enable();
        cancelInteract.Enable();
        resetInteract.Enable();
    }

    private void OnDisable()
    {
        interact.Disable();
        cancelInteract.Disable();
        resetInteract.Disable();
    }

    void Update()
    {
        bool[] buttonDown = { interact.ReadValue<float>() != 0,
            cancelInteract.ReadValue<float>() != 0,
            resetInteract.ReadValue<float>() != 0
        };

        Debug.DrawRay(pivot.position, pivot.forward, Color.red);

        if (Physics.Raycast(pivot.position, pivot.forward, out RaycastHit hit, interactionDistance, layerMask))
        {
            InteractionObject obj = hit.transform.gameObject.GetComponent<InteractionObject>();
            // looking at an interactable object
            if (buttonDown[0] && !isInteracting)
            {
                if (obj.continuousUpdate)
                {
                    isInteracting = true;
                }
                Debug.Log("hit");
                obj.OnInteract();
                
            } else if (buttonDown[1] && isInteracting)
            {
                // player was interacting with the object, wants to 
            } else if (buttonDown[2])
            {
                // player wants to reset the object
                obj.OnResetInteract();
                isInteracting = false;
            } else if (isInteracting)
            {
                // player is currently interacting with the item
                obj.OnInteract();
            }
        }
    }
}
