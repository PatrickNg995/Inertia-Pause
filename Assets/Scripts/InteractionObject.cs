using UnityEngine;

// class is used as the interaction handler; actual interaction logic is in other scripts
public class InteractionObject : MonoBehaviour
{
    public InteractableObjectInfo InteractableInfo => _interactableInfo;

    // if the interaction should run once on press or per frame
    public bool continuousUpdate;

    // the script with the behaviour on interactions
    private IInteractable[] InteractionBehaviour;

    [SerializeField] private InteractableObjectInfo _interactableInfo;

    private void Start()
    {
        InteractionBehaviour = GetComponents<IInteractable>();
    }

    public void OnStartInteract()
    {
        foreach (IInteractable behaviour in InteractionBehaviour) behaviour.OnStartInteract();
    }

    public void OnHoldInteract()
    {
        foreach (IInteractable behaviour in InteractionBehaviour) behaviour.OnHoldInteract();
    }

    public void OnEndInteract()
    {
        foreach (IInteractable behaviour in InteractionBehaviour) behaviour.OnEndInteract();
    }

    public void OnCancelInteract()
    {
        foreach (IInteractable behaviour in InteractionBehaviour) behaviour.OnCancelInteract();
    }

    public void OnResetInteract()
    {
        foreach (IInteractable behaviour in InteractionBehaviour) behaviour.OnResetInteract();
    }
}
