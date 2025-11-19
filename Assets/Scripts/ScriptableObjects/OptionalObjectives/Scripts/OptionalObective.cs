using UnityEngine;

// Abstract base class for optional objectives in the game.
public abstract class OptionalObective : ScriptableObject
{
    [Header("Objective Details")]
    [Tooltip("The description of the objective to be displayed in-game.")]
    public string Description;

    // Method to check if the objective has been completed.
    public abstract bool CheckCompletion();
}
