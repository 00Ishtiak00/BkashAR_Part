using UnityEngine;
using UnityEngine.Events;
using DG.Tweening; // Add this line to include DoTween

namespace MarksAssets.MindAR {
    public class ImageTargetMono : MonoBehaviour {
        public int targetIndex = 0;
        public UnityEvent targetFound;
        public UnityEvent targetLost;
        public Glow glow; // Add this line to include a reference to the Glow component

        #pragma warning disable CS0414
        private ImageTarget imageTarget;
        private Vector3 position = new Vector3();
        private Quaternion rotation = new Quaternion();
        private Vector3 scale = new Vector3();

        void Start () {
        #if UNITY_WEBGL && !UNITY_EDITOR
        if (!MindAR.isRunning()) MindAR.start();

        imageTarget = MindAR.imageTargets[targetIndex];

        imageTarget.targetFound += () => {
            targetFound.Invoke();
            //SetPositionAndScale();
            //FadeInGameObject();
            enabled = true;
            glow.PlayFirstSequence(); // Call the PlayFirstSequence function
            Invoke("PauseARSession", 5f); // Call PauseARSession method after 5 seconds
            //MindAR.pause(true); // Pause the AR session but keep the camera feed on
            
            
        };

        imageTarget.targetLost += () => {
            targetLost.Invoke();
            //transform.position = new Vector3(Screen.width / 2, Screen.height / 2, transform.position.z);
            enabled = false;
        };

        enabled = false;
        #endif
        }


        void Update () {
        #if UNITY_WEBGL && !UNITY_EDITOR
        position.Set(imageTarget.posx, imageTarget.posy, imageTarget.posz);
        //rotation.Set(imageTarget.rotx, imageTarget.roty, imageTarget.rotz, imageTarget.rotw);
        scale.Set(imageTarget.scale, imageTarget.scale, imageTarget.scale);

        transform.position = position;
        //transform.rotation = rotation;
        transform.localScale = scale;
        #endif
        }

        /*private void SetPositionAndScale()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        position.Set(imageTarget.posx, imageTarget.posy, imageTarget.posz);
        scale.Set(imageTarget.scale, imageTarget.scale, imageTarget.scale);

        transform.position = position;
        transform.localScale = scale;
#endif
        }*/

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
    }
}
