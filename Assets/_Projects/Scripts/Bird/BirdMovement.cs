using UnityEngine;
using DG.Tweening;

public class BirdMovement : MonoBehaviour
{
   public RectTransform canvasRect; // Assign your Canvas RectTransform here
    public GameObject birdPrefab; // Assign your bird prefab here
    public int birdCount = 5; // Number of birds to spawn
    public float minMoveDuration = 2f; // Minimum time for a bird to cross the screen
    public float maxMoveDuration = 5f; // Maximum time for a bird to cross the screen
    public float spawnYOffset = 100f; // Range for random vertical spawn positions
    public float minSpawnDelay = 0.5f; // Minimum delay between bird spawns
    public float maxSpawnDelay = 2f; // Maximum delay between bird spawns

    private float canvasWidth;

    void Start()
    {
        // Get canvas width (half-width since the origin is in the center)
        canvasWidth = canvasRect.sizeDelta.x / 2;

        // Spawn and initialize birds
        for (int i = 0; i < birdCount; i++)
        {
            SpawnBirdWithDelay(i);
        }
    }

    void SpawnBirdWithDelay(int index)
    {
        // Random delay before each bird starts moving
        float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
        Invoke(nameof(SpawnBird), delay);
    }

    void SpawnBird()
    {
        // Instantiate the bird prefab as a child of the canvas
        GameObject birdInstance = Instantiate(birdPrefab, canvasRect);

        // Get the RectTransform of the bird instance
        RectTransform birdRect = birdInstance.GetComponent<RectTransform>();

        // Place the bird off-screen on the left side
        birdRect.anchoredPosition = GetStartPosition();

        // Start the left-to-right movement
        MoveBirdAcrossScreen(birdRect);
    }

    void MoveBirdAcrossScreen(RectTransform bird)
    {
        // Target position is off-screen on the right side
        Vector2 targetPosition = new Vector2(canvasWidth + 100f, bird.anchoredPosition.y);

        // Randomized duration for the bird to cross the screen
        float duration = Random.Range(minMoveDuration, maxMoveDuration);

        // Animate the bird moving from left to right
        bird.DOAnchorPosX(targetPosition.x, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // Once the bird reaches the right side, reposition it to the left and repeat
                bird.anchoredPosition = GetStartPosition();
                MoveBirdAcrossScreen(bird);
            });
    }

    Vector2 GetStartPosition()
    {
        // Spawn off-screen on the left, with a random vertical position within the Y bounds
        float x = -canvasWidth - 100f; // Slightly off-screen to the left
        float y = Random.Range(-canvasRect.sizeDelta.y / 2 + spawnYOffset, canvasRect.sizeDelta.y / 2 - spawnYOffset);

        return new Vector2(x, y);
    }
}