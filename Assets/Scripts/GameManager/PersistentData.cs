using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    /// <summary>
    /// List of ActionCommands from the previous attempt to complete a level.
    /// </summary>
    [SerializeField] public List<ActionCommand> PreviousAttemptCommandList { get; set; } = new List<ActionCommand>();

    /// <summary>
    /// Whether the game should load the previous attempt's commands upon starting a level.
    /// </summary>
    public bool WillLoadPreviousAttempt { get; set; } = false;

    /// <summary>
    /// Singleton instance of PersistentData used to access data passed between scenes.
    /// </summary>
    public static PersistentData Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
