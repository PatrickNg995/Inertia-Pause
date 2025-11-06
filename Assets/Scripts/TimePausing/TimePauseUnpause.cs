using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimePauseUnpause : MonoBehaviour
{
    // For player inputs.
    private PlayerActions _inputActions;
    private InputAction _timePause;

    // Bool to only unpause once.
    private bool _hasUnpaused = false;

    private IPausable[] _pausableObjects;

    void Awake()
    {
        // Bind time pause input to function.
        _inputActions = new PlayerActions();
        _timePause = _inputActions.Ingame.TimePause;
    }

    void Start()
    {
        _hasUnpaused = false;

        // Pause all pausable objects.
        MonoBehaviour[] allObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        _pausableObjects = allObjects.OfType<IPausable>().ToArray();

        PauseAllObjects();
    }

    // Enable & disable input actions.
    private void OnEnable()
    {
        _timePause.performed += CheckUnpause;
        _timePause.Enable();
    }
    private void OnDisable()
    {
        _timePause.performed -= CheckUnpause;
        _timePause.Disable();
    }

    public void PauseAllObjects()
    {
        foreach (IPausable pausable in _pausableObjects)
        {
            pausable.Pause();
        }
    }

    public void ResetAllObjectStatesBeforeUnpause()
    {
        _hasUnpaused = false;

        foreach (IPausable pausable in _pausableObjects)
        {
            pausable.ResetStateBeforeUnpause();
        }
    }

    // Unpause time if time has been paused.
    private void CheckUnpause(InputAction.CallbackContext context)
    {
        if (!_hasUnpaused)
        {
            _hasUnpaused = true;
            foreach (IPausable pausable in _pausableObjects)
            {
                pausable.Unpause();
            }
        }
    }
}
