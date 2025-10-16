using UnityEngine;

public class ObjectState
{
    private GameObject Object { get; set; }
    private Vector3 Location { get; set; }
    private Quaternion Rotation { get; set; }

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
