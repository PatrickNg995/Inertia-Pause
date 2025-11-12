using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CommandManager
{
    private int _actionCount = 0;

    // List holding the actions performed by the player that can be undone.
    private List<ActionCommand> _undoCommandList = new List<ActionCommand>();

    // List holding the actions that have been undone and can be redone.
    private List<ActionCommand> _redoCommandList = new List<ActionCommand>();

    // Events forwarded to the GameManager.
    public Action<int> OnActionUpdate;
    public Action OnUndoAvailable;
    public Action OnUndoUnavailable;
    public Action OnRedoAvailable;
    public Action OnRedoUnavailable;

    public int ActionCount => _actionCount;

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
        if (redoCommand != null)
        {
            _redoCommandList.Add(redoCommand);
        }

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
}
