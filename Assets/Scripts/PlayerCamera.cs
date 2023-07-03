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

    float movementSpeed;
    Vector3 newPos;
    Vector3 currentRot;
    Vector3 startRot;
    Quaternion newRot;
    Vector3 newZoom;

    void Start(){
        newPos = transform.position;
        newRot = transform.rotation;
        newZoom = playerCamera.localPosition;
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
        //Zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            Vector3 direction = playerCamera.localPosition - transform.localPosition;
            direction.Normalize();
            newZoom -= Input.mouseScrollDelta.y * direction * zoomSpeed;
        }

        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, newZoom, zoomTime * Time.deltaTime);
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
