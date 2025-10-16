using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Player Interact")]
    [SerializeField] private PlayerInteract PlayerInteract;

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

    // Whether the level has been won, for future use.
    public bool LevelWon { get; private set; } = false;

    // Make GameManager a singleton.
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern implementation.
        if (Instance != null && Instance != this)
        {
            // Destroy duplicate GameManagers.
            Destroy(gameObject); 
        }
        Instance = this;

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
        PlayerInteract.OnInteractAction += HandlePlayerInteractAction;

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

    public void RecordAction(GameObject interactObject)
    {
        // Record the current state before an action is performed.
        ObjectState currentState = new ObjectState(interactObject, interactObject.transform.position, interactObject.transform.rotation);
        _undoStack.Push(currentState);

        // Clear the redo stack since a new action has been performed.
        _redoStack.Clear();

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
            objectState.LoadObjectState();

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
}

