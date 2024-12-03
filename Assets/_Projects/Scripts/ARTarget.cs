/*using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using MarksAssets.MindAR;

public class ARTarget : MonoBehaviour {
    public int targetIndex = 0;
    public UnityEvent targetFound;
    public UnityEvent targetLost;

    private ImageTarget imageTarget;
    private Vector3 position = new Vector3();
    private Quaternion rotation = new Quaternion();
    private Vector3 scale = new Vector3();

    private float smoothingFactor = 0.1f; // Adjust this for position smoothing
    private float rotationSmoothingFactor = 0.05f; // Adjust this for rotation smoothing

    [Header("References")]
    public Glow glow;
    public ResetTransform resetTransform;
    public TransformTweener transformTweener;

    void Start() {
        if (!MindAR.isRunning()) MindAR.start();

        imageTarget = MindAR.imageTargets[targetIndex];

        imageTarget.targetFound += () => {
            targetFound.Invoke();
            resetTransform.ResetPositionAndRotation();
            AlignObjectToMarker(); // Instantly set the object to the marker
            FadeInGameObject();    // Fade in after setting position
            transformTweener.HideInstructionAR();
            Invoke("Glow", 2f);
        };

        imageTarget.targetLost += () => {
            targetLost.Invoke();
            resetTransform.SetPositionToCenterOfScreen();
            transformTweener.IsntructionAR();
            MindAR.pause(true);
        };
    }

    void Update() {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Get the current position, rotation, and scale
        position.Set(imageTarget.posx, imageTarget.posy, imageTarget.posz);
        rotation.Set(imageTarget.rotx, imageTarget.roty, imageTarget.rotz, imageTarget.rotw);
        scale.Set(imageTarget.scale, imageTarget.scale, imageTarget.scale);

        // Smoothly interpolate position and rotation if already visible
        if (gameObject.activeSelf) {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, position, smoothingFactor);
            Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, rotation, rotationSmoothingFactor);

            // Apply the smoothed position and rotation
            transform.position = smoothedPosition;
            transform.rotation = smoothedRotation;
            transform.localScale = scale;
        }
#endif
    }

    private void AlignObjectToMarker() {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Set the object's position, rotation, and scale instantly
        position.Set(imageTarget.posx, imageTarget.posy, imageTarget.posz);
        rotation.Set(imageTarget.rotx, imageTarget.roty, imageTarget.rotz, imageTarget.rotw);
        scale.Set(imageTarget.scale, imageTarget.scale, imageTarget.scale);

        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
#endif
    }

    private void FadeInGameObject() {
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null) {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 1f);
    }

    private void Glow() {
        glow.PlayFirstSequence();
    }
    
    public void PlayARSession() {
        MindAR.pause(true);
    }
}*/

using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using MarksAssets.MindAR;

public class ARTarget : MonoBehaviour {
    public int targetIndex = 0;
    public UnityEvent targetFound;
    public UnityEvent targetLost;

    private ImageTarget imageTarget;
    private Vector3 position = new Vector3();
    private Quaternion rotation = new Quaternion();
    private Vector3 scale = new Vector3();

    private Vector3 previousPosition = new Vector3();
    private Quaternion previousRotation = new Quaternion();
    
    private float smoothingFactor = 0.1f; // Adjust this for position smoothing
    private float rotationSmoothingFactor = 0.05f; // Adjust this for rotation smoothing

    [Header("References")]
    public Glow glow; // Add this line to include a reference to the Glow component
    public ResetTransform resetTransform; // Add this line to include a reference to the ResetTransform component
    public TransformTweener transformTweener; // Add this line to include a reference to the TransformTweener component

    void Start() {
        if (!MindAR.isRunning()) MindAR.start();

        imageTarget = MindAR.imageTargets[targetIndex];

        imageTarget.targetFound += () => {
            targetFound.Invoke();
            resetTransform.ResetPositionAndRotation(); // Call the ResetPositionAndRotation function
            SmoothAttachToMarker(); // Use smooth transition for first attachment
            FadeInGameObject();
            transformTweener.HideInstructionAR(); // Call the HideInstructionAR function
            //Invoke("Glow", 2f); // Call Glow method after 1 second  
            //SmoothAttachToMarker(); // Use smooth transition for first attachment
            
        };

        imageTarget.targetLost += () => {
            targetLost.Invoke();
            resetTransform.SetPositionToCenterOfScreen(); // Call the SetPositionToCenterOfScreen function
            transformTweener.IsntructionAR(); // Call the IsntructionAR function
            MindAR.stop(); // Pause the AR session but keep the camera feed on
        };
    }

    void Update() {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Get the current position, rotation, and scale
        position.Set(imageTarget.posx, imageTarget.posy, imageTarget.posz);
        rotation.Set(imageTarget.rotx, imageTarget.roty, imageTarget.rotz, imageTarget.rotw);
        scale.Set(imageTarget.scale, imageTarget.scale, imageTarget.scale);

        // Smoothly interpolate the position
        Vector3 smoothedPosition = Vector3.Lerp(previousPosition, position, smoothingFactor);
        Quaternion smoothedRotation = Quaternion.Lerp(previousRotation, rotation, rotationSmoothingFactor);

        /*// Apply the smoothed position, rotation, and scale
        transform.DOMove(smoothedPosition, 0.2f).SetEase(Ease.OutQuad); // Smooth position move
        transform.DORotateQuaternion(smoothedRotation, 0.2f).SetEase(Ease.OutQuad); // Smooth rotation
        transform.DOScale(scale, 0.2f).SetEase(Ease.OutQuad); // Smooth scale*/

        // Update the previous values for the next frame
        previousPosition = smoothedPosition;
        previousRotation = smoothedRotation;
#endif
    }

    private void SmoothAttachToMarker() {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Set the initial position, rotation, and scale to the marker's values
        position.Set(imageTarget.posx, imageTarget.posy, imageTarget.posz);
        rotation.Set(imageTarget.rotx, imageTarget.roty, imageTarget.rotz, imageTarget.rotw);
        scale.Set(imageTarget.scale, imageTarget.scale, imageTarget.scale);

        // Smoothly move the AR content to the marker's position and rotation
        transform.DOMove(position, 1f).SetEase(Ease.OutQuad);
        transform.DORotateQuaternion(rotation, 1f).SetEase(Ease.OutQuad);
        transform.DOScale(scale, 1f).SetEase(Ease.OutQuad);

        // Store initial values for continuous tracking
        previousPosition = position;
        previousRotation = rotation;
#endif
    }

    private void FadeInGameObject() {
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null) {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 5f);
    }
    
    private void PauseARSession() {
        MindAR.pause(true); // Pause the AR session but keep the camera feed on
    }

    private void Glow()
    {
        glow.PlayFirstSequence(); // Call the PlayFirstSequence function
    }
    
    public void PlayARSession() {
        MindAR.start();
    }
}
