using MarksAssets.MindAR;
using TMPro;
using UnityEngine;

public class ResetTransform : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 initialRotation;
    private Vector3 initialScale;
    
    public ImageTargetMono ImageTargetMono;

    private bool _firstTime = true;
    [ContextMenu("ResetPositionAndRotation")]
    // Function to reset position and rotation
    public void ResetPositionAndRotation()
    {
        if (_firstTime)
        {
            SaveTransform();
            _firstTime = false;
            return;
        }
        transform.localPosition=initialPosition;
        transform.localRotation=Quaternion.Euler(initialRotation);
        transform.localScale=initialScale;
    }

    public TMP_Text uiText; // Reference to the UI Text component

    void Update()
    {
        if (uiText != null)
        {
            Vector3 position = transform.localPosition;
            Quaternion rotation = transform.localRotation;
            Vector3 scale = transform.localScale;

            uiText.text = $"Position: {position}\n" +
                          $"Rotation: {rotation.eulerAngles}\n" +
                          $"Scale: {scale}";
        }
    }

    [ContextMenu("SetPositionToCenterOfScreen")]
    // Function to set the position to the center of the screen
    public void SetPositionToCenterOfScreen()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, transform.position.z);
        transform.position = Camera.main.ScreenToWorldPoint(screenCenter);
        transform.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation to (0, 0, 0)
        
        //ImageTargetMono.FadeInGameObject();
    }
    
    private void SaveTransform()
    {
        initialPosition=transform.localPosition;
        initialRotation=transform.localRotation.eulerAngles;
        initialScale=transform.localScale;
    }
}