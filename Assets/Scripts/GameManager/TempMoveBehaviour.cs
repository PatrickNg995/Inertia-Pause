using UnityEngine;

// Quick and dirty InteractionObject just to test the MoveCommand.
// Not for actual use in the game.
public class TempMoveBehaviour : InteractionObject
{
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private const float MOVE_DISTANCE = 5f;

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
        IsContinuousUpdate = true;
        _initialPosition = gameObject.transform.position;
        _initialRotation = gameObject.transform.rotation;
    }

    public override void OnCancelInteract()
    {
        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Physics.Raycast(ray, MOVE_DISTANCE, 0, QueryTriggerInteraction.Ignore);

        Vector3 targetPosition = ray.origin + ray.direction.normalized * MOVE_DISTANCE;

        Quaternion targetRotation = transform.rotation;

        // Set up and execute move command.
        // If the object has already taken an action, this command will not count as an action and vice versa.
        ActionCommand = new MoveCommand(this, transform.position, transform.rotation, targetPosition, targetRotation, !HasTakenAction);
        GameManager.Instance.RecordAndExecuteCommand(ActionCommand);
    }

    public override void OnInteract()
    {
        
    }

    public override void OnResetInteract()
    {
        // Set up a new command to redo the last action after reset.
        ActionCommand = new MoveCommand(this, _initialPosition, _initialRotation, transform.position, transform.rotation, true);

        // Reset the object commands in the GameManager.
        GameManager.Instance.ResetObjectCommands(this, ActionCommand);

        // Reset the object back to starting position.
        gameObject.transform.position = _initialPosition;
        gameObject.transform.rotation = _initialRotation;
    }
}
