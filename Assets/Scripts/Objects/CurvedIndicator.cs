using UnityEngine;

public class CurvedIndicator : Indicator
{
    [SerializeField] private LineRenderer _line;
    [SerializeField] private float _lineLength = 1f;
    [SerializeField] private float _lineWidth = 0.01f;
    [SerializeField]private float _curveLift = 0.1f;
    [SerializeField] private int _segments = 20;
    // how much the line should drop, as a multiplier of _lineLength
    [SerializeField] private float _endDrop = 0.3f;

    private void Start()
    {
        _line.enabled = false;
        _line.positionCount = _segments + 1;
        _line.startWidth = _lineWidth;
        _line.endWidth = 0;
    }

    public override void Enable()
    {
        _line.enabled = true;
    }
    public override void Disable()
    {
        _line.enabled = false;
    }

    public override void Draw()
    {
        Vector3 p0 = transform.position;

        // End point forward + down
        Vector3 p1 = p0
            + transform.forward * _lineLength
            - Vector3.up * (_lineLength * _endDrop);

        // Midpoint slightly ABOVE the straight line to make curve concave DOWN
        Vector3 mid = (p0 + p1) / 2;

        // Lift is small and relative to the downward drop
        float lift = Mathf.Abs((p0.y - p1.y)) * _curveLift;
        Vector3 controlPoint = mid + (Vector3.up * lift);

        // Bézier sampling
        for (int i = 0; i <= _segments; i++)
        {
            float t = i / (float)_segments;
            Vector3 point =
                (1 - t) * (1 - t) * p0 +
                2 * (1 - t) * t * controlPoint +
                t * t * p1;

            _line.SetPosition(i, point);
        }
    }
}
