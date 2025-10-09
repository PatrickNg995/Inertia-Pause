using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Pause all objects after a delay, & unpause objects on input
public class TimePauseUnpause : MonoBehaviour
{
    // For player inputs
    private PlayerActions inputActions;
    private InputAction timePause;

    // Add start delay to time pause
    public float timePauseDelay;
    private float timePauseDelayTimer;

    // Bool to only pause / unpause once
    private bool hasPaused = false;
    private bool hasUnpaused = false;

    IPausable[] pausableObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Bind time pause input to function
        inputActions = new PlayerActions();
        timePause = inputActions.Ingame.TimePause;
    }
    private void Start()
    {
        // make sure it is false
        hasPaused = false;
        hasUnpaused = false;
        timePauseDelayTimer = timePauseDelay;
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
        if (!hasPaused)
        {
            // Run timer & pause time if timer <= 0
            timePauseDelayTimer -= Time.deltaTime;
            if (timePauseDelayTimer < 0)
            {
                hasPaused = true;
                PauseObjects();
            }
        }
    }

    // Get all pausable objects & pause them
    private void PauseObjects()
    {
        MonoBehaviour[] allObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        pausableObjects = allObjects.OfType<IPausable>().ToArray();

        foreach (IPausable pausable in pausableObjects)
        {
            pausable.Pause();
        }
    }

    // Unpause time if time has been paused
    private void checkUnpause(InputAction.CallbackContext context)
    {
        if (hasPaused && !hasUnpaused)
        {
            hasUnpaused = true;
            foreach (IPausable pausable in pausableObjects)
            {
                pausable.Unpause();
            }
        }
    }
}
