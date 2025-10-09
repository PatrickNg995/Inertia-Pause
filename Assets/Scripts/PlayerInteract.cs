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
    private InteractionObject targetObject;

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

        if (isInteracting || targetObject == null) return;
        
        if (targetObject.continuousUpdate)
        {
            isInteracting = true;
        }

        targetObject.OnInteract();
    }

    private void OnResetInteract(InputAction.CallbackContext _)
    {
        if (targetObject == null) return;

        targetObject.OnResetInteract();
        isInteracting = false;
    }

    private void OnCancelInteract(InputAction.CallbackContext _)
    {
        if (targetObject = null) return;

        if (isInteracting)
        {
            targetObject.OnCancelInteract();
            isInteracting = false;
        }
    }

    void Update()
    {
        bool lookingAtObj = Physics.Raycast(pivot.position, pivot.forward, out RaycastHit hit, interactionDistance, layerMask);
        if (!lookingAtObj && !isInteracting)
        {
            targetObject = null;
            return;
        }

        targetObject = hit.transform.gameObject.GetComponent<InteractionObject>();

        if (isInteracting)
        {
            // if the player has already pressed on the object 
            targetObject.OnInteract();
        }
    }
}
