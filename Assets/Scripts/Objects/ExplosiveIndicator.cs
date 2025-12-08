using UnityEngine;

public class ExplosiveIndicator : Indicator
{
    [SerializeField] private float _radius = 1f;
    [SerializeField] private int _segments = 64;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _yOffset = 0f;

    private Vector3[] _buffer; // holds both circles

    void Awake()
    {
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.loop = false;
        _lineRenderer.startWidth = 0.01f;
        _lineRenderer.endWidth = 0.01f;

        // double segments (horizontal + vertical)
        _buffer = new Vector3[_segments * 2];
    }

    public override void Enable()
    {
        _lineRenderer.enabled = true;
    }

    public override void Disable()
    {
        _lineRenderer.enabled = false;
    }

    public override void Draw()
    {
        int idx = 0;

        // Horizontal circle (XZ plane)
        for (int i = 0; i < _segments; i++)
        {
            float angle = (float)i / _segments * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * _radius;
            float z = Mathf.Sin(angle) * _radius;

            _buffer[idx++] = new Vector3(x, _yOffset, z);
        }

        // Vertical circle (XY plane)
        for (int i = 0; i < _segments; i++)
        {
            float angle = (float)i / _segments * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * _radius;
            float y = Mathf.Sin(angle) * _radius;

            _buffer[idx++] = new Vector3(x, y + _yOffset, 0);
        }

        // Apply to LineRenderer
        _lineRenderer.positionCount = _buffer.Length;
        _lineRenderer.SetPositions(_buffer);
    }
}
