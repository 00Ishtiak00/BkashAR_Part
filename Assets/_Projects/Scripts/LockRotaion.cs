using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Quaternion initialRotation;

    void Start()
    {
        // Store the initial rotation of the GameObject
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Lock X and Z rotation, but allow Y rotation
        transform.rotation = Quaternion.Euler(
            initialRotation.eulerAngles.x, // Lock X axis
            transform.rotation.eulerAngles.y, // Allow Y axis
            initialRotation.eulerAngles.z  // Lock Z axis
        );
    }
}