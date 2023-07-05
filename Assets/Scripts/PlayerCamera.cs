using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCamera : MonoBehaviour
{

    //TO DO

    //Hoding MMB not only allows rotation but general camera control
    //Zooming moves the camera towards the mouse



    [Header("WASD")]
    [SerializeField] float fastSpeed;
    [SerializeField] float normalSpeed;
    [SerializeField] float distanceMultiplier;
    [SerializeField] float WASDTime;

    [Header("Rotation")]
    [SerializeField] float rotationSpeed;

    [Header("Zoom")]
    [SerializeField] float zoomSpeed;
    [SerializeField] float zoomTime;

    [Header("Misc")]
    [SerializeField] Transform playerCamera;

    //WASD
    float movementSpeed;
    Vector3 newPos;

    //Rotation
    Vector3 currentRot;
    Vector3 startRot;
    Quaternion newRot;

    //Zoom
    float desiredZoom = 0f;


    void Start(){
        newPos = transform.position;
        newRot = transform.rotation;
        desiredZoom = Vector3.Distance(playerCamera.position, transform.position);
    }

    void Update(){
        handleKeyboardInput();
        handleMouseinput();
    }


    void handleMouseinput(){
        HandleZoom();
        HandleRotation();
    }

    void handleKeyboardInput()  //WASD Movement
    {

        if (Input.GetKey(KeyCode.LeftShift))
            movementSpeed = fastSpeed;
        else
            movementSpeed = normalSpeed;

        float distanceToGround = Vector3.Distance(transform.localPosition, playerCamera.localPosition);
        movementSpeed *= (distanceToGround) / 1000 * distanceMultiplier;


        if (Input.GetKey(KeyCode.W))
            newPos += transform.forward * movementSpeed;

        if (Input.GetKey(KeyCode.S))
            newPos += transform.forward * -movementSpeed;

        if (Input.GetKey(KeyCode.D))
            newPos += transform.right * movementSpeed;

        if (Input.GetKey(KeyCode.A))
            newPos += transform.right * -movementSpeed;


        transform.position = Vector3.Lerp(transform.position, newPos, WASDTime * Time.deltaTime);

    }


    void HandleZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
            desiredZoom -= Input.mouseScrollDelta.y * zoomSpeed;
            //clamp the distance inside if 

        Vector3 direction = (playerCamera.position - transform.position).normalized;
        Vector3 desiredPos = transform.position + direction * desiredZoom;

        // Convert the desired position to the local space of the rig
        Vector3 desiredLocalPos = transform.InverseTransformPoint(desiredPos);

        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, desiredLocalPos, zoomTime * Time.deltaTime);
    }


    void HandleRotation()
    {
        //Rotation
        if (Input.GetMouseButtonDown(2))
            startRot = Input.mousePosition;

        if (Input.GetMouseButton(2))
        {
            currentRot = Input.mousePosition;

            Vector3 difference = startRot - currentRot;
            startRot = currentRot;

            newRot *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);

    }
}
