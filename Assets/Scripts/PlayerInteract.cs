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
    private InteractionObject obj;

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

        interact.performed += OnStartInteract;
        cancelInteract.performed += OnCancelInteract;
        resetInteract.performed += OnResetInteract;

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

    private void OnStartInteract(InputAction.CallbackContext _)
    {

        if (isInteracting || obj == null) return;
        
        if (obj.continuousUpdate)
        {
            isInteracting = true;
        }

        obj.OnInteract();
    }

    private void OnResetInteract(InputAction.CallbackContext _)
    {
        if (obj == null) return;

        obj.OnResetInteract();
        isInteracting = false;
    }

    private void OnCancelInteract(InputAction.CallbackContext _)
    {
        if (obj = null) return;

        if (isInteracting)
        {
            obj.OnCancelInteract();
            isInteracting = false;
        }
    }

    void Update()
    {
        bool lookingAtObj = Physics.Raycast(pivot.position, pivot.forward, out RaycastHit hit, interactionDistance, layerMask);
        if (!lookingAtObj && !isInteracting)
        {
            obj = null;
            return;
        }

        obj = hit.transform.gameObject.GetComponent<InteractionObject>();

        if (isInteracting)
        {
            // if the player has already pressed on the object 
            obj.OnInteract();
        }
    }
}
