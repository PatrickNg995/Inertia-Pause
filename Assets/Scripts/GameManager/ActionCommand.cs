using UnityEngine;

public abstract class ActionCommand : ICommand
{
    /// <summary>
    /// The Interaction Object that this command affects.
    /// </summary>
    public InteractionObject ActionObject { get; protected set; }

    /// <summary>
    /// Whether the command will count as an action or not, true by default.
    /// </summary>
    public bool WillCountAsAction { get; protected set; } = true;

    /// <summary>
    /// Execute the command; should mark the InteractionObject as having taken an action.
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// Undo the command; should mark the InteractionObject as no longer having taken an action.
    /// </summary>
    public abstract void Undo();
}
