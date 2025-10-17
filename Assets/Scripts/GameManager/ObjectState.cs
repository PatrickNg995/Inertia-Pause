using UnityEngine;

public struct ObjectState
{
    public GameObject Object;
    public Vector3 Location;
    public Quaternion Rotation;

    public ObjectState(GameObject gameObject, Vector3 location, Quaternion rotation)
    {
        Object = gameObject;
        Location = location;
        Rotation = rotation;
    }

    public void LoadObjectState()
    {
        if (Object != null)
        {
            Object.transform.position = Location;
            Object.transform.rotation = Rotation;
        }
    }
}
