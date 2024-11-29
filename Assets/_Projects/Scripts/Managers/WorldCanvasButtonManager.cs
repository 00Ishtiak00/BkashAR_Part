using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorldCanvasButtonManager : MonoBehaviour
{
    [Header("Button Settings")]
    public List<GameObject> buttons; // List of all buttons
    public float animationDuration = 0.5f; // Duration for scale animation

    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    private void Start()
    {
        // Deactivate all buttons initially and store original scales
        foreach (var button in buttons)
        {
            originalScales[button] = button.transform.localScale; // Store original scale
            button.SetActive(false); // Ensure all buttons start inactive
        }
    }

    public void BringButtonsForward()
    {
        // Activate and animate all buttons simultaneously
        foreach (var button in buttons)
        {
            ActivateButton(button);
        }
    }

    private void ActivateButton(GameObject button)
    {
        // Set the button active
        button.SetActive(true);

        // Animate scale-in with DoTween
        button.transform.localScale = Vector3.zero; // Reset scale
        button.transform.DOScale(originalScales[button], animationDuration).SetEase(Ease.OutBack);
    }
}