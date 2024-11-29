using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveEffect : MonoBehaviour
{
    [SerializeField] private List<List<Transform>> buttonGroups; // Each group contains buttons (assign in Inspector)
    [SerializeField] private float waveHeight = -15f;             // Peak Z position
    [SerializeField] private float waveDuration = 1f;           // Duration for each group's animation
    [SerializeField] private float groupDelay = 0.5f;           // Delay between each group's animation

    void Start()
    {
        CreateWaveEffect();

    }

    private void CreateWaveEffect()
    {
        Sequence waveSequence = DOTween.Sequence();

        // Animate each group sequentially
        for (int groupIndex = 0; groupIndex < buttonGroups.Count; groupIndex++)
        {
            List<Transform> group = buttonGroups[groupIndex];

            foreach (Transform button in group)
            {
                // Animate all buttons in the current group simultaneously
                waveSequence.Join(
                    button.DOLocalMoveZ(waveHeight, waveDuration / 2)
                        .SetEase(Ease.OutQuad) // Smooth ease up
                );
                waveSequence.Join(
                    button.DOLocalMoveZ(0, waveDuration / 2)
                        .SetEase(Ease.InQuad) // Smooth ease down
                );
            }

            // Add a delay between groups
            if (groupIndex < buttonGroups.Count - 1) // Avoid unnecessary delay after the last group
            {
                waveSequence.AppendInterval(groupDelay);
            }
        }

        // Callback for when all groups complete their animations
        waveSequence.OnComplete(() =>
        {
            Debug.Log("All groups have completed their wave animations!");
        });

        waveSequence.Play();
    }
}