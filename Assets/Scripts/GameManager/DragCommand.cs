using UnityEngine;

public class DragCommand : ActionCommand
{
    private Transform _transform;
    private Transform _playerCamera;
    private Vector3 _initialPosition;
    private Vector3 _finalPosition;

    // to allow for a reset command to be made
    public DragCommand(InteractionObject interactionObject, Vector3 initialPosition, Vector3 finalPosition, bool willCountAsAction)
    {
        ActionObject = interactionObject;
        _transform = interactionObject.transform;
        _initialPosition = initialPosition;
        _finalPosition = finalPosition;
        WillCountAsAction = willCountAsAction;
    }

    public override void Execute()
    {
        _transform.position = _finalPosition;
    }

    public override void Undo()
    {
        _transform.position = _initialPosition;
    }
}