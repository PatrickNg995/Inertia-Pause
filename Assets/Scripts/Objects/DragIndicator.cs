using UnityEngine;

public class DragIndicator : MonoBehaviour
{
    [SerializeField] private LineRenderer _directionalIndicator;
    // intended to be false for things like bullets
    [SerializeField] private bool _isFallingIndicator;
    [SerializeField] private float _lineLength = 1f;
    [SerializeField] private float _lineWidth = 0.01f;

    void Start()
    {
        _directionalIndicator.enabled = false;
        _directionalIndicator.positionCount = 2;
        _directionalIndicator.startWidth = _lineWidth;
        _directionalIndicator.endWidth = 0;
    }

    public void Enable()
    {
        _directionalIndicator.enabled = true;
    }

    public void Disable()
    {
        _directionalIndicator.enabled = false;
    }

    public void DrawLine()
    {
        Vector3 startPos = transform.position;
        _directionalIndicator.SetPosition(0, startPos);

        Vector3 endPos = startPos;
        if (_isFallingIndicator)
        {
            endPos -= Vector3.up * _lineLength;
        }
        else
        {
            endPos += transform.forward * _lineLength;
        }

        _directionalIndicator.SetPosition(1, endPos);
    }
}
