using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpriteAnimator  : MonoBehaviour
{
    [Header("Sprite Animation Settings")]
    public Image imageComponent;         // Reference to the UI Image component
    public Sprite[] sprites;             // Array of sprites for animation frames
    public float animationDuration = 1f; // Total animation time for one loop

    [Header("Floating Animation Settings")]
    public Transform floatTarget;        // Reference to the Transform to float
    public float floatDistance = 10f;    // Distance to float on Y-axis
    public float floatDuration = 1f;     // Time for one up-and-down float cycle

    private void Start()
    {
        AnimateSprites();
        //AnimateFloating();
    }

    private void AnimateSprites()
    {
        // Calculate frame interval based on number of sprites and animation duration
        float frameDuration = animationDuration / sprites.Length;

        // Create a sequence for sprite animation with infinite looping
        Sequence spriteSequence = DOTween.Sequence();
        foreach (Sprite sprite in sprites)
        {
            spriteSequence.AppendCallback(() => imageComponent.sprite = sprite);
            spriteSequence.AppendInterval(frameDuration);
        }
        spriteSequence.SetLoops(-1); // Loop the sprite sequence indefinitely
    }

    private void AnimateFloating()
    {
        // Apply a floating animation on the Y-axis using a sequence
        floatTarget.DOMoveY(floatTarget.position.y + floatDistance, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo); // Move up and down in a Yoyo loop
    }
}