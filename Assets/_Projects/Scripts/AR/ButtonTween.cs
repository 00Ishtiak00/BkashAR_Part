using UnityEngine;
using DG.Tweening; // Include DoTween namespace
public class ButtonTween : MonoBehaviour
{
    [Header("Target Transform")] public Transform targetTransform; // Assign the target transform in the Inspector

    [Header("Target Z Position")] public float targetZPosition = 0f; // The desired Z position

    [Header("Tween Settings")] public float duration = 1f; // Duration of the tween
    public Ease easeType = Ease.Linear; // Type of easing for the tween
    

    public void TweenButton()
    {
        if (targetTransform != null)
        {
            // Tween only the Z-axis
            targetTransform.DOMoveZ(targetZPosition, duration).SetEase(easeType);
        }
        else
        {
            Debug.LogWarning("Target Transform is not assigned.");
        }
    }
}
