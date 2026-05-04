using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Find the main camera once when the scene starts
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Only run if camera was found
        if (mainCamera == null) return;

        // Make this label always look at the camera
        transform.LookAt(mainCamera.transform);

        // Flip 180 degrees so text faces TOWARD camera not away
        transform.Rotate(0, 180, 0);
    }
}