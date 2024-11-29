using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase; // Include DoTween namespace

public class DragPinchZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float minFov = 15f; // Minimum Field of View
    public float maxFov = 90f; // Maximum Field of View
    public float zoomSpeed = 0.1f; // Speed multiplier for zoom
    public float zoomSmoothness = 0.25f; // Smoothing factor for zoom
    public float zoomDelay = 0.1f; // Delay before zoom starts (in seconds)

    [Header("Drag Settings")]
    public Vector2 xAxisLimits = new Vector2(-10f, 10f); // Limits for X-axis movement
    public Vector2 yAxisLimits = new Vector2(-5f, 5f); // Limits for Y-axis movement
    public float dragSpeed = 10f; // Drag sensitivity multiplier
    public float dragSmoothness = 0.25f; // Smoothing factor for drag

    private Camera cam;
    private Vector3 dragTargetPosition;
    private float targetFov;

    private bool isZooming; // Flag to indicate active zooming
    private float lastZoomTime; // Timer to delay zoom activation
    
    private Vector2 touch0PrevPosition, touch1PrevPosition; // Store previous touch positions

    void Start()
    {
        cam = Camera.main; // Get the main camera
        if (cam == null)
        {
            Debug.LogError("Main Camera not found!");
        }

        // Initialize target values
        dragTargetPosition = cam.transform.position;
        targetFov = cam.fieldOfView;
    }

    void Update()
    {
        HandleZoom();
        HandleDrag();

        // Smoothly move the camera to the target position
        cam.transform.DOMove(dragTargetPosition, dragSmoothness).SetEase(Ease.OutQuad);

        // Smoothly adjust the Field of View
        cam.DOFieldOfView(targetFov, zoomSmoothness).SetEase(Ease.OutQuad);
    }

    private void HandleZoom()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Check for delay before starting zoom
            if (Time.time - lastZoomTime < zoomDelay) return;
            lastZoomTime = Time.time;

            // Store previous touch positions
            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                touch0PrevPosition = touch0.position;
                touch1PrevPosition = touch1.position;
            }

            // Calculate distance between the two touches
            float previousDistance = Vector2.Distance(touch0PrevPosition, touch1PrevPosition);
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);

            // Determine if the touches are moving apart (zoom in) or coming together (zoom out)
            if (Mathf.Abs(currentDistance - previousDistance) > 10f) // Threshold for zoom to activate
            {
                if (currentDistance > previousDistance) // Fingers moving apart
                {
                    AdjustFieldOfView(-zoomSpeed); // Zoom in
                }
                else // Fingers moving together
                {
                    AdjustFieldOfView(zoomSpeed); // Zoom out
                }
            }

            // Update previous positions for the next frame
            touch0PrevPosition = touch0.position;
            touch1PrevPosition = touch1.position;
        }
    }

    private void AdjustFieldOfView(float delta)
    {
        targetFov = Mathf.Clamp(targetFov + delta, minFov, maxFov);
    }

    private void HandleDrag()
    {
        if (isZooming || Input.touchCount == 2) return; // Skip drag if zooming is active or if there are two touch points

        // For touch drag
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 deltaPosition = touch.deltaPosition;
                UpdateDragTarget(deltaPosition * dragSpeed * 0.01f); // Scaled for mobile responsiveness
            }
        }
        // For mouse drag (WebGL)
        else if (Input.GetMouseButton(0)) 
        {
            Vector3 mouseDelta = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0);
            UpdateDragTarget(mouseDelta * dragSpeed);
        }
    }

    private void UpdateDragTarget(Vector3 delta)
    {
        Vector3 newPosition = dragTargetPosition;

        // Adjust the target position based on delta input
        newPosition.x = Mathf.Clamp(newPosition.x + delta.x, xAxisLimits.x, xAxisLimits.y);
        newPosition.y = Mathf.Clamp(newPosition.y + delta.y, yAxisLimits.x, yAxisLimits.y);

        dragTargetPosition = newPosition;
    }

    // Optional: Methods to adjust limits programmatically
    public void SetZoomLimits(float min, float max)
    {
        minFov = min;
        maxFov = max;
    }

    public void SetDragLimits(Vector2 xLimits, Vector2 yLimits)
    {
        xAxisLimits = xLimits;
        yAxisLimits = yLimits;
    }
}
