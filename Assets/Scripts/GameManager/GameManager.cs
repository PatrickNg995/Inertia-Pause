using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Player Interact")]
    [SerializeField] private PlayerInteract _playerInteract;

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

    // Count of actions taken in the current level.
    private int _actionCount = 0;

    // Stack holding the state of objects prior to an action.
    private Stack<ICommand> _undoStack = new Stack<ICommand>();

    // Stack holding the state of objects prior to an undo.
    private Stack<ICommand> _redoStack = new Stack<ICommand>();

    // Lists of enemies and allies in the scene.
    private List<GameObject> _listOfEnemies = new List<GameObject>();
    private List<GameObject> _listOfAllies = new List<GameObject>();

    // For player input actions.
    private PlayerActions _inputActions;
    private InputAction _undo;
    private InputAction _redo;
    private InputAction _pauseMenu;

    // Whether the level has been won, for future use.
    public bool LevelWon { get; private set; } = false;

    /// <summary>
    /// Number of actions taken in the current level.
    /// </summary>
    public int ActionCount => _actionCount;

    // Make GameManager a singleton.
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Destroy if duplicate GameManager.
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        // Set up input actions.
        _inputActions = new PlayerActions();
        _undo = _inputActions.Ingame.Undo;
        _redo = _inputActions.Ingame.Redo;
        _pauseMenu = _inputActions.Ingame.PauseMenu;
    }

    private void OnEnable()
    {
        _undo.performed += Undo;
        _redo.performed += Redo;
        _pauseMenu.performed += PauseMenu;

        _undo.Enable();
        _redo.Enable();
        _pauseMenu.Enable();
    }

    private void OnDisable()
    {
        _undo.performed -= Undo;
        _redo.performed -= Redo;
        _pauseMenu.performed -= PauseMenu;

        _undo.Disable();
        _redo.Disable();
        _pauseMenu.Disable();
    }

    private void Start()
    {
        // Get list of enemies and allies in the scene for use in determining victory.
        _listOfEnemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        _listOfAllies.AddRange(GameObject.FindGameObjectsWithTag("Ally"));

        // Level start called immediately, though should be after opening cut scene in final game.
        OnLevelStart?.Invoke();
    }

    private void Update()
    {
        // All the code in this method is temporary for testing.

        // Press space to check victory.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckVictoryCondition();
        }
    }

    public void CheckVictoryCondition()
    {
        // Check if level hasn't been won yet and all enemies are dead and no allies are dead.
        if (CheckNPCsDead(_listOfEnemies) && !CheckNPCsDead(_listOfAllies))
        {
            LevelWon = true;
            Debug.Log("Level won!");
        }
        else
        {
            Debug.Log("Level lost!");
        }
    }
    public void RecordAndExecuteCommand(ICommand command)
    {
        // Execute the command.
        command.Execute();

        // Push the action object's command onto the undo stack.
        _undoStack.Push(command);

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
            // Pop the next command to undo from the undo stack and push it onto the redo stack.
            ICommand undoCommand = _undoStack.Pop();

            // Undo the command and update stacks.
            UndoAndPushToRedoStack(undoCommand);

            Debug.Log("Action undone. Total actions: " + _actionCount);
        }
    }

    /// <summary>
    /// Undo a specific command, picking it out from the undo stack.
    /// Used for resetting interactions outside of the normal undo/redo order.
    /// </summary>
    /// <param name="command">The command to be undone.</param>
    public void UndoSpecificCommand(ICommand command)
    {
        Stack<ICommand> tempStack = new Stack<ICommand>();

        while (_undoStack.Count > 0)
        {
            // Pop commands until we find the specific command to undo.
            ICommand poppedCommand = _undoStack.Pop();
            if (!poppedCommand.Equals(command))
            {
                tempStack.Push(poppedCommand);
            }
            else
            {
                // Found the specific command, perform the undo and update stacks.
                UndoAndPushToRedoStack(poppedCommand);
                Debug.Log("Specific action undone. Total actions: " + _actionCount);
            }
        }

        // Restore the other commands back to the undo stack.
        while (tempStack.Count > 0)
        {
            _undoStack.Push(tempStack.Pop());
        }
    }

    public void Redo(InputAction.CallbackContext context)
    {
        if (_redoStack.Count > 0)
        {
            // Pop the next command to redo from the redo stack and push it onto the undo stack.
            ICommand redoCommand = _redoStack.Pop();
            _undoStack.Push(redoCommand);

            // Use Execute() from ICommand to perform the redo.
            redoCommand.Execute();

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
    /// Helper function to undo a command and push it to the redo stack, and update action count.
    /// </summary>
    /// <param name="command">Command to undo.</param>
    private void UndoAndPushToRedoStack(ICommand command)
    {
        // Use Undo() from ICommand to perform the undo.
        command.Undo();

        // Push the command onto the redo stack.
        _redoStack.Push(command);

        // Decrement action count.
        _actionCount--;
        OnActionUpdate?.Invoke(_actionCount);

        // Notify if redo is now available.
        if (_redoStack.Count == 1)
        {
            OnRedoAvailable?.Invoke();
        }
    }

    private bool CheckNPCsDead(List<GameObject> listOfNPCs)
    {
        foreach (GameObject npc in listOfNPCs)
        {
            if (npc.GetComponent<NPC>().IsAlive)
            {
                return false;
            }
        }
        return true;
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

