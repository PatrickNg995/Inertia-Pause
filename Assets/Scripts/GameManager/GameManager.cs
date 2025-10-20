using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Player Information")]
    [SerializeField] private PlayerInteract PlayerInteract;

    [field: Header("Scenario Information")]
    [field: SerializeField] public ScenarioInfo ScenarioInfo { get; private set; }

    /// <summary>
    /// Invoked when the level starts.
    /// </summary>
    public Action OnLevelStart;

    /// <summary>
    /// Invoked when the action count is updated.
    /// </summary>
    public Action<int> OnActionUpdate;

    /// <summary>
    /// Invoked when a redo becomes available (after an undo).
    /// </summary>
    public Action OnRedoAvailable;

    /// <summary>
    /// Invoked when no redos are available (after a redo).
    /// </summary>
    public Action OnRedoUnavailable;

    /// <summary>
    /// Invoked when the player resumes the game after pausing it.
    /// </summary>
    public Action OnGameResume;

    /// <summary>
    /// Invoked when the player pauses the game.
    /// </summary>
    public Action OnGamePause;

    /// <summary>
    /// Invoked when the level is completed.
    /// </summary>
    public Action<LevelResults> OnLevelComplete;

    // Count of actions taken in the current level.
    private int _actionCount = 0;

    // Stack holding the state of objects prior to an action.
    private Stack<ObjectState> _undoStack = new Stack<ObjectState>();

    // Stack holding the state of objects prior to an undo.
    private Stack<ObjectState> _redoStack = new Stack<ObjectState>();

    // Lists of enemies and allies in the scene.
    private List<GameObject> _listOfEnemies = new List<GameObject>();
    private List<GameObject> _listOfAllies = new List<GameObject>();

    // For player input actions.
    private PlayerActions _inputActions;
    private InputAction _undo;
    private InputAction _redo;
    private InputAction _unpause;
    private InputAction _pauseMenu;

    // Whether the level has been won, for future use.
    public bool LevelWon { get; private set; } = false;

    /// <summary>
    /// Number of actions taken in the current level.
    /// </summary>
    public int ActionCount => _actionCount;

    private void Awake()
    {
        // Set up input actions.
        _inputActions = new PlayerActions();
        _undo = _inputActions.Ingame.Undo;
        _redo = _inputActions.Ingame.Redo;
        _unpause = _inputActions.Ingame.TimePause;
        _pauseMenu = _inputActions.Ingame.PauseMenu;
    }

    private void OnEnable()
    {
        _undo.performed += Undo;
        _redo.performed += Redo;
        _unpause.performed += CheckVictoryCondition;
        _pauseMenu.performed += PauseMenu;

        _undo.Enable();
        _redo.Enable();
        _unpause.Enable();
        _pauseMenu.Enable();
    }

    private void OnDisable()
    {
        _undo.performed -= Undo;
        _redo.performed -= Redo;
        _unpause.performed -= CheckVictoryCondition;
        _pauseMenu.performed -= PauseMenu;

        _undo.Disable();
        _redo.Disable();
        _unpause.Disable();
        _pauseMenu.Disable();
    }

    private void Start()
    {
        // Subscribe to player interact action event.
        PlayerInteract.OnActionTaken += HandlePlayerInteractAction;

        // Get list of enemies and allies in the scene for use in determining victory.
        _listOfEnemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        _listOfAllies.AddRange(GameObject.FindGameObjectsWithTag("Ally"));

        // Level start called immediately, though should be after opening cut scene in final game.
        OnLevelStart?.Invoke();
    }

    private void CheckVictoryCondition(InputAction.CallbackContext context)
    {
        // Delay checking for victory to let objects interact first.
        // TODO: Should make this to check after an ending camera pan around or something.
        Invoke("TempDelayedCheckVictory", 3f);
    }

    private void TempDelayedCheckVictory()
    {
        // The following logic should be in CheckVictoryCondition when properly implemented.
        int enemiesAlive = GetNumNPCsAlive(_listOfEnemies);
        int alliesAlive = GetNumNPCsAlive(_listOfAllies);

        // Check if there are no enemies alive and all allies are alive.
        if (enemiesAlive == 0 && alliesAlive == _listOfAllies.Count)
        {
            LevelWon = true;
            Debug.Log("Level won!");
        }
        else
        {
            Debug.Log("Level lost!");
        }

        LevelResults results = new()
        {
            CiviliansRescued = alliesAlive,
            AlliesSaved = 0,
            EnemiesKilled = _listOfEnemies.Count - enemiesAlive,
            // TODO: Optional objectives.
            OptionalObjectivesComplete = new bool[2] { true, false },
            ActionsTaken = _actionCount
        };

        // Call level complete after determining victory.
        OnLevelComplete?.Invoke(results);
    }

    private int GetNumNPCsAlive(List<GameObject> listOfNPCs)
    {
        int numAlive = 0;
        foreach (GameObject npc in listOfNPCs)
        {
            if (npc.GetComponent<NPC>().IsAlive)
            {
                numAlive++;
            }
        }
        return numAlive;
    }

    public void RecordAction(GameObject interactObject)
    {
        // Record the current state before an action is performed.
        ObjectState currentState = new ObjectState(interactObject, interactObject.transform.position, interactObject.transform.rotation);
        _undoStack.Push(currentState);

        // Clear the redo stack since a new action has been performed.
        _redoStack.Clear();
        OnRedoUnavailable?.Invoke();

        // Update action count.
        _actionCount++;
        OnActionUpdate?.Invoke(_actionCount);

        Debug.Log("Action recorded. Total actions: " + _actionCount);
    }

    public void Undo(InputAction.CallbackContext context)
    {
        if (_undoStack.Count > 0)
        {
            // Load the last object state from the undo stack and push the current state onto the redo stack.
            ObjectState objectState = GetObjectStateFromStacks(_undoStack, _redoStack);

            // Use the ResetInteract from InteractionObject to perform the undo.
            objectState.Object.GetComponent<InteractionObject>().OnResetInteract();

            // Decrement action count.
            _actionCount--;
            OnActionUpdate?.Invoke(_actionCount);

            // Notify if redo is now available.
            if (_redoStack.Count == 1)
            {
                OnRedoAvailable?.Invoke();
            }
            Debug.Log("Action undone. Total actions: " + _actionCount);
        }
    }

    public void Redo(InputAction.CallbackContext context)
    {
        if (_redoStack.Count > 0)
        {
            // Load the last object state from the redo stack and push the current state onto the undo stack.
            ObjectState objectState = GetObjectStateFromStacks(_redoStack, _undoStack);

            // Load the saved object state to perform the redo.
            objectState.LoadObjectState();

            // Can use OnInteract to perform the redo instead, better in the case of the shelf but wouldn't work for
            // moving a bullet back to where you moved it before.
            //objectState.Object.GetComponent<InteractionObject>().OnInteract();

            // Increment action count.
            _actionCount++;
            OnActionUpdate?.Invoke(_actionCount);

            // Notify if no more redos are available.
            if (_redoStack.Count == 0)
            {
                OnRedoUnavailable?.Invoke();
            }
            Debug.Log("Action redone. Total actions: " + _actionCount);
        }
    }

    /// <summary>
    /// Loads an object state from the load stack and pushes the current state of that object onto the push stack.
    /// </summary>
    /// <param name="loadStack">The stack to load an ObjectState from.</param>
    /// <param name="pushStack">The stack to push the current ObjectState of the loaded object to.</param>
    /// <returns>The popped ObjectState from the loadStack</returns>
    public ObjectState GetObjectStateFromStacks(Stack<ObjectState> loadStack, Stack<ObjectState> pushStack)
    {
        // Get the object state to load.
        ObjectState loadObjectState = loadStack.Pop();
        GameObject gameObject = loadObjectState.Object;

        // Save the current state before loading, push onto the push stack.
        ObjectState currentState = new ObjectState(gameObject, gameObject.transform.position, gameObject.transform.rotation);
        pushStack.Push(currentState);

        // Load the saved state.
        return loadObjectState;
    }

    private void HandlePlayerInteractAction(GameObject interactObject)
    {
        RecordAction(interactObject);
    }

    public void ResumeFromPauseMenu()
    {
        _inputActions.Enable();
        Cursor.lockState = CursorLockMode.Locked;

        OnGameResume?.Invoke();
    }

    private void PauseMenu(InputAction.CallbackContext context)
    {
        _inputActions.Disable();
        Cursor.lockState = CursorLockMode.None;

        OnGamePause?.Invoke();
    }
}

