using UnityEngine;

// class is used as the interaction handler; actual interaction logic is in other scripts
public class InteractionObject : MonoBehaviour
{
    // if the interaction should run once on press or per frame
    public bool continuousUpdate;

    // the script with the behaviour on interactions
    private IInteractable[] interactionBehaviour;

    private void Start()
    {
        interactionBehaviour = GetComponents<IInteractable>();
    }

    public void OnInteract()
    {
        foreach (IInteractable behaviour in interactionBehaviour) behaviour.OnInteract();
    }

    public void OnCancelInteract()
    {
        foreach (IInteractable behaviour in interactionBehaviour) behaviour.OnCancelInteract();
    }

    public void OnResetInteract()
    {
        foreach (IInteractable behaviour in interactionBehaviour) behaviour.OnResetInteract();
    }
}
