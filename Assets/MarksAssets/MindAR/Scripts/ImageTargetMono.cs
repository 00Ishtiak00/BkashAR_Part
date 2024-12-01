using UnityEngine;
using UnityEngine.Events;
using DG.Tweening; // Add this line to include DoTween

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

        void Start () {
            
            //SaveTransform();
        //#if UNITY_WEBGL && !UNITY_EDITOR
        if (!MindAR.isRunning()) MindAR.start();

        imageTarget = MindAR.imageTargets[targetIndex];

        
        imageTarget.targetFound += () => {
            resetTransform.ResetPositionAndRotation(); // Call the ResetPositionAndRotation function
            transformTweener.HideInstructionAR(); // Call the HideInstructionAR function
            targetFound.Invoke();
            //SetPositionAndScale();
            FadeInGameObject();
            enabled = true;
            //glow.PlayFirstSequence(); // Call the PlayFirstSequence function
            Invoke("Glow", 2f); // Call Glow method after 1 second  
            //Invoke("PauseARSession", 5f); // Call PauseARSession method after 5 seconds
            //MindAR.pause(true); // Pause the AR session but keep the camera feed on
            
            
        };
        
        imageTarget.targetLost += () => {
            resetTransform.SetPositionToCenterOfScreen(); // Call the SetPositionToCenterOfScreen function
            transformTweener.IsntructionAR(); // Call the IsntructionAR function
            targetLost.Invoke();
            enabled = false;

        };

        enabled = false;
        //#endif
        }


        void Update () {
        #if UNITY_WEBGL && !UNITY_EDITOR
        position.Set(imageTarget.posx, imageTarget.posy, imageTarget.posz);
        rotation.Set(imageTarget.rotx,imageTarget.roty, imageTarget.rotz, imageTarget.rotw);
        scale.Set(imageTarget.scale, imageTarget.scale, imageTarget.scale);

        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;

        #endif
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

        private void FadeInGameObject() { 
            CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 1f).OnComplete(() => {
                enabled = true;
            });
        }
        private void PauseARSession() {
            MindAR.pause(true); // Pause the AR session but keep the camera feed on
        }

        private void Glow()
        {
            glow.PlayFirstSequence(); // Call the PlayFirstSequence function
        }

    }
}
