using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TimePauseUnpause : MonoBehaviour
{
    // For player inputs
    private PlayerActions inputActions;
    private InputAction timePause;

    // Events to trigger functions in other scripts
    public UnityEvent pauseTime;
    public UnityEvent unpauseTime;

    // Add start delay to time pause
    public float timePauseDelay;

    // Bool to only pause / unpause once
    private bool hasPaused = false;
    private bool hasUnpaused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Bind time pause input to function
        inputActions = new PlayerActions();
        timePause = inputActions.Ingame.TimePause;

        // make sure it is false
        hasPaused = false;
        hasUnpaused = false;
}

    // Enable & disable input actions
    private void OnEnable()
    {
        timePause.performed += checkUnpause;
        timePause.Enable();
    }

    private void OnDisable()
    {
        timePause.performed -= checkUnpause;
        timePause.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // Only done at the beginning
        while (!hasPaused)
        {
            // Run timer & pause time if timer <= 0
            timePauseDelay -= Time.deltaTime;
            if (timePauseDelay <= 0)
            {
                hasPaused = true; 
                pauseTime.Invoke();
            }
        }
    }

    // Unpause time if time has been paused
    private void checkUnpause(InputAction.CallbackContext context)
    {
        if (hasPaused && !hasUnpaused)
        {
            hasUnpaused = true;
            unpauseTime.Invoke();
        }
    }
}
