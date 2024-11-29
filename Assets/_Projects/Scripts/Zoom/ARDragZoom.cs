using System.Security.Cryptography;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Assertions.Comparers;
using UnityEngine.UI;

public class ARDragZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 0.05f;             // Speed factor for zooming
    public float minScale = 0.5f;              // Minimum scale for the object
    public float maxScale = 3.0f;              // Maximum scale for the object
    public float zoomThreshold = 2.0f;         // Minimum distance to detect zooming
    public float zoomTransitionDuration = 0.2f; // Smooth transition time in seconds

    [Header("Drag Settings")]
    public float dragThreshold = 5.0f;         // Minimum movement to detect dragging
    public float dragSpeed = 0.01f;            // Speed factor for dragging

    private Camera mainCamera;
    private Vector3 dragStartWorldPos;         // World position when drag starts
    private bool isZooming = false;            // Is the user performing a zoom action
    public bool isDragging = false;           // Is the user performing a drag action

    [SerializeField]private float time;
    
    [SerializeField]private bool touchInteractionsEnabled = true; // Flag to enable/disable touch interactions

    [Header("Script References")]
    [SerializeField] private GraphicRaycaster graphicRaycaster; // Reference to the GraphicRaycaster component
    
    // Flag to enable/disable interactions
    private bool isEnabled = true;
    
    void Start()
    {
        mainCamera = Camera.main; // Cache the main camera
        DisableTouchInteractions(); // Disable touch interactions by default
        Invoke(nameof(EnableTouchInteractions), time); // Call EnableTouchInteractions after 5 seconds
    }
    
    
    /// <summary>
    /// Disables all touch interactions.
    /// </summary>
    public void DisableTouchInteractions()
    {
        touchInteractionsEnabled = false;
        graphicRaycaster.enabled = false; // Disable the GraphicRaycaster
        ResetStates(); // Reset states to ensure no ongoing interactions
    }

    /// <summary>
    /// Enables all touch interactions.
    /// </summary>
    public void EnableTouchInteractions()
    {
        touchInteractionsEnabled = true;
        graphicRaycaster.enabled = true; // Enable the GraphicRaycaster
        
    }

    void Update()
    {
        if (!touchInteractionsEnabled)
            return; // If disabled, skip further processing

        if (Input.touchCount == 1 && !isZooming)
        {
            HandleSingleFingerDrag();
        }
        else if (Input.touchCount == 2)
        {
            HandleTwoFingerGesture();
        }
        else
        {
            ResetStates(); // Reset when no relevant touch is detected
        }
    }

    /// <summary>
    /// Handles dragging functionality for one-finger gestures.
    /// </summary>
    private void HandleSingleFingerDrag()
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            dragStartWorldPos = GetWorldPositionFromTouch(touch.position);
        }

        if (touch.phase == TouchPhase.Moved)
        {
            Vector3 currentWorldPos = GetWorldPositionFromTouch(touch.position);
            Vector3 dragDelta = currentWorldPos - dragStartWorldPos;

            if (dragDelta.magnitude > dragThreshold)
            {
                isDragging = true;
                DragObject(dragDelta);
                dragStartWorldPos = currentWorldPos; // Update for the next frame
            }
        }

        if (touch.phase == TouchPhase.Ended)
        {
            ResetStates();
        }
    }

    /// <summary>
    /// Handles zooming or dragging functionality for two-finger gestures.
    /// </summary>
    private void HandleTwoFingerGesture()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
        {
            isZooming = true;
            isDragging = false; // Disable single drag when two fingers are present
        }

        if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
        {
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);
            float prevDistance = Vector2.Distance(touch1.position - touch1.deltaPosition, touch2.position - touch2.deltaPosition);
            float distanceDelta = currentDistance - prevDistance;

            Vector2 moveDirection1 = touch1.deltaPosition.normalized;
            Vector2 moveDirection2 = touch2.deltaPosition.normalized;
            float directionSimilarity = Vector2.Dot(moveDirection1, moveDirection2);

            if (directionSimilarity > 0.8f) // Drag (same direction)
            {
                isZooming = false;
                HandleTwoFingerDrag(touch1, touch2);
            }
            else // Zoom (different directions)
            {
                isZooming = true;
                HandleZoom(-distanceDelta * zoomSpeed); // Invert zooming behavior
            }
        }

        if (touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended)
        {
            ResetStates();
        }
    }

    /// <summary>
    /// Handles two-finger dragging functionality.
    /// </summary>
    private void HandleTwoFingerDrag(Touch touch1, Touch touch2)
    {
        // Calculate average movement for a smooth drag
        Vector3 averageDelta = (touch1.deltaPosition + touch2.deltaPosition) / 2f;

        Vector3 dragDelta = new Vector3(averageDelta.x, averageDelta.y, 0) * dragSpeed;

        DragObject(dragDelta);
    }

    /// <summary>
    /// Handles zooming functionality based on two-finger pinch gestures.
    /// </summary>
    private void HandleZoom(float increment)
    {
        Vector3 targetScale = transform.localScale + Vector3.one * increment;

        // Clamp scale to prevent over-scaling
        targetScale.x = Mathf.Clamp(targetScale.x, minScale, maxScale);
        targetScale.y = Mathf.Clamp(targetScale.y, minScale, maxScale);
        targetScale.z = Mathf.Clamp(targetScale.z, minScale, maxScale);

        // Smoothly transition to the target scale using DoTween
        transform.DOScale(targetScale, zoomTransitionDuration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// Drags the object based on touch input, constrained to X and Y axes.
    /// </summary>
    /// <param name="dragDelta">Movement delta in world space.</param>
    private void DragObject(Vector3 dragDelta)
    {
        Vector3 targetPosition = transform.position + new Vector3(dragDelta.x, dragDelta.y, 0);

        // Clamp the target position to keep the object within the camera's view
        targetPosition = ClampToCameraView(targetPosition);

        transform.position = targetPosition;
    }

    /// <summary>
    /// Converts a screen touch position to a world position.
    /// </summary>
    private Vector3 GetWorldPositionFromTouch(Vector2 touchPosition)
    {
        Vector3 screenPos = new Vector3(touchPosition.x, touchPosition.y, mainCamera.WorldToScreenPoint(transform.position).z);
        return mainCamera.ScreenToWorldPoint(screenPos);
    }

    /// <summary>
    /// Clamps the object's position to keep it within the camera's visible viewport,
    /// while allowing movement to the edges at maximum zoom.
    /// </summary>
    /// <param name="targetPosition">The proposed position of the object.</param>
    /// <returns>A clamped position within the camera's bounds.</returns>
    private Vector3 ClampToCameraView(Vector3 targetPosition)
    {
        // Get the object's current bounds in world space
        Bounds objectBounds = GetObjectBounds();

        // Convert the corners of the object's bounds to viewport points
        Vector3 minViewport = mainCamera.WorldToViewportPoint(objectBounds.min);
        Vector3 maxViewport = mainCamera.WorldToViewportPoint(objectBounds.max);

        // Calculate clamping margins based on the viewport
        float horizontalMargin = Mathf.Max(0, (maxViewport.x - minViewport.x) / 2f);
        float verticalMargin = Mathf.Max(0, (maxViewport.y - minViewport.y) / 2f);

        // Convert target position to viewport and apply clamping
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(targetPosition);
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, horizontalMargin, 1 - horizontalMargin);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, verticalMargin, 1 - verticalMargin);

        // Convert back to world space and return the clamped position
        return mainCamera.ViewportToWorldPoint(viewportPosition);
    }

    /// <summary>
    /// Calculates the bounds of the object, accounting for its scale.
    /// </summary>
    /// <returns>The world space bounds of the object.</returns>
    private Bounds GetObjectBounds()
    {
        Renderer objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            return objectRenderer.bounds;
        }
        else
        {
            // Fallback if no renderer is attached
            return new Bounds(transform.position, Vector3.zero);
        }
    }


    /// <summary>
    /// Resets the states of zooming and dragging.
    /// </summary>
    private void ResetStates()
    {
        isZooming = false;
        //DOVirtual.DelayedCall(0.2f, () => isDragging = false);
        isDragging = false;
    }
    
    /// <summary>
    /// Disables zooming and dragging functionality.
    /// </summary>
    public void DisableInteraction()
    {
        isEnabled = false; // Prevent drag and zoom
    }

    /// <summary>
    /// Enables zooming and dragging functionality.
    /// </summary>
    public void EnableInteraction()
    {
        isEnabled = true; // Re-enable drag and zoom
    }
}
