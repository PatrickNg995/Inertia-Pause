using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimePauseUnpause : MonoBehaviour
{
    // For player inputs.
    private PlayerActions _inputActions;
    private InputAction _timePause;

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

    public void UnpauseAllObjects()
    {
        foreach (IPausable pausable in _pausableObjects)
        {
            pausable.Unpause();
        }
    }

    public void ResetAllObjectStatesBeforeUnpause()
    {
        foreach (IPausable pausable in _pausableObjects)
        {
            pausable.ResetStateBeforeUnpause();
        }
    }

    public void EnableTimePause()
    {
        _timePause.Enable();
    }

    // Unpause time if time has been paused.
    private void CheckUnpause(InputAction.CallbackContext context)
    {
        _timePause.Disable();
        UnpauseAllObjects();
        StartCoroutine(GameManager.Instance.EndLevel());
    }
}
