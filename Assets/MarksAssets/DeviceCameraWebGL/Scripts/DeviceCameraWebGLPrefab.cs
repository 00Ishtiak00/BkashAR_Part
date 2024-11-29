using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MarksAssets.DeviceCameraWebGL;
using status = MarksAssets.DeviceCameraWebGL.DeviceCameraWebGL.status;
using System;

public class DeviceCameraWebGLPrefab : MonoBehaviour {
	[Serializable] public class UnityEventStatus : UnityEvent<status>{};

    [SerializeField] private bool existingVideo;
    [SerializeField] private bool isFront;

    [SerializeField] private UnityEventStatus onStart;

    [SerializeField] private UnityEvent onStop;

    [SerializeField] private RenderTexture targetTexture;

    private string lastFacingMode;

    void Start() {
        targetTexture.colorBuffer.ToString();//see https://issuetracker.unity3d.com/issues/getnativetextureptr-returns-0-on-rendertexture-until-colorbuffer-property-get-is-called
        if (!existingVideo) {
        
            DeviceCameraWebGL.createVideoElement(updateAspectRatio);

            if (isFront)
                DeviceCameraWebGL.startFrontCamera(flipX);
            else
                DeviceCameraWebGL.startRearCamera(flipX);
        } else {
            DeviceCameraWebGL.selector = "video:not([src])";//check to find video without source attribute. Useful if using VideoPlayerWebGL as well.
            DeviceCameraWebGL.waitForElement(hijackVideoElement);
        }

    }

    // Update is called once per frame
    void Update() {
        if (DeviceCameraWebGL.canUpdateTexture != 0) {
            DeviceCameraWebGL.updateTexture(targetTexture.GetNativeTexturePtr());
        }
    }

    void hijackVideoElement() {
        DeviceCameraWebGL.assignCSSToVideo();
        DeviceCameraWebGL.assignPlayingEventToVideo(flipX);
        DeviceCameraWebGL.assignResizeEventToVideo(updateAspectRatio);
        if (isFront) {
            if (DeviceCameraWebGL.getCurrentFacingMode() == "user") return;
            DeviceCameraWebGL.startFrontCamera(flipX);
        }
        else {
            if (DeviceCameraWebGL.getCurrentFacingMode() == "environment" || (DeviceCameraWebGL.getCurrentFacingMode() == "user" && DeviceCameraWebGL.isDesktop()) ) return;
            DeviceCameraWebGL.startRearCamera(flipX);
        }
    }

    public void Play() {
        if (DeviceCameraWebGL.getCurrentFacingMode() == "none") {//if stopped camera
            if (lastFacingMode == "user") DeviceCameraWebGL.startFrontCamera(flipX);
            else DeviceCameraWebGL.startRearCamera(flipX);
        }
    }
    
    public void Stop() {
        DeviceCameraWebGL.stop(onStopMethod);
    }

    public void UserCamera() {
        if (DeviceCameraWebGL.getCurrentFacingMode() == "user") return;
        DeviceCameraWebGL.startFrontCamera(flipX);
    }

    public void RearCamera() {
        if (DeviceCameraWebGL.getCurrentFacingMode() == "environment" || (DeviceCameraWebGL.getCurrentFacingMode() == "user" && DeviceCameraWebGL.isDesktop()) ) return;
        DeviceCameraWebGL.startRearCamera(flipX);
    }

    public void SwitchCamera() {
        if (DeviceCameraWebGL.getCurrentFacingMode() == "user" && DeviceCameraWebGL.isDesktop()) return;
        DeviceCameraWebGL.switchCamera(flipX);
    }

    private void onStopMethod() {
        onStop?.Invoke();
    }

    private void updateAspectRatio(uint videoWidth, uint videoHeight) {
        GetComponent<AspectRatioFitter>().aspectRatio = (float)videoWidth/(float)videoHeight;
	}

    private void flipX(status stat) {
        if (stat == status.Success) {
            string facingMode = DeviceCameraWebGL.getCurrentFacingMode();
            if (facingMode == "user" && transform.localScale.x > 0 || facingMode == "environment" && transform.localScale.x < 0)
                transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
        }

        onStart?.Invoke(stat);
    }
}
