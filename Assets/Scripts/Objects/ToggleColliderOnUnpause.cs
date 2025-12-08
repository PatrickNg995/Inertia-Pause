using UnityEngine;

public class ToggleColliderOnUnpause : MonoBehaviour, IPausable
{
    [SerializeField] private Collider _collider;

    void Start()
    {
        _collider.enabled = false;
    }

    public void Pause()
    {
        
    }

    public void ResetStateBeforeUnpause()
    {
        _collider.enabled = false;
    }

    public void Unpause()
    {
        _collider.enabled = true;
    }
}
