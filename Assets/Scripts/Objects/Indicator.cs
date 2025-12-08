using UnityEngine;

public abstract class Indicator : MonoBehaviour
{
    public abstract void Enable();
    public abstract void Disable();
    public abstract void Draw();
}