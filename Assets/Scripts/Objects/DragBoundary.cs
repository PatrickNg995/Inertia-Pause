using UnityEngine;

public class DragBoundary : MonoBehaviour
{
    [SerializeField] private float _radius = 1f;
    [SerializeField] private int _segments = 64;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _yOffset = 0f;

    private Vector3 center;
    void Awake()
    {
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.loop = true;
        _lineRenderer.startWidth = 0.01f;
        _lineRenderer.endWidth = 0.01f;

        center = transform.position;
        center.y += _yOffset;
    }

    public void ShowCircle(bool show)
    {
        _lineRenderer.enabled = show;

        if (show)
        {
            DrawCircle();
        }
    }

    private void DrawCircle()
    {
        Vector3[] points = new Vector3[_segments];
        for (int i = 0; i < _segments; i++)
        {
            float angle = (float)i / _segments * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * _radius;
            float y = Mathf.Sin(angle) * _radius;
            points[i] = center + new Vector3(x, 0, y);
        }
        
        _lineRenderer.positionCount = _segments;
        _lineRenderer.SetPositions(points);
    }
}
