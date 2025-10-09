using Unity.VisualScripting;
using UnityEngine;

public interface IPausable
{
    void Pause();
    void Unpause();

    void AddToTimePause(IPausable newObject)
    {
        TimePauseUnpause timePauseScript = GameObject.FindGameObjectWithTag("TimePause").GetComponent<TimePauseUnpause>();
        timePauseScript.AddPausableObject(newObject);
    }
}
