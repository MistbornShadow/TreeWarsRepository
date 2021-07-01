using UnityEngine;


public class CameraController : MonoBehaviour

{

    // Public Variables

    // How quickly the camera moves

    public float panSpeed = 10f;

    // The minimum distance of the mouse cursor from the screen edge required to pan the camera

    public float borderWidth = 10f;

    // Boolean to control if moving the mouse within the borderWidth distance will pan the camera

    public bool edgeScrolling = true;

    // A placeholder for a reference to the camera in the scene

    public Camera cam;

    // Floats to hold reference to the mouse position, no values assigned to these yet

    private float mouseX, mouseY;


    void Start()

    {

        // On start, get a reference to the Main Camera

        cam = Camera.main;

    }


    void Update()

    {

        Movement();

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -9f, 16f),
            -4.3f,
            -1f);
    }


    void Movement()

    {

        // Local variable to hold the camera target's position during each frame

        Vector3 pos = transform.position;

        // Local variable to reference the direction the camera is facing (Which is driven by the Camera target's rotation)

        Vector3 forward = transform.forward;

        // Ensure the camera target doesn't move up and down

        forward.y = 0;

        // Normalize the X, Y & Z properties of the forward vector to ensure they are between 0 & 1

        forward.Normalize();


        // Local variable to reference the direction the camera is facing + 90 clockwise degrees (Which is driven by the Camera target's rotation)

        Vector3 right = transform.right;

        // Ensure the camera target doesn't move up and down

        right.y = 0;

        // Normalize the X, Y & Z properties of the right vector to ensure they are between 0 & 1

        right.Normalize();

        // Move the camera (camera_target) Right relative to current rotation if "D" is pressed or if the mouse moves within the borderWidth distance from the right edge of the screen

        if (Input.GetKey("d") || edgeScrolling == true && Input.mousePosition.x >= Screen.width - borderWidth)

        {

            pos += right * panSpeed * Time.deltaTime;

        }


        // Move the camera (camera_target) Left relative to current rotation if "A" is pressed or if the mouse moves within the borderWidth distance from the left edge of the screen

        if (Input.GetKey("a") || edgeScrolling == true && Input.mousePosition.x <= borderWidth)

        {

            pos -= right * panSpeed * Time.deltaTime;

        }


        // Setting the camera target's position to the modified pos variable

        transform.position = pos;

    }

    // End of file

}