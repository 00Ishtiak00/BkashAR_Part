using UnityEngine;
using DG.Tweening; // Make sure you have DOTween imported and set up in your project

public class CameraHandAnimation : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera; // Reference to the main camera
    public GameObject handObject; // Reference to the hand GameObject

    [Header("Camera Positions")]
    public Vector3 positionA = new Vector3(220, 370, -10); // Set Z to camera's current Z
    public Vector3 positionB = new Vector3(115, 220, -10);
    public Vector3 positionC = new Vector3(-250, -300, -10);

    [Header("Settings")]
    public float fovInitial = 60f;  // Start FOV
    public float fovTarget = 15f;   // Target FOV
    public float fovFinal = 60f;    // Reset FOV
    public float transitionDuration = 2f;
    public float fadeDuration = 0.5f;

    private bool isAnimating = false; // Flag to track animation state
    
    [SerializeField] private ARDragZoom ARDragZoom; // Reference to the ARDragZoom script
    private void Start()
    {
        // Initialize camera properties
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Set the initial FOV to 60
        mainCamera.fieldOfView = fovInitial;
        mainCamera.transform.position = new Vector3(0, 0, mainCamera.transform.position.z); // Start at (0, 0, Z)

        // Start the sequence
        //PlayAnimationSequence();
    }

    public void PlayAnimationSequence()
    {
        isAnimating = true; // Set flag to true at the start of the animation
        
        // Create a DOTween sequence
        Sequence sequence = DOTween.Sequence();

        // Step 1: Transition FOV from 60 to 15 and move camera to Position A simultaneously
        sequence.Append(mainCamera.DOFieldOfView(fovTarget, transitionDuration).SetEase(Ease.InOutQuad));
        sequence.Join(mainCamera.transform.DOMove(positionA, transitionDuration).SetEase(Ease.InOutQuad));

        // Step 2: Fade in the hand
        sequence.AppendCallback(() => handObject.SetActive(true)); // Activate hand object
        sequence.Join(handObject.GetComponent<CanvasGroup>().DOFade(1, fadeDuration).From(0)); // Fade in hand object

        // Step 3: Move to Position B
        sequence.Append(mainCamera.transform.DOMove(positionB, transitionDuration).SetEase(Ease.InOutQuad));

        // Step 4: Move to Position C
        sequence.Append(mainCamera.transform.DOMove(positionC, transitionDuration).SetEase(Ease.InOutQuad));

        
        // Step 5: Fade out the hand and set FOV back to 60
        sequence.AppendCallback(() => handObject.GetComponent<CanvasGroup>().DOFade(0, fadeDuration)); // Fade out hand
        sequence.AppendCallback(() => handObject.SetActive(false)); // Deactivate hand object
        //sequence.Append(mainCamera.DOFieldOfView(fovFinal, transitionDuration).SetEase(Ease.InOutQuad));

        // Step 6: Reset camera position and FOV to (0, 0, 0) and 60 simultaneously
        sequence.Append(mainCamera.transform.DOMove(Vector3.zero, transitionDuration).SetEase(Ease.InOutQuad));
        sequence.Join(mainCamera.DOFieldOfView(fovFinal, transitionDuration).SetEase(Ease.InOutQuad)); // Reset FOV to 60
    }
}
