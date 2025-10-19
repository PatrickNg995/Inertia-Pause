public interface ICommand
{
    /// <summary>
    /// Execute the command; should mark the InteractionObject as having taken an action.
    /// </summary>
    public void Execute();

    /// <summary>
    /// Undo the command; should mark the InteractionObject as no longer having taken an action.
    /// </summary>
    public void Undo();
}
