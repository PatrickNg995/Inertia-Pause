using UnityEngine;

public struct TransformState
{
    public Vector3 Position;
    public Quaternion Rotation;
    public TransformState(Transform transform)
    {
        Position = transform.position;
        Rotation = transform.rotation;
    }
    public void ApplyTo(Transform transform)
    {
        transform.position = Position;
        transform.rotation = Rotation;
    }
}
