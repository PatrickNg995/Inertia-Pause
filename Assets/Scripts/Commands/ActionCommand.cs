// Abstract base class for all commands that count as Actions in-game.
using UnityEngine;

public abstract class ActionCommand : ICommand
{
    /// <summary>
    /// The Interaction Object that this command affects.
    /// </summary>
    public InteractionObject ActionObject { get; protected set; }

    /// <summary>
    /// Name of the Interaction Object that this command affects. Used for identifying the object
    /// for reloading a previous attempt's commands.
    /// </summary>
    public string ObjectNameID { get; protected set; }

    /// <summary>
    /// Whether the command will count as an action or not, true by default.
    /// </summary>
    public bool WillCountAsAction { get; protected set; } = true;

    /// <summary>
    /// Used when reloading a previous attempt's commands.
    /// Relink the ActionObject reference by finding the object in the scene using ObjectNameID.
    /// Derived classes should override this method to also update any other references as needed.
    /// </summary>
    public virtual void RelinkActionObjectReference()
    {
        ActionObject = GameObject.Find(ObjectNameID).GetComponent<InteractionObject>();
    }

    /// <summary>
    /// Execute the command; should mark the InteractionObject as having taken an action.
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// Undo the command; should mark the InteractionObject as no longer having taken an action.
    /// </summary>
    public abstract void Undo();
}
