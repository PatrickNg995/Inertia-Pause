public interface IInteractable
{
    public bool OnStartInteract();
    public void OnHoldInteract();
    public void OnEndInteract();
    public void OnCancelInteract();
    public void OnResetInteract();
}
