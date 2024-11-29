using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Glow : MonoBehaviour
{
    [Header("Sprite Animation Settings")]
    public Image imageComponent;         // Reference to the UI Image component
    public Sprite[] sprites;             // Array of sprites for animation frames
    public float animationDuration = 1f; // Total animation time for one loop

    [Header("Tween Settings")]
    public TransformTweener transferTweener; // Reference to the TransferTweener script
    
    

    private void Start()
    {
        PlayFirstSequence();
    }

    private void PlayFirstSequence()
    {
        if (sprites.Length == 0 || imageComponent == null || transferTweener == null) return;

        float frameDuration = animationDuration / sprites.Length;

        // Create the first sequence
        Sequence firstSequence = DOTween.Sequence();

        foreach (Sprite sprite in sprites)
        {
            firstSequence.AppendCallback(() => imageComponent.sprite = sprite);
            firstSequence.AppendInterval(frameDuration);
        }

        // On complete, call TweenMap and start the looping sequence
        firstSequence.OnComplete(() =>
        {
            transferTweener.TweenMap();
            SetImageTransparencyToZero();
            //PlayLoopingSequence();
        });

        firstSequence.Play();
    }
    private void SetImageTransparencyToZero()
    {
        if (imageComponent != null)
        {
            Color color = imageComponent.color;
            color.a = 0f;
            imageComponent.color = color;
        }
    }
    

    private void PlayLoopingSequence()
    {
        if (sprites.Length == 0 || imageComponent == null) return;

        float frameDuration = animationDuration / sprites.Length;

        // Create the looping sequence
        Sequence loopingSequence = DOTween.Sequence();

        foreach (Sprite sprite in sprites)
        {
            loopingSequence.AppendCallback(() => imageComponent.sprite = sprite);
            loopingSequence.AppendInterval(frameDuration);
        }

        // Set it to loop indefinitely
        loopingSequence.SetLoops(-1);
        loopingSequence.Play();
    }
}