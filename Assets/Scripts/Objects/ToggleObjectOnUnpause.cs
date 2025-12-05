using UnityEngine;

public class ToggleObjectOnUnpause : MonoBehaviour, IPausable
{
    public void Pause()
    {
        gameObject.SetActive(true);
    }

    public void Unpause()
    {
        gameObject.SetActive(false);
    }

    public void ResetStateBeforeUnpause()
    {
        gameObject.SetActive(true);
    }

    public void SimulatePrePauseBehaviour()
    {
        // No pre-pause behaviour to simulate.
    }
}
