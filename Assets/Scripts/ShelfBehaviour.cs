using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShelfBehaviour : InteractionObject
{
    [SerializeField] private float _torque = 2000f;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // TODO: notification logic to show how / if it'll fall
    }

    public override void OnCancelInteract()
    {
        // this should only be thrown due to a logic error in PlayerInteract
        throw new System.NotImplementedException();
    }

    public override void OnInteract()
    {
        if (HasTakenAction) { return; }

        // Set up and execute the topple command, in the right direction.
        ActionCommand = new ToppleCommand(this, _rb, transform.right, _torque);
        GameManager.Instance.RecordAndExecuteCommand(ActionCommand);
    }

    public override void OnResetInteract()
    {
        GameManager.Instance.UndoSpecificCommand(ActionCommand);
    }
}
