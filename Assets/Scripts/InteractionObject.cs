using UnityEngine;

// class is used as the interaction handler; actual interaction logic is in other scripts
public class InteractionObject : MonoBehaviour
{
    // if the interaction should run once on press or per frame
    public bool continuousUpdate;

    // the script with the behaviour on interactions
    private IInteractable interactionBehaviour;

    private void Start()
    {
        interactionBehaviour = GetComponent<IInteractable>();
    }

    public void OnInteract()
    {
        interactionBehaviour.OnInteract();
    }

    public void OnCancelInteract()
    {
        interactionBehaviour.OnCancelInteract();
    }

    public void OnResetInteract()
    {
        interactionBehaviour.OnResetInteract();
    }
}
