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
    /// Invoked when the player starts interacting with a continuous interactable object.
    /// </summary>
    public Action<InteractableObjectInfo> OnContinuousInteract;

    /// <summary>
    /// Invoked when the player starts interacting with a one-shot interactable object.
    /// </summary>
    public Action<InteractableObjectInfo> OnOneShotInteract;

    /// <summary>
    /// Invoked when the player is interacting with an object and ends (confirms or cancels) the interaction.
    /// </summary>
    public Action<InteractableObjectInfo> OnEndInteraction;

    /// <summary>
    /// Invoked when the player resets an interaction with an object.
    /// </summary>
    public Action<ICommand> OnInteractReset;

    [SerializeField] private float _interactionDistance = 2;

    private Transform _pivot;
    private LayerMask _layerMask;
    private PlayerActions _actions;
    private InputAction _interact;
    private InputAction _cancelInteract;
    private InputAction _resetInteract;
    private bool _isInteracting = false;
    private InteractionObject _targetObject;

    private void Awake()
    {
        _pivot = GetComponent<Transform>();
        _layerMask = LayerMask.GetMask("InteractionObjects");
        _actions = new PlayerActions();
    }

    private void OnEnable()
    {
        _interact = _actions.Ingame.StartInteract;
        _cancelInteract = _actions.Ingame.CancelInteract;
        _resetInteract = _actions.Ingame.ResetInteract;

        _interact.performed += OnStartInteract;
        _cancelInteract.performed += OnCancelInteract;
        _resetInteract.performed += OnResetInteract;

        _interact.Enable();
        _cancelInteract.Enable();
        _resetInteract.Enable();
    }

    private void OnDisable()
    {
        _interact.Disable();
        _cancelInteract.Disable();
        _resetInteract.Disable();
    }

    private void OnStartInteract(InputAction.CallbackContext _)
    {
        if (_targetObject == null) return;

        if (_isInteracting)
        {
            _targetObject.OnEndInteract();
            OnEndInteraction?.Invoke(_targetObject.InteractableInfo);
            Debug.Log($"Ended interaction with {_targetObject.name}");
            _isInteracting = false;
            return;
        }

        if (_targetObject.IsContinuousUpdate)
        {
            _isInteracting = true;
            OnContinuousInteract?.Invoke(_targetObject.InteractableInfo);
        }
        else
        {
            OnOneShotInteract?.Invoke(_targetObject.InteractableInfo);
        }

        _targetObject.OnStartInteract();
        Debug.Log($"Started interacting with {_targetObject.name}");
    }

    private void OnResetInteract(InputAction.CallbackContext _)
    {
        if (_targetObject == null) return;  

        _targetObject.OnResetInteract();

        _isInteracting = false;
        OnEndInteraction?.Invoke(_targetObject.InteractableInfo);
        Debug.Log($"Reset interaction with {_targetObject.name}");
    }

    private void OnCancelInteract(InputAction.CallbackContext _)
    {
        if (_targetObject == null) return;

        if (_isInteracting)
        {
            _targetObject.OnCancelInteract();
            _isInteracting = false;
            OnEndInteraction?.Invoke(_targetObject.InteractableInfo);
            Debug.Log($"Cancelled interaction with {_targetObject.name}");
        }
    }

    void Update()
    {
        bool lookingAtObj = Physics.Raycast(_pivot.position, _pivot.forward, out RaycastHit hit, InteractionDistance, _layerMask, QueryTriggerInteraction.Collide);
        InteractionObject previousTarget = _targetObject;

        if (!lookingAtObj && !_isInteracting)
        {
            _targetObject = null;

            if (previousTarget != null)
            {
                // Player was looking at an interactable last frame and is not this frame.
                OnLookAwayFromInteractable?.Invoke();
            }

            return;
        }

        if (_isInteracting)
        {
            // If the player is currently interacting with an object.
            _targetObject.OnHoldInteract();
            return;
        }

        _targetObject = hit.transform.gameObject.GetComponent<InteractionObject>();

        // Looking at nothing then looking at an interactable, or switching from one interactable to another.
        if (previousTarget != _targetObject)
        {
            OnLookAtInteractable?.Invoke(_targetObject.InteractableInfo);
        }
    }
}
