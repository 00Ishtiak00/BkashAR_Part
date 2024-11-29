using UnityEngine;
using UnityEngine.UI;

public class WebCamFeed : MonoBehaviour
{
    public RawImage rawImage; // Assign your RawImage in the Inspector
    public AspectRatioFitter aspectRatioFitter;
    private WebCamTexture webCamTexture;

    // Desired low resolution
    public int targetWidth = 320;
    public int targetHeight = 240;

    void Start()
    {
        StartWebCam();
    }

    private void StartWebCam()
    {
        // Check if there are cameras available
        if (WebCamTexture.devices.Length > 0)
        {
            WebCamDevice rearCamera = WebCamTexture.devices[0];

            // Find the rear camera
            foreach (WebCamDevice device in WebCamTexture.devices)
            {
                if (device.isFrontFacing == false)
                {
                    rearCamera = device;
                    break;
                }
            }

            // Initialize WebCamTexture with low resolution
            webCamTexture = new WebCamTexture(rearCamera.name, targetWidth, targetHeight);

            // Assign the texture to the RawImage
            rawImage.texture = webCamTexture;
            rawImage.material.mainTexture = webCamTexture;

            // Maintain the aspect ratio of the camera feed
            if (aspectRatioFitter != null)
            {
                aspectRatioFitter.aspectRatio = (float)webCamTexture.width / webCamTexture.height;
            }

            // Start the camera
            webCamTexture.Play();
        }
        else
        {
            Debug.LogError("No cameras found on this device!");
        }
    }

    void OnDisable()
    {
        // Stop the camera when the script is disabled
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }
}