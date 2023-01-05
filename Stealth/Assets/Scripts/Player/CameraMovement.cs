using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    // Camera and player position variables
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 maxCameraOffset = new Vector3(0, 0, -10f);
    private Vector3 cameraOffset;

    // Zoom to player variables
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float maxZoom = 5f;
    [SerializeField] private float minZoom = 20f;

    // Rotate around player variables
    [SerializeField] private float maxXRotation = 80f;
    [SerializeField] private float minXRotation = 10f;
    [SerializeField] private Vector2 rotationSpeed = new Vector2(180f, 180f);
    private Vector3 previousPosition;

    bool isRotating = false;
    // Code based on Emma Prats tutorial

    void Awake()
    {
        transform.rotation = Quaternion.Euler(minXRotation, 0, 0);
        cameraOffset = maxCameraOffset;
    }

    void Update()
    {
        if(player != null)
        {
            // Camera follows player
            followCamera();
            // Rotates around characther, if right click is being pressed
            if (isRotating)
                rotateAroundCharacter();
        }
    }

    // Sets player transform for camera to follow
    public void setPlayer(Transform _player)
    {
        player = _player;
        previousPosition = transform.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
        transform.position = player.transform.position + cameraOffset;
        transform.LookAt(player.transform.position);
    }

    // Camera follows given player transform
    private void followCamera()
    {
        transform.position = player.position;
        transform.Translate(maxCameraOffset);
    }

    // Changes camera zoom
    public void changeZoom(InputAction.CallbackContext context)
    {
        maxCameraOffset.z = Mathf.Clamp(maxCameraOffset.z + context.ReadValue<Vector2>().y * Time.deltaTime * zoomSpeed, -minZoom, -maxZoom);
    }

    public void rotate(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // Gets mouse position on screen, when right click is pressed
            previousPosition = transform.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
            isRotating = true;
        }
        if (context.canceled)
            isRotating = false;

    }

    // Calculates camera rotation
    private void rotateAroundCharacter()
    {
        Vector3 direction = previousPosition - transform.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
        Vector3 cameraRotation = transform.rotation.eulerAngles;

        cameraRotation.x += direction.y * rotationSpeed.y;
        cameraRotation.y -= direction.x * rotationSpeed.x;

        // Clamp x camera rotation between min and max x rotation
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, minXRotation, maxXRotation);
        transform.rotation = Quaternion.Euler(cameraRotation);

        followCamera();

        previousPosition = transform.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);
    }
}
