using System;
using DG.Tweening;
using UnityEngine;

public class CanvasDragZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    [SerializeField] private float minZoom = 0.5f; // Minimum scale
    [SerializeField] private float maxZoom = 3f;   // Maximum scale
    [SerializeField] private float zoomSpeed = 0.01f; // Speed of zooming
    [SerializeField] private float zoomSmoothing = 0.25f; // Smoothing time for zoom

    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 0.005f; // Drag multiplier
    [SerializeField] private float dragSmoothing = 0.25f; // Smoothing time for drag
    [SerializeField] private Vector2 defaultPosition;
    [SerializeField] private float maxX;
    [SerializeField] private float maxY;
    
    private Vector2 targetPosition; // Target position for smooth drag
    private Vector3 targetScale;    // Target scale for smooth zoom

    [SerializeField] private bool isTouchSupported;
    private float previousPinchDistance; // Tracks the previous pinch distance

    private void Start()
    {
        targetPosition = transform.position;
        targetScale = transform.localScale;

        isTouchSupported = Input.touchSupported;
    }

    private void OnEnable()
    {
        defaultPosition = GetComponent<RectTransform>().position;
    }

    private void Update()
    {
        if (isTouchSupported)
        {
            HandleTouchDrag();
            HandlePinchToZoom();
        }
        else
        {
            HandleMouseDrag();
            HandleMouseZoom();
        }
    }

    private void HandleTouchDrag()
    {
        if (Input.touchCount == 1) // Single touch for dragging
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                // Calculate delta and invert direction
                Vector2 delta = new Vector2(-touch.deltaPosition.x, -touch.deltaPosition.y) * dragSpeed;
                targetPosition += delta;
                targetPosition.x = Mathf.Clamp(targetPosition.x, defaultPosition.x - maxX, defaultPosition.x + maxX);
                targetPosition.y = Mathf.Clamp(targetPosition.y, defaultPosition.y - maxY, defaultPosition.y + maxY);
            }
            // Smoothly move towards the target position
            transform.DOMove(targetPosition, dragSmoothing).SetEase(Ease.OutQuad);
        }
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButton(0)) // Mouse drag
        {
            // Get mouse delta and invert direction
            float deltaX = -Input.GetAxis("Mouse X") * dragSpeed;
            float deltaY = -Input.GetAxis("Mouse Y") * dragSpeed;
            targetPosition += new Vector2(deltaX, deltaY);
            targetPosition.x = Mathf.Clamp(targetPosition.x, defaultPosition.x - maxX, defaultPosition.x + maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, defaultPosition.y - maxY, defaultPosition.y + maxY);
            
            // Smoothly move towards the target position
            transform.DOMove(targetPosition, dragSmoothing).SetEase(Ease.OutQuad);
        }
    }

    private void HandlePinchToZoom()
    {
        if (Input.touchCount == 2) // Two-finger pinch
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Calculate the distance between the two fingers
            float currentPinchDistance = Vector2.Distance(touch0.position, touch1.position);

            if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                // Check for the first frame of the pinch
                if (previousPinchDistance == 0)
                {
                    previousPinchDistance = currentPinchDistance;
                    return;
                }

                // Determine the zoom delta (positive = zoom in, negative = zoom out)
                float zoomDelta = (currentPinchDistance - previousPinchDistance) * zoomSpeed;
                AdjustZoom(zoomDelta);

                // Update the previous pinch distance
                previousPinchDistance = currentPinchDistance;
            }
            
            // Smoothly scale towards the target scale
            transform.DOScale(targetScale, zoomSmoothing).SetEase(Ease.OutQuad);
        }
        else
        {
            // Reset the pinch distance when fingers are lifted
            previousPinchDistance = 0;
        }
    }

    private void HandleMouseZoom()
    {
        if (Input.mouseScrollDelta.y != 0) // Mouse scroll wheel for zoom
        {
            AdjustZoom(Input.mouseScrollDelta.y * zoomSpeed);
            
            // Smoothly scale towards the target scale
            transform.DOScale(targetScale, zoomSmoothing).SetEase(Ease.OutQuad);
        }
    }

    private void AdjustZoom(float increment)
    {
        targetScale += Vector3.one * increment;
        targetScale.x = Mathf.Clamp(targetScale.x, minZoom, maxZoom);
        targetScale.y = Mathf.Clamp(targetScale.y, minZoom, maxZoom);
        targetScale.z = Mathf.Clamp(targetScale.z, minZoom, maxZoom);
    }
}