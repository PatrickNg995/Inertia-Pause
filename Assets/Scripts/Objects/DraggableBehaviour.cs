using UnityEngine;

public class DraggableBehaviour : InteractionObject
{
    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private float DragSpeed = 10f;

    public float MaxDragDistance = 1f;

    private DragCommand _currentDragCommand;
    private Vector3 _initialPosition;

    private void Awake()
    {
        IsContinuousUpdate = true;

        PlayerCamera = Camera.main.transform;
        _initialPosition = transform.position;
    }

    public override void OnStartInteract()
    {
        // Create a new drag command when interaction starts
        _currentDragCommand = new DragCommand(this, PlayerCamera, DragSpeed, MaxDragDistance);
    }

    public override void OnHoldInteract()
    {
        // Update the drag position through the command
        _currentDragCommand?.UpdateDrag();
    }

    public override void OnEndInteract()
    {
        if (_currentDragCommand == null) return;

        // Finalize the drag to store the final position
        _currentDragCommand.FinalizeDrag();

        // Record and execute the command
        ActionCommand = _currentDragCommand;
        GameManager.Instance.RecordAndExecuteCommand(ActionCommand);

        _currentDragCommand = null;
    }

    public override void OnCancelInteract()
    {
        if (_currentDragCommand == null) return;

        // Cancel the drag without recording it
        _currentDragCommand.CancelDrag();
        _currentDragCommand = null;
    }

    public override void OnResetInteract()
    {
        transform.position = _initialPosition;
        // command added sets the position the current, before the above assignment, on execute, and to _initialPosition on undo
        GameManager.Instance.ResetObjectCommands(this, new DragCommand(this, _initialPosition, transform.position));
    }
}