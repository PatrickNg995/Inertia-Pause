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

    private void UnpauseLevel(InputAction.CallbackContext context)
    {
        // Disable further time pause inputs.
        _timePause.Disable();

        // Unpause all pausable objects.
        UnpauseAllObjects();

        // Start end level sequence.
        StartCoroutine(GameManager.Instance.EndLevel());
    }
}
