using UnityEngine;

public interface IInteractable
{
    public void OnStartInteract();

    public void OnHoldInteract();

    public void OnEndInteract();
    public void OnCancelInteract();
    public void OnResetInteract();
}
