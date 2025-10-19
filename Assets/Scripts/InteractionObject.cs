using UnityEngine;

// Abstract base class for all interactable objects in the game.
public abstract class InteractionObject : MonoBehaviour
{
    public InteractableObjectInfo InteractableInfo => _interactableInfo;
    [SerializeField] private InteractableObjectInfo _interactableInfo;

    // Whether the object has had its action taken.
    public bool HasTakenAction = false;

    // If the interaction should run once on press or per frame.
    public bool IsContinuousUpdate { get; protected set; }

    // Command to be executed on action.
    public ICommand ActionCommand { get; protected set; }

    public abstract void OnInteract();

    public abstract void OnCancelInteract();

    public abstract void OnResetInteract();
}
