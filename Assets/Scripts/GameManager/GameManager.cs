using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Time Pausing")]
    [SerializeField] private TimePauseUnpause _timePauseUnpause;

    [Header("NPC Lists")]
    [SerializeField] private GameObject _enemies;
    [SerializeField] private GameObject _allies;
    [SerializeField] private GameObject _civilians;

    [Header("Player")]
    [SerializeField] private GameObject _player;
    [SerializeField] private PlayerInteract _playerInteract;
    [SerializeField] private CharacterController _playerController;

    // Toggle disabling player interaction after pausing; may be useful for testing.
    [SerializeField] private bool _isInputDisabledAfterLevelComplete = true;

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
    /// Invoked when an undo becomes available (after any action).
    /// </summary>
    public Action OnUndoAvailable;

    /// <summary>
    /// Invoked when no undos are available (after undoing all actions).
    /// </summary>
    public Action OnUndoUnavailable;

    /// <summary>
    /// Invoked when a redo becomes available (after an undo).
    /// </summary>
    public Action OnRedoAvailable;

    /// <summary>
    /// Invoked when no redos are available (after a redo).
    /// </summary>
    public Action OnRedoUnavailable;

    /// <summary>
    /// Invoked when the player opens any blocking menu (pause, tutorial, etc).
    /// </summary>
    public Action OnAnyBlockingMenuOpen;

    /// <summary>
    /// Invoked when the player returns to the game after a blocking menu.
    /// </summary>
    public Action OnAnyBlockingMenuClose;

    /// <summary>
    /// Invoked when the player pauses the game.
    /// </summary>
    public Action OnPauseMenuOpen;

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
    private List<GameObject> _listOfCivilians = new List<GameObject>();

    // For player input actions.
    private PlayerActions _inputActions;
    private InputAction _undo;
    private InputAction _redo;
    private InputAction _unpause;
    private InputAction _pauseMenu;

    // Store initial player position and rotation for resetting level.
    private Vector3 _initialPlayerPosition;
    private Quaternion _initialPlayerRotation;

    /// <summary>
    /// Whether the level has been won.
    /// </summary>
    public bool LevelWon { get; private set; } = false;

    /// <summary>
    /// Number of actions taken in the current level.
    /// </summary>
    public int ActionCount => _actionCount;

    /// <summary>
    /// Instance of the GameManager singleton used to access GameManager functionality.
    /// </summary>
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

        // Store initial player position and rotation for resetting level.
        _initialPlayerPosition = _player.transform.position;
        _initialPlayerRotation = _player.transform.rotation;

        // Get list of enemies and allies in the scene for use in determining victory.
        _listOfEnemies = GetDirectChildrenOfObject(_enemies);
        _listOfAllies = GetDirectChildrenOfObject(_allies);
        _listOfCivilians = GetDirectChildrenOfObject(_civilians);

        // Set up input actions.
        _inputActions = new PlayerActions();
        _undo = _inputActions.Ingame.Undo;
        _redo = _inputActions.Ingame.Redo;
        _unpause = _inputActions.Ingame.TimePause;
        _pauseMenu = _inputActions.Ingame.PauseMenu;
    }

    private void Update()
    {
        // For testing reset level before unpause with L key.
        // Will remove after PR is approved.
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            RewindLevel();

            // Bootleg measure to re-enable input after rewinding for testing.
            AnyBlockingMenuClosed();
            GameObject.Find("ResultMenu").SetActive(false);
        }
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
        // Level start called immediately, though should be after opening cut scene in final game.
        OnLevelStart?.Invoke();
    }

    private List<GameObject> GetDirectChildrenOfObject(GameObject parentObject)
    {
        List<GameObject> childList = new List<GameObject>();

        foreach (Transform childTransform in parentObject.transform)
        {
            childList.Add(childTransform.gameObject);
        }

        return childList;
    }

    public void RewindLevel()
    {
        // Reset player position and rotation.
        _playerController.enabled = false;
        _player.transform.position = _initialPlayerPosition;
        _player.transform.rotation = _initialPlayerRotation;
        _playerController.enabled = true;

        // Reset all object states to before unpause, then pause objects again.
        _timePauseUnpause.ResetAllObjectStatesBeforeUnpause();
        _timePauseUnpause.PauseAllObjects();

        // Re-enable player interaction.
        _playerInteract.enabled = true;

        // Re-enable undo/redo/unpause inputs.
        _undo.Enable();
        _redo.Enable();
        _unpause.Enable();

        // Re-enable collisions on the player.
        _playerController.detectCollisions = false;
    }

    private void CheckVictoryCondition(InputAction.CallbackContext context)
    {
        if (_isInputDisabledAfterLevelComplete)
        {
            // Prevent players from interacting with objects after unpausing.
            _playerInteract.enabled = false;
            Debug.Log("Player Interact disabled");

            // Disable undo/redo/unpause inputs.
            _undo.Disable();
            _redo.Disable();
            _unpause.Disable();

            // Disable collisions on the player.
            _playerController.detectCollisions = false;
        }

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
        int civiliansAlive = GetNumNPCsAlive(_listOfCivilians);
        Debug.Log($"Enemies alive: {enemiesAlive}, Allies alive: {alliesAlive}, Civilians alive: {civiliansAlive}");

        // Check if there are no enemies alive and all allies and civilians are alive.
        if (enemiesAlive == 0 && (alliesAlive + civiliansAlive) == (_listOfAllies.Count + _listOfCivilians.Count))
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
            CiviliansRescued = civiliansAlive,
            AlliesSaved = alliesAlive,
            EnemiesKilled = _listOfEnemies.Count - enemiesAlive,
            // TODO: Optional objectives.
            OptionalObjectivesComplete = new bool[2] { true, false },
            ActionsTaken = _actionCount
        };

        // Call level complete after determining victory.
        OnLevelComplete?.Invoke(results);

        // Allow player to click on results screen buttons.
        Cursor.lockState = CursorLockMode.None;
    }
    private int GetNumNPCsAlive(List<GameObject> listOfNPCs)
    {
        int numAlive = 0;
        foreach (GameObject gameObject in listOfNPCs)
        {
            NPC npc = gameObject.GetComponent<NPC>();

            // If NPC component is not on the parent object, check children.
            if (npc == null)
            {
                npc = gameObject.GetComponentInChildren<NPC>();
            }

            if (npc.IsAlive)
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

        // Notify that undo is now available.
        OnUndoAvailable?.Invoke();

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

        // Notify if there are no more actions to undo.
        if (_undoCommandList.Count == 0)
        {
            OnUndoUnavailable?.Invoke();
        }

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

            // Notify if there are no more actions to undo.
            if (_undoCommandList.Count == 0)
            {
                OnUndoUnavailable?.Invoke();
            }

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

        // Notify if there are no more actions to undo.
        if (_undoCommandList.Count == 0)
        {
            OnUndoUnavailable?.Invoke();
        }

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

            // Notify that undo is now available.
            OnUndoAvailable?.Invoke();

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

    public void AnyBlockingMenuOpened()
    {
        _inputActions.Disable();
        Cursor.lockState = CursorLockMode.None;

        OnAnyBlockingMenuOpen?.Invoke();
    }

    public void AnyBlockingMenuClosed()
    {
        _inputActions.Enable();
        Cursor.lockState = CursorLockMode.Locked;

        OnAnyBlockingMenuClose?.Invoke();
    }

    private void PauseMenu(InputAction.CallbackContext context)
    {
        OnPauseMenuOpen?.Invoke();
        AnyBlockingMenuOpened();
    }
}

