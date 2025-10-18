using UnityEngine;
using System.Collections.Generic;

public class BulletBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private float DragSpeed = 10f;

    private bool _isDragging = false;
    private Vector3 _targetPosition;
    private Vector3 _resetPosition;
    private Vector3 _cancelPosition;
    private float _currentDragDistance;
    private float _yPosition;

    private void Awake()
    {
        PlayerCamera = Camera.main.transform;
        _resetPosition = transform.position;
    }

    public void OnStartInteract()
    {
        if (!_isDragging)
        {
            _isDragging = true;

            _yPosition = transform.position.y;

            _cancelPosition = transform.position;

            // Calculate and store the distance from camera to object
            _currentDragDistance = Vector3.Distance(PlayerCamera.position, transform.position);
        }
    }

    public void OnHoldInteract()
    {
        // Calculate position in front of camera
        _targetPosition = PlayerCamera.position + PlayerCamera.forward * _currentDragDistance;

        _targetPosition.y = _yPosition;

        // Move object to target position
        transform.position = Vector3.Lerp(transform.position, _targetPosition, DragSpeed * Time.unscaledDeltaTime);
    }

    public void OnEndInteract()
    {
        _isDragging = false;
    }

    public void OnCancelInteract()
    {
        transform.position = _cancelPosition;

        _isDragging = false;
    }

    public void OnResetInteract()
    {
        transform.position = _resetPosition;

        _isDragging = false;
    }
}