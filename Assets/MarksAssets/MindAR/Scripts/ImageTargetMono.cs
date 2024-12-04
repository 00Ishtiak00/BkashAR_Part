using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;
using UnityEngine.UI; // Add this line to include DoTween

namespace MarksAssets.MindAR {
    public class ImageTargetMono : MonoBehaviour {
        public int targetIndex = 0;
        public UnityEvent targetFound;
        public UnityEvent targetLost;
        public Glow glow; // Add this line to include a reference to the Glow component
        public ResetTransform resetTransform; // Add this line to include a reference to the ResetTransform component
        public TransformTweener transformTweener; // Add this line to include a reference to the TransformTweener component

        #pragma warning disable CS0414
        private ImageTarget imageTarget;
        private Vector3 position = new Vector3();
        private Quaternion rotation = new Quaternion();
        private Vector3 scale = new Vector3();
        
        private Vector3 initialPosition;
        private Vector3 initialRotation;
        private Vector3 initialScale;
        
        private Quaternion previousRotation = new Quaternion();
        private float rotationThreshold = 1.2f; // Adjust this threshold to avoid jitter


        public TMP_Text rotationText; // Add this line to include a reference to the TMP_Text component
        
        public bool isTracking = false; // Flag to indicate if the target is being tracked

        public PopupManager popupManager; // Add this line to include a reference to the PopupManager component
        void Start () {
            
            //SaveTransform();
        //#if UNITY_WEBGL && !UNITY_EDITOR
        if (!MindAR.isRunning()) MindAR.start();

        imageTarget = MindAR.imageTargets[targetIndex];

        
        imageTarget.targetFound += () => {
            isTracking = true; // Set the flag to true when the target is found
            //PlaySoundOnTrack(); // Call the PlaySoundOnTrack function
            resetTransform.ResetPositionAndRotation(); // Call the ResetPositionAndRotation function
            transformTweener.HideInstructionAR(); // Call the HideInstructionAR function
            targetFound.Invoke();
            ResetInitialTransform();
            //SetPositionAndScale();
            FadeInGameObject();
            enabled = true;
            Invoke("Glow", 1.5f); // Call Glow method after 1 second  
            //PlaySoundOnTrack();
      
            
            
        };
        
        imageTarget.targetLost += () => {
            isTracking = false; // Set the flag to false when the target is lost
            //PauseARSession(); // Call the PauseARSession function
            //transformTweener.IsntructionAR(); // Call the IsntructionAR function
            targetLost.Invoke();
            enabled = false;
            resetTransform.SetPositionToCenterOfScreen(); // Call the SetPositionToCenterOfScreen function

        };

        enabled = false;
        //#endif
        }


        void Update () 
        {
            if (isTracking)
            {
                position.Set(imageTarget.posx, imageTarget.posy, imageTarget.posz);
                rotation.Set(imageTarget.rotx, imageTarget.roty, imageTarget.rotz, imageTarget.rotw);
                scale.Set(imageTarget.scale, imageTarget.scale, imageTarget.scale);

                float deltaX = Mathf.Abs(rotation.eulerAngles.x - previousRotation.eulerAngles.x);
                float deltaY = Mathf.Abs(rotation.eulerAngles.y - previousRotation.eulerAngles.y);
                float deltaZ = Mathf.Abs(rotation.eulerAngles.z - previousRotation.eulerAngles.z);

                if (deltaX > rotationThreshold || deltaY > rotationThreshold || deltaZ > rotationThreshold)
                {
                    rotation = Quaternion.Lerp(previousRotation, rotation, 0.1f);

                    transform.DOMove(position, 0.2f).SetEase(Ease.OutQuad);
                    transform.DORotateQuaternion(rotation, 0.2f).SetEase(Ease.OutQuad);
                    transform.DOScale(scale, 0.2f).SetEase(Ease.OutQuad);

                    previousRotation = rotation;
                }
            }

            /*// Check if the change in rotation is above the threshold
            if (Quaternion.Angle(previousRotation, rotation) > rotationThreshold) {
                // Smoothly interpolate position and rotation using DOTween
                transform.DOMove(position, 0.2f).SetEase(Ease.OutQuad);
                transform.DORotateQuaternion(rotation, 0.2f).SetEase(Ease.OutQuad);
                transform.DOScale(scale, 0.2f).SetEase(Ease.OutQuad);

                // Update the previous rotation
                previousRotation = rotation;
            }*/

            /*transform.position = position;
            transform.rotation = rotation;
            transform.localScale = scale;*/
            
            //#endif
            
            // Update the UI Text with the GameObject's rotation
            if (rotationText != null) 
            {
                rotationText.text = "Rotation: " + transform.localRotation.eulerAngles.ToString();
            } 
        }

        public void SetPositionAndScale()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        position.Set(imageTarget.posx, imageTarget.posy, imageTarget.posz);
        rotation.Set(imageTarget.rotx, imageTarget.roty, imageTarget.rotz, imageTarget.rotw);
        scale.Set(imageTarget.scale, imageTarget.scale, imageTarget.scale);

        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
#endif
        }

        public void FadeInGameObject() { 
            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 1f).OnComplete(() => {
                enabled = true;
            });
        }
        
        public void FadeOutGameObject() {
            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1;
            canvasGroup.DOFade(0, 1f).OnComplete(() => {
                enabled = false;
            });
        }
        private void PauseARSession() {
            
            MindAR.pause(true); // Pause the AR session but keep the camera feed on
        }
        
        public void ResumeARSession() 
        {
            MindAR.pause(false); // Resume the AR session
            
        }
        
        private void ResetTracking()
        {
            MindAR.start();
        }

        private void Glow()
        {
            glow.PlayFirstSequence(); // Call the PlayFirstSequence function
        }
        
        private void ResetInitialTransform()
        {
            transform.position = initialPosition;
            transform.rotation = Quaternion.Euler(initialRotation);
            transform.localScale = initialScale;
        }
        
        public void PlaySoundOnTrack()
        {
            if (isTracking = true)
            {
                popupManager.DelayAudio(); // Call the PlayAudio function
                isTracking=false;
            }
            
        }

    }
}
