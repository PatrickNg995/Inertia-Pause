using UnityEngine;

public class PausableObject : MonoBehaviour
{
    // reference time pause script & rigidbody
    private TimePauseUnpause timePauseScript;
    private Rigidbody rb;

    // to save velocity when time is paused
    private Vector3 savedVelocity;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get rigidbody
        rb = GetComponent<Rigidbody>();

        // Connect to pause & unpause functions
        timePauseScript = GameObject.FindGameObjectWithTag("TimePause").GetComponent<TimePauseUnpause>();
        timePauseScript.pauseTime.AddListener(pauseObject);
        timePauseScript.unpauseTime.AddListener(unpauseObject);
    }

    // Save velocity & stop movement
    private void pauseObject()
    {
        savedVelocity = rb.linearVelocity;
        rb.isKinematic = true;
    }

    // Start movement & add back saved velocity
    private void unpauseObject()
    {
        rb.isKinematic = false;
        rb.linearVelocity = savedVelocity;
    }
}
