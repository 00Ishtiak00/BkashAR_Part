using UnityEngine;
using DG.Tweening; // Include DoTween namespace

public class TransformTweener : MonoBehaviour
{
    [Header("Target Transform")]
    public Transform targetTransform; // Assign the target transform in the Inspector

    [Header("Target Z Position")]
    public float targetZPosition = 0f; // The desired Z position

    [Header("Tween Settings")]
    public float duration = 1f; // Duration of the tween
    public Ease easeType = Ease.Linear; // Type of easing for the tween
    

    
    [Header("Script References")]
    public WaveEffectGrouped WaveEffectGrouped;
    public CameraHandAnimation CameraHandAnimation;
    
    
    
    //public ButtonTween ButtonTween; // Reference to the TransferTweener script
    //public WorldCanvasButtonManager WorldCanvasButtonManager;

    private void Start()
    {
       
    }

    public void TweenMap()
    {
        if (targetTransform != null)
        {
            
            // Tween only the Z-axis
            //targetTransform.DOMoveZ(targetZPosition, duration).SetEase(easeType);
            targetTransform.DOLocalMoveZ(targetZPosition, duration).SetEase(easeType).OnComplete(() =>
            {
                WaveEffectGrouped.StartWaveAnimation();
                Debug.Log("Tween completed!");
                CameraHandAnimation.PlayAnimationSequence();
            });
            //WaveEffectGrouped.StartWaveAnimation();
        }
        else
        {
            Debug.LogWarning("Target Transform is not assigned.");
        }
    }

   
    
}