using UnityEngine;

using Unity.Cinemachine;




public class CameraController : MonoBehaviour
{
    
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private CinemachineCamera cinemachineVirtualCamera;

    private float minX = -100f;
    private float maxX = 100f;
    private float minZ = -100f;
    private float maxZ = 100f;
    
    
    private float fieldOfView;
    private float targetFieldOfView;

    private void Start() {
        fieldOfView = cinemachineVirtualCamera.Lens.OrthographicSize;
        targetFieldOfView = fieldOfView;
    }

    private void Update() {
        

        
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    
    private void HandleMovement()
    {
        // Get input for movement
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        // Translate input into camera-relative movement
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Flatten forward and right to ignore vertical movement (stick to the XZ plane)
        forward.y = 0f;
        right.y = 0f;

        // Normalize vectors to maintain consistent speed
        forward.Normalize();
        right.Normalize();

        // Combine input with direction vectors
        Vector3 moveDirection = forward * verticalInput + right * horizontalInput;

        // Apply movement
        Vector3 newPosition = transform.position + moveDirection * panSpeed * Time.deltaTime;

        // Clamp position within bounds
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        transform.position = newPosition;
    }


    private void HandleZoom(){
        float zoomAmount = 2f;

        targetFieldOfView += -Input.mouseScrollDelta.y * zoomAmount;

        float minFieldOfView = 25;
        float maxFieldOfView = 60;
        float zoomSpeed = 10f;

        fieldOfView = Mathf.Lerp(fieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);

        targetFieldOfView = Mathf.Clamp(targetFieldOfView, minFieldOfView, maxFieldOfView);
        cinemachineVirtualCamera.Lens.OrthographicSize = targetFieldOfView;
    }

    private void HandleRotation()
    {
        float rotationDirection = 0f;

        
        if (Input.GetKey(KeyCode.Q))
        {
            rotationDirection = -1f; 
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotationDirection = 1f; 
        }

        // Apply rotation
        if (rotationDirection != 0f)
        {
            RotateCamera(rotationDirection);
        }
    }

    private void RotateCamera(float direction)
    {
        
        
        float rotationStep = rotationSpeed * Time.deltaTime * direction;

        
        transform.Rotate(0f, rotationStep, 0f, Space.World);

       
        Vector3 eulerAngles = transform.eulerAngles;
        transform.eulerAngles = new Vector3(35f, eulerAngles.y, 0f);
    }

    
    
}
