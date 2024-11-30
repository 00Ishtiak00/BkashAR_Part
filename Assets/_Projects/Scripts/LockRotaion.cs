using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        // Store the initial local rotation of the GameObject
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Lock X and Z local rotation, but allow Y local rotation
        transform.localRotation = Quaternion.Euler(
            initialRotation.eulerAngles.x, // Lock X axis
            transform.localRotation.eulerAngles.y, // Allow Y axis
            initialRotation.eulerAngles.z  // Lock Z axis
        );
    }
}