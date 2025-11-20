using UnityEngine;

public class ToggleObjectOnUnpause : MonoBehaviour, IPausable
{
    void IPausable.Pause()
    {
        gameObject.SetActive(true);
    }

    void IPausable.Unpause()
    {
        gameObject.SetActive(false);
    }

    void IPausable.ResetStateBeforeUnpause()
    {
        gameObject.SetActive(true);
    }
}
