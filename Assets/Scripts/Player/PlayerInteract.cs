using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    /// <summary>
    /// Invoked when the player looks at an interactable object.
    /// </summary>
    public Action<InteractableObjectInfo> OnLookAtInteractable;

    /// <summary>
    /// Invoked when the player is looking at an interactable object and then looks away.
    /// </summary>
    public Action OnLookAwayFromInteractable;

    /// <summary>
    /// Invoked when the player starts interacting with an interactable object.
    /// </summary>
    public Action<InteractableObjectInfo> OnInteract;

    /// <summary>
    /// Invoked when the player is interacting with an object and ends (confirms or cancels) the interaction.
    /// </summary>
    public Action<InteractableObjectInfo> OnEndInteraction;

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
        OnInteract?.Invoke(targetObject.InteractableInfo);
    }

    private void OnResetInteract(InputAction.CallbackContext _)
    {
        if (targetObject == null) return;

        targetObject.OnResetInteract();
        isInteracting = false;
        OnEndInteraction?.Invoke(targetObject.InteractableInfo);
    }

    private void OnCancelInteract(InputAction.CallbackContext _)
    {
        if (targetObject = null) return;

        if (isInteracting)
        {
            targetObject.OnCancelInteract();
            isInteracting = false;
            OnEndInteraction?.Invoke(targetObject.InteractableInfo);
        }
    }

    void Update()
    {
        bool lookingAtObj = Physics.Raycast(pivot.position, pivot.forward, out RaycastHit hit, interactionDistance, layerMask);
        InteractionObject previousTarget = targetObject;

        if (!lookingAtObj && !isInteracting)
        {
            targetObject = null;

            if (previousTarget != null)
            {
                // Player was looking at an interactable last frame and is not this frame.
                OnLookAwayFromInteractable?.Invoke();
            }

            return;
        }

        targetObject = hit.transform.gameObject.GetComponent<InteractionObject>();

        // Looking at nothing then looking at an interactable, or switching from one interactable to another.
        if (previousTarget != targetObject)
        {
            OnLookAtInteractable?.Invoke(targetObject.InteractableInfo);
        }


        if (isInteracting)
        {
            // if the player has already pressed on the object 
            targetObject.OnInteract();
        }
    }
}
