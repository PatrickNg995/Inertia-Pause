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

    // Whether the level has been won, for future use.
    public bool LevelWon { get; private set; } = false;

    private void Awake()
    {
        // Set up input actions.
        _inputActions = new PlayerActions();
        _undo = _inputActions.Ingame.Undo;
        _redo = _inputActions.Ingame.Redo;
    }

    private void OnEnable()
    {
        _undo.performed += Undo;
        _redo.performed += Redo;
        _undo.Enable();
        _redo.Enable();
    }

    private void OnDisable()
    {
        _undo.performed -= Undo;
        _redo.performed -= Redo;
        _undo.Disable();
        _redo.Disable();
    }

    private void Start()
    {
        // Subscribe to player interact action event.
        _playerInteract.OnActionTaken += HandlePlayerActionTaken;

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

    public void Undo(InputAction.CallbackContext context)
    {
        if (_undoStack.Count > 0)
        {
            // Pop the next command to undo from the undo stack and push it onto the redo stack.
            ICommand undoObject = _undoStack.Pop();
            _redoStack.Push(undoObject);

            // Use Undo() from ICommand to perform the undo.
            undoObject.Undo();

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
            // Pop the next command to redo from the redo stack and push it onto the undo stack.
            ICommand redoObject = _redoStack.Pop();
            _undoStack.Push(redoObject);

            // Use Execute() from ICommand to perform the redo.
            redoObject.Execute();

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

    private void HandlePlayerActionTaken(GameObject actionObject)
    {
        // Push the action object's command onto the undo stack.
        _undoStack.Push(actionObject.GetComponent<ICommand>());

        // Clear the redo stack since a new action has been performed.
        _redoStack.Clear();
        OnRedoUnavailable?.Invoke();

        // Update action count.
        _actionCount++;
        OnActionUpdate?.Invoke(_actionCount);

        Debug.Log("Action recorded. Total actions: " + _actionCount);
    }
}

