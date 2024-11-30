using TMPro;
using UnityEngine;

public class ResetTransform : MonoBehaviour
{
    [ContextMenu("ResetPositionAndRotation")]
    // Function to reset position and rotation
    public void ResetPositionAndRotation()
    {
        transform.position = Vector3.zero; // Reset position to (0, 0, 0)
        transform.rotation = Quaternion.identity; // Reset rotation to (0, 0, 0)
        //Debug.Log($"{gameObject.name} position and rotation have been reset to zero.");
    }

    //public TMP_Text uiText; // Reference to the UI Text component

    void Update()
    {
        /*if (uiText != null)
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            Vector3 scale = transform.localScale;

            uiText.text = $"Position: {position}\n" +
                          $"Rotation: {rotation.eulerAngles}\n" +
                          $"Scale: {scale}";
        }*/
    }

    [ContextMenu("SetPositionToCenterOfScreen")]
    // Function to set the position to the center of the screen
    public void SetPositionToCenterOfScreen()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, transform.position.z);
        transform.position = Camera.main.ScreenToWorldPoint(screenCenter);
        transform.rotation = Quaternion.Euler(0, 0, 0); // Reset rotation to (0, 0, 0)
    }
}