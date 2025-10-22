using UnityEngine;

// Abstract base class for all interactable objects in the game.
public abstract class InteractionObject : MonoBehaviour, IInteractable
{
    public InteractableObjectInfo InteractableInfo => _interactableInfo;
    [SerializeField] private InteractableObjectInfo _interactableInfo;

    // Whether the object has had its action taken.
    public bool HasTakenAction { get; set; } = false;

    // If the interaction should run once on press or per frame.
    public bool IsContinuousUpdate { get; protected set; } = false;

    // Command to be executed on action.
    public ActionCommand ActionCommand { get; protected set; }

    public abstract void OnStartInteract();

    public abstract void OnHoldInteract();

    public abstract void OnEndInteract();

    public abstract void OnCancelInteract();

    public abstract void OnResetInteract();
}
