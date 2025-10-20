using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Player Interact")]
    [SerializeField] private PlayerInteract _playerInteract;

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
    private List<ActionCommand> _undoCommandList = new List<ActionCommand>();

    // Stack holding the state of objects prior to an undo.
    private List<ActionCommand> _redoCommandList = new List<ActionCommand>();

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
        Invoke(nameof(TempDelayedCheckVictory), 3f);
        Debug.Log("Checking victory after short delay...");
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

    public void RecordAndExecuteCommand(ActionCommand command)
    {
        // Execute the command.
        command.Execute();

        // Add the command to the undo list.
        _undoCommandList.Add(command);

        // Clear the redo list since a new action has been performed.
        _redoCommandList.Clear();
        OnRedoUnavailable?.Invoke();

        if (command.WillCountAsAction)
        {
            // Update action count.
            _actionCount++;
            OnActionUpdate?.Invoke(_actionCount);
        }

        Debug.Log("Action recorded. Total actions: " + _actionCount);
    }

    public void ResetObjectCommands(InteractionObject interactionObject, ActionCommand redoCommand)
    {
        // Remove all the commands of the reset object from both lists.
        RemoveCommandsOfObjectFromLists(interactionObject);

        // Add the redo command to the redo list.
        _redoCommandList.Add(redoCommand);

        // Notify if redo is now available.
        if (_redoCommandList.Count == 1)
        {
            OnRedoAvailable?.Invoke();
        }
    }

    public void Undo(InputAction.CallbackContext context)
    {
        if (_undoCommandList.Count > 0)
        {
            // Pop the next command to undo from the undo list and add it to the redo list.
            ActionCommand undoCommand = PopList(_undoCommandList);

            // Undo the command and add to redo list.
            UndoAndAddToRedoList(undoCommand);

            Debug.Log("Action undone. Total actions: " + _actionCount);
        }
    }

    public void UndoSpecificCommand(ActionCommand command)
    {
        int removeIndex = _undoCommandList.IndexOf(command);

        // Undo the specific command and add to redo list.
        UndoAndAddToRedoList(_undoCommandList[removeIndex]);

        // Remove Command from undo list.
        _undoCommandList.RemoveAt(removeIndex);

        Debug.Log("Specific action undone. Total actions: " + _actionCount);
    }

    public void Redo(InputAction.CallbackContext context)
    {
        if (_redoCommandList.Count > 0)
        {
            // Pop the next command to redo from the redo list.
            ActionCommand redoCommand = PopList(_redoCommandList);

            // Add command to the undo list.
            _undoCommandList.Add(redoCommand);

            // Use Execute() from ICommand to perform the redo.
            redoCommand.Execute();

            if (redoCommand.WillCountAsAction)
            {
                // Increment action count.
                _actionCount++;
                OnActionUpdate?.Invoke(_actionCount);
            }

            // Notify if no more redos are available.
            if (_redoCommandList.Count == 0)
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
    private void UndoAndAddToRedoList(ActionCommand command)
    {
        // Use Undo() from ICommand to perform the undo.
        command.Undo();

        // Add the command to the redo list.
        _redoCommandList.Add(command);

        if (command.WillCountAsAction)
        {
            // Decrement action count.
            _actionCount--;
            OnActionUpdate?.Invoke(_actionCount);
        }

        // Notify if redo is now available.
        if (_redoCommandList.Count == 1)
        {
            OnRedoAvailable?.Invoke();
        }
    }

    /// <summary>
    /// Removes all the commands in both the undo and redo lists that affect a specified object.
    /// </summary>
    /// <param name="interactionObject">The object whose commands will be removed.</param>
    private void RemoveCommandsOfObjectFromLists(InteractionObject interactionObject)
    {
        // Remove all the commands of the object from both the undo and redo lists.
        _undoCommandList.RemoveAll(obj => obj.ActionObject == interactionObject);
        _redoCommandList.RemoveAll(obj => obj.ActionObject == interactionObject);

        // Decrement action count.
        _actionCount--;
        OnActionUpdate?.Invoke(_actionCount);

        // Notify if no more redos are available.
        if (_redoCommandList.Count == 0)
        {
            OnRedoUnavailable?.Invoke();
        }
        Debug.Log("Commands of an object removed. Total actions: " + _actionCount);
    }

    private ActionCommand PopList(List<ActionCommand> list)
    {
        int lastIndex = list.Count - 1;

        ActionCommand command = list[lastIndex];
        list.RemoveAt(lastIndex);

        return command;
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

