using UnityEngine;

public class ToggleColliderOnUnpause : MonoBehaviour, IPausable
{
    [SerializeField] private Collider _collider;

    private void Start()
    {
        _collider.enabled = false;
    }

    public void Pause()
    {
        // No action needed on pause.
    }

    public void ResetStateBeforeUnpause()
    {
        _collider.enabled = false;
    }

    public void Unpause()
    {
        _collider.enabled = true;
    }

    public void SimulatePrePauseBehaviour()
    {
        // No pre-pause behaviour to simulate.
    }
}
