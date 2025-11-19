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

    private bool _isUnpauseEnabled = true;

    private IPausable[] _pausableObjects;

    void Awake()
    {
        // Bind time pause input to function.
        _inputActions = new PlayerActions();
        _timePause = _inputActions.Ingame.TimePause;
    }

    void Start()
    {
        // Pause all pausable objects.
        MonoBehaviour[] allObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        _pausableObjects = allObjects.OfType<IPausable>().ToArray();

        PauseAllObjects();
    }

    // Enable & disable input actions.
    private void OnEnable()
    {
        _timePause.performed += UnpauseLevel;
        _timePause.Enable();
    }

    private void OnDisable()
    {
        _timePause.performed -= UnpauseLevel;
        _timePause.Disable();
    }

    public void PauseAllObjects()
    {
        foreach (IPausable pausable in _pausableObjects)
        {
            pausable.Pause();
        }
    }

    public void UnpauseAllObjects()
    {
        foreach (IPausable pausable in _pausableObjects)
        {
            pausable.Unpause();
        }
    }

    public void DisableTimeUnpause()
    {
        _isUnpauseEnabled = false;
    }

    public void EnableTimeUnpause()
    {
        _isUnpauseEnabled = true;
    }

    public void ResetAllObjectStatesBeforeUnpause()
    {
        foreach (IPausable pausable in _pausableObjects)
        {
            pausable.ResetStateBeforeUnpause();
        }
    }

    public void EnableTimePauseInput()
    {
        _timePause.Enable();
    }

    private void UnpauseLevel(InputAction.CallbackContext context)
    {
        if (_isUnpauseEnabled && !_hasUnpaused)
        {
            Debug.Log("Time unpause initiated.");
            _hasUnpaused = true;

            // Unpause all pausable objects.
            UnpauseAllObjects();

            // Start end level sequence.
            StartCoroutine(GameManager.Instance.EndLevel());
        }
        else
        {
            // A pop up saying time unpause is disabled might be a good idea.
            Debug.Log("Time unpause attempted but is currently disabled or has already unpaused.");
        }
    }
}
