using UnityEngine;
using System.Collections.Generic;

public class BulletBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float dragSpeed = 10f;

    private bool isDragging = false;
    private Vector3 targetPosition;
    private Vector3 initialPosition = new Vector3();
    private float currentDragDistance;
    private float yPosition;

    private void Awake()
    {
        playerCamera = Camera.main.transform;
    }

    public void OnInteract()
    {
        // When the player starts interacting
        if (!isDragging)
        {
            isDragging = true;

            yPosition = transform.position.y;

            // Calculate and store the distance from camera to object
            currentDragDistance = Vector3.Distance(playerCamera.position, transform.position);
        }

        // Calculate position in front of camera
        targetPosition = playerCamera.position + playerCamera.forward * currentDragDistance;

        targetPosition.y = yPosition;

        // Move object to target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, dragSpeed * Time.unscaledDeltaTime);
    }

    public void OnCancelInteract()
    {
        isDragging = false;
    }

    public void OnResetInteract()
    {
        transform.position = initialPosition;

        OnCancelInteract();
    }
}