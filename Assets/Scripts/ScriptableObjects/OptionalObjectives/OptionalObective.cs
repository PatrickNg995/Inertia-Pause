using UnityEngine;

// Abstract base class for optional objectives in the game.
public abstract class OptionalObective : ScriptableObject
{
    // Description of the objective.
    public string Description;

    // Indicates whether the objective has been completed this level.
    public bool IsCompleted { get; protected set; }

    // Method to check if the objective has been completed.
    // Implementation should set IsCompleted to true if the objective is met.
    public abstract bool CheckCompletion();
}
