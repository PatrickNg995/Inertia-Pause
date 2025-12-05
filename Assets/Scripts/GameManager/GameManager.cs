using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimePauseUnpause _timePauseUnpause;
    [SerializeField] private SavedLevelProgressManager _savedLevelProgressManager;
    [SerializeField] private ReplayCameraManager _replayCameraManager;

    [Header("NPC Lists")]
    [SerializeField] private GameObject _enemies;
    [SerializeField] private GameObject _allies;
    [SerializeField] private GameObject _civilians;

    [Header("Player")]
    [SerializeField] private NewPlayerMovement _playerMovement;
    [SerializeField] private PlayerInteract _playerInteract;
    [SerializeField] private CharacterController _playerController;

    // Toggle disabling player interaction after pausing; may be useful for testing.
    [SerializeField] private bool _isInputDisabledAfterLevelComplete = true;

    [field: Header("Scenario Information")]
    [field: SerializeField] public ScenarioInfo ScenarioInfo { get; private set; }

    /// <summary>
    /// Invoked when the level starts for the first time.
    /// </summary>
    public Action OnLevelStart;

    /// <summary>
    /// Invoked every time the level is rewinded in the results screen.
    /// </summary>
    public Action OnLevelRewind;

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

    // Lists of enemies and allies gameObjects in the scene.
    private List<GameObject> _listOfEnemiesObjects = new List<GameObject>();
    private List<GameObject> _listOfAlliesObjects = new List<GameObject>();
    private List<GameObject> _listOfCiviliansObjects = new List<GameObject>();

    // List of animation Scripts
    private List<AllyAnimationScript> _allyAnimationScripts = new List<AllyAnimationScript>();
    private List<CivilianAnimationScript> _civilianAnimationScripts = new List<CivilianAnimationScript>();

    // For player input actions.
    private PlayerActions _inputActions;
    private InputAction _undo;
    private InputAction _redo;
    private InputAction _pauseMenu;

    // Command manager that handles logic for undo/redo and action count.
    private CommandManager _commandManager;

    // Lists of NPCs in the scene.
    private List<NPC> _listOfEnemies = new List<NPC>();
    private List<NPC> _listOfAllies = new List<NPC>();
    private List<NPC> _listOfCivilians = new List<NPC>();

    private float _openingCutsceneDuration = 2f;

    /// <summary>
    /// Whether the level has been won.
    /// </summary>
    public bool LevelWon { get; private set; } = false;

    /// <summary>
    /// Number of actions taken in the current level.
    /// </summary>
    public int ActionCount => _commandManager?.ActionCount ?? 0;

    /// <summary>
    /// List holding the actions performed by the player that can be undone.
    /// </summary>
    public List<ActionCommand> UndoCommandList => _commandManager?.UndoCommandList;

    /// <summary>
    /// // List holding the actions that have been undone and can be redone.
    /// </summary>
    public List<ActionCommand> RedoCommandList => _commandManager?.RedoCommandList;

    // List of causes of death for NPCs.
    public List<GameObject> ListOfCausesOfDeath { get; private set; } = new List<GameObject>();

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

        // Create command manager and wire events.
        _commandManager = new CommandManager();
        _commandManager.OnActionUpdate += count => OnActionUpdate?.Invoke(count);
        _commandManager.OnUndoAvailable += () => OnUndoAvailable?.Invoke();
        _commandManager.OnUndoUnavailable += () => OnUndoUnavailable?.Invoke();
        _commandManager.OnRedoAvailable += () => OnRedoAvailable?.Invoke();
        _commandManager.OnRedoUnavailable += () => OnRedoUnavailable?.Invoke();

        // Get list of enemies and allies as gameObjects in the scene.
        _listOfEnemiesObjects = GetDirectChildrenOfObject(_enemies);
        _listOfAlliesObjects = GetDirectChildrenOfObject(_allies);
        _listOfCiviliansObjects = GetDirectChildrenOfObject(_civilians);

        // Get list of NPC components from the gameObjects.
        _listOfEnemies = GetComponentsFromObjects<NPC>(_listOfEnemiesObjects);
        _listOfAllies = GetComponentsFromObjects<NPC>(_listOfAlliesObjects);
        _listOfCivilians = GetComponentsFromObjects<NPC>(_listOfCiviliansObjects);

        // Get list of Animation Scripts from the gameObjects.
        _allyAnimationScripts = GetComponentsFromObjects<AllyAnimationScript>(_listOfAlliesObjects);
        _civilianAnimationScripts = GetComponentsFromObjects<CivilianAnimationScript>(_listOfCiviliansObjects);

        // Set up input actions.
        _inputActions = new PlayerActions();
        _undo = _inputActions.Ingame.Undo;
        _redo = _inputActions.Ingame.Redo;
        _pauseMenu = _inputActions.Ingame.PauseMenu;
    }

    private List<T> GetComponentsFromObjects<T>(List<GameObject> objects) where T : Component
    {
        List<T> result = new List<T>(objects.Count);

        foreach (GameObject go in objects)
        {
            T component = go.GetComponent<T>();
            if (component == null)
            {
                component = go.GetComponentInChildren<T>();
            }

            if (component != null)
            {
                result.Add(component);
            }
            else
            {
                Debug.LogWarning($"No {typeof(T).Name} found on or under {go.name}");
            }
        }

        return result;
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
        // Play the opening cutscene then start the level.
        StartCoroutine(PlayOpeningCutscene());
        OnLevelStart?.Invoke();

        // Update last level in saved progress.
        _savedLevelProgressManager.UpdateLastLevel(ScenarioInfo.EnvironmentSceneName, ScenarioInfo.ScenarioAssetsSceneName);
    }

    private IEnumerator PlayOpeningCutscene()
    {
        yield return null;
        _timePauseUnpause.SimulateAllPrePauseBehaviours();
        yield return new WaitForSeconds(_openingCutsceneDuration);
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
        // Rewind all objects in the level.
        RewindObjects();

        // Re-enable player inputs and interaction.
        _inputActions.Enable();
        _playerInteract.enabled = true;

        // Re-enable undo/redo/unpause inputs.
        _undo.Enable();
        _redo.Enable();
        EnableTimeUnpause();

        // Re-enable collisions on the player.
        _playerController.detectCollisions = true;

        // Reset replay cameras.
        _replayCameraManager.ResetCameras();

        // Update billboard icons for all NPCs.
        UpdateBillboardIconsInList(_listOfEnemies);
        UpdateBillboardIconsInList(_listOfAllies);
        UpdateBillboardIconsInList(_listOfCivilians);

        // Show objectives again.
        OnLevelRewind?.Invoke();
    }

    public void RewindObjects()
    {
        // Clear causes of death list.
        ListOfCausesOfDeath.Clear();

        // Reset all object states to before unpause, then pause objects again.
        _timePauseUnpause.ResetAllObjectStatesBeforeUnpause();
        _timePauseUnpause.PauseAllObjects();
    }

    public void DisableTimeUnpause()
    {
        _timePauseUnpause.DisableTimeUnpause();
    }

    public void EnableTimeUnpause()
    {
        _timePauseUnpause.EnableTimeUnpause();
    }

    public IEnumerator EndLevel()
    { 
        if (_isInputDisabledAfterLevelComplete)
        {
            // Disable player input actions and interaction.
            _inputActions.Disable();
            _playerInteract.enabled = false;
            Debug.Log("Player Interact disabled");

            // Disable undo/redo/unpause inputs.
            _undo.Disable();
            _redo.Disable();

            // Disable collisions on the player.
            _playerController.detectCollisions = false;
        }

        // Activate replay sequence.
        if (_replayCameraManager != null)
        {
            OnAnyBlockingMenuOpen?.Invoke();
            yield return StartCoroutine(_replayCameraManager.StartReplaySequence());
            OnAnyBlockingMenuClose?.Invoke();
        }
            
        // Check victory condition and end level.
        CheckVictoryCondition();
    }

    private void CheckVictoryCondition()
    {
        // The following logic should be in CheckVictoryCondition when properly implemented.
        int enemiesAlive = GetNumNPCsAlive(_listOfEnemies);
        int alliesAlive = GetNumNPCsAlive(_listOfAllies);
        int civiliansAlive = GetNumNPCsAlive(_listOfCivilians);
        Debug.Log($"Enemies alive: {enemiesAlive}, Allies alive: {alliesAlive}, Civilians alive: {civiliansAlive}");

        // Check if there are no enemies alive and all allies and civilians are alive.
        if (enemiesAlive == 0 && (alliesAlive + civiliansAlive) == (_listOfAlliesObjects.Count + _listOfCiviliansObjects.Count))
        {
            LevelWon = true;

            // Play Each Ally's Relieved Animation.
            foreach (AllyAnimationScript anim in _allyAnimationScripts)
            {
                anim.PlayRelievedAnimation();
            }

            // Play Each Civilians Relieved Animation.
            foreach (CivilianAnimationScript anim in _civilianAnimationScripts)
            {
                anim.PlayRelievedAnimation();
            }
            Debug.Log("Level won!");
        }
        else
        {
            Debug.Log("Level lost!");
        }
        // Evaluate optional objectives.
        bool[] optionalResults = EvaluateOptionalObjectives();

        // Create results struct.
        LevelResults results = new()
        {
            CiviliansRescued = civiliansAlive,
            AlliesSaved = alliesAlive,
            EnemiesKilled = _listOfEnemiesObjects.Count - enemiesAlive,
            OptionalObjectivesComplete = optionalResults,
            ActionsTaken = ActionCount
        };

        // Update saved level progress if level was won.
        if (LevelWon)
        {
            LevelProgressInfo levelInfo = new(ScenarioInfo.EnvironmentSceneName, ScenarioInfo.ScenarioAssetsSceneName, ActionCount, optionalResults);
            _savedLevelProgressManager.UpdateLevelProgress(levelInfo);
            _savedLevelProgressManager.UpdateLastLevel(ScenarioInfo.NextEnvironmentSceneName, ScenarioInfo.NextScenarioAssetsSceneName);
        }

        // Call level complete after determining victory.
        OnLevelComplete?.Invoke(results);

        // Allow player to click on results screen buttons.
        Cursor.lockState = CursorLockMode.None;
    }

    private void UpdateBillboardIconsInList(List<NPC> npcList)
    {
        foreach (NPC npc in npcList)
        {
            npc.UpdateBillboardIconState();
        }
    }

    private int GetNumNPCsAlive(List<NPC> listOfNPCs)
    {
        int numAlive = 0;
        foreach (NPC npc in listOfNPCs)
        {
            if (npc.IsAlive)
            {
                numAlive++;
            }
        }
        return numAlive;
    }

    public void RecordNpcDeath(GameObject cause)
    {
        ListOfCausesOfDeath.Add(cause);
    }

    private bool[] EvaluateOptionalObjectives()
    {
        bool[] optionalResults = Array.Empty<bool>();
        if (ScenarioInfo != null && ScenarioInfo.Objectives.OptionalObjectives != null)
        {
            List<OptionalObjective> optionalObjectives = ScenarioInfo.Objectives.OptionalObjectives;
            optionalResults = new bool[optionalObjectives.Count];

            for (int i = 0; i < optionalObjectives.Count; i++)
            {
                OptionalObjective objective = optionalObjectives[i];

                // Check if each optional objective is completed, and only count it if the level is also won.
                optionalResults[i] = objective.CheckCompletion() && LevelWon;
            }
        }
        return optionalResults;
    }

    public void RecordAndExecuteCommand(ActionCommand command)
    {
        _commandManager.RecordAndExecuteCommand(command);
    }

    public void ResetObjectCommands(InteractionObject interactionObject, ActionCommand redoCommand)
    {
        _commandManager.ResetObjectCommands(interactionObject, redoCommand);
    }

    public void Undo(InputAction.CallbackContext context)
    {
        if (_playerInteract.IsInteracting)
        {
            Debug.Log("Undo attempted while interacting with an object, ignoring command.");
            return;
        }

        _commandManager.Undo(context);
    }

    public void UndoSpecificCommand(ActionCommand command)
    {
        _commandManager.UndoSpecificCommand(command);
    }

    public void Redo(InputAction.CallbackContext context)
    {
        if (_playerInteract.IsInteracting)
        {
            Debug.Log("Redo attempted while interacting with an object, ignoring command.");
            return;
        }

        _commandManager.Redo(context);
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
        MusicPlayer.Instance.UnpauseMusic();
    }

    private void PauseMenu(InputAction.CallbackContext context)
    {
        OnPauseMenuOpen?.Invoke();
        AnyBlockingMenuOpened();
        MusicPlayer.Instance.PauseMusic();
    }
}

