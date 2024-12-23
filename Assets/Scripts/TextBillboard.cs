using UnityEngine;

public class TextBillboard : MonoBehaviour
{
    private Transform playerCamera;

    void Start()
    {
        playerCamera = Camera.main.transform; // Assumes main camera represents the player
    }

    void LateUpdate()
    {
        if (playerCamera != null)
        {
            // Rotate to face the camera
            transform.LookAt(transform.position + playerCamera.forward);
        }
    }
}
