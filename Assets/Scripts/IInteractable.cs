using System;
using UnityEngine;

public interface IInteractable
{
    public void OnInteract();
    public void OnCancelInteract();
    public void OnResetInteract();
}
