using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveEffectGrouped: MonoBehaviour
{
   [Header("Assign GameObjects")]
    public List<GameObject> Group1 = new List<GameObject>();
    public List<GameObject> Group2 = new List<GameObject>();
    public List<GameObject> Group3 = new List<GameObject>();

    [Header("Animation Settings")]
    public float scaleFactor = 1.2f; // How much to scale up
    public float animationDuration = 0.5f; // Duration of each animation
    public float delayBetweenWaves = 0.2f; // Delay between each wave

    // Dictionary to store the initial scales of objects
    private Dictionary<GameObject, Vector3> initialScales = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        // Store the initial scale of each object
        StoreInitialScales();

        InvokeRepeating(nameof(StartWaveAnimation), 0, 5f);
        // Start the wave animation
        //StartWaveAnimation();
    }

    private void StoreInitialScales()
    {
        // Combine all groups and store their initial scales
        foreach (GameObject obj in Group1) SaveInitialScale(obj);
        foreach (GameObject obj in Group2) SaveInitialScale(obj);
        foreach (GameObject obj in Group3) SaveInitialScale(obj);
    }

    private void SaveInitialScale(GameObject obj)
    {
        if (obj != null && !initialScales.ContainsKey(obj))
        {
            initialScales[obj] = obj.transform.localScale; // Store the object's initial scale
        }
    }

    [ContextMenu("StartWaveAnimation")]
    public void StartWaveAnimation()
    {
        // Animate each group sequentially
        AnimateGroup(Group1, 0);
        AnimateGroup(Group2, delayBetweenWaves);
        AnimateGroup(Group3, delayBetweenWaves * 2);
    }

    private void AnimateGroup(List<GameObject> group, float initialDelay)
    {
        for (int i = 0; i < group.Count; i++)
        {
            GameObject obj = group[i];
            if (obj == null) continue;

            float delay = initialDelay + (i * 0.1f); // Slight delay between items in the same group
            Vector3 originalScale = initialScales[obj]; // Retrieve the object's original scale

            // Animate scale in a wave effect
            obj.transform.DOScale(originalScale * scaleFactor, animationDuration)
                .SetEase(Ease.OutQuad)
                .SetDelay(delay)
                .OnComplete(() =>
                {
                    // Return to original scale
                    obj.transform.DOScale(originalScale, animationDuration).SetEase(Ease.InQuad);
                });
        }
    }
}