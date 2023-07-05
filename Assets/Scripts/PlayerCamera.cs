using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

    //TO DO

    //Hoding MMB not only allows rotation but general camera control
    //Zooming moves the camera towards the mouse
    //maxCameraCeiling currently not used to limit the camera



    [Header("WASD")]
    [SerializeField] float fastSpeed;
    [SerializeField] float normalSpeed;
    [SerializeField] float distanceMultiplier;
    [SerializeField] float WASDTime;

    [Header("Rotation")]
    [SerializeField] float rotationSpeed;
    [SerializeField] float upperBound;
    [SerializeField] float maxUpper = 100f;
    [SerializeField] float maxLower = 200f;
    [SerializeField] float lowerBound;
    [SerializeField] float boundOffsetMultiplier;

    [Header("Zoom")]
    [SerializeField] float zoomSpeed;
    [SerializeField] float zoomTime;

    [Header("Misc")]
    [SerializeField] Transform playerCamera;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float maxCameraCeiling = 5000f;

    //WASD
    float movementSpeed;
    Vector3 newPos;
    

    //Rotation
    Vector3 currentRot;
    Vector3 startRot;
    float currentYRotation;
    float currentXRotation;

    //Zoom
    float desiredZoom = 0f;

    //Common
    float distanceToGround; //WASD + rot


    void Start(){
        newPos = transform.position;
        desiredZoom = Vector3.Distance(playerCamera.position, transform.position);

        currentXRotation = 170f;  //this is likely a terrible solution
        currentYRotation = 0f;
    }

    void Update(){
        calculateCommon();
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

        //increase or lower speed based on distance to ground
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

        //Convert the desired position to the local space of the rig
        Vector3 desiredLocalPos = transform.InverseTransformPoint(desiredPos);

        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, desiredLocalPos, zoomTime * Time.deltaTime);
    }


    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(2))
            startRot = Input.mousePosition;

        if (Input.GetMouseButton(2))
        {
            currentRot = Input.mousePosition;
        }


        Vector3 difference = startRot - currentRot;
        startRot = currentRot;

        //Increment currentYRotation and currentXRotation
        currentYRotation -= difference.x;
        currentXRotation += difference.y;

        //Calculating distance offset, closer to ground -> looking higher
        float multiplier = distanceToGround / maxCameraCeiling;
        multiplier *= multiplier; //pow 2

        float currentUpper = lerpValues(upperBound, maxUpper, multiplier);
        float currentLower = lerpValues(lowerBound, maxLower, multiplier);

        //Clamp currentXRotation
        currentXRotation = Mathf.Clamp(currentXRotation, currentUpper, currentLower);

        Quaternion targetYRotation = Quaternion.Euler(0, currentYRotation / 5f, 0);
        Quaternion targetXRotation = Quaternion.Euler(currentXRotation / 5f, 0, 0);

        //Rotate the rig (parent) around world's Y axis
        transform.rotation = Quaternion.Lerp(transform.rotation, targetYRotation, rotationSpeed * Time.deltaTime);

        //Rotate the camera around its local X axis
        playerCamera.localRotation = Quaternion.Lerp(playerCamera.localRotation, targetXRotation, rotationSpeed * Time.deltaTime);
    }


    void calculateCommon()
    {
        //distance from camera to ground
        if (Physics.Raycast(playerCamera.position, Vector3.down, out RaycastHit hit, maxCameraCeiling, groundMask))
            distanceToGround = hit.distance;
        else
            distanceToGround = maxCameraCeiling;

    }

    float lerpValues(float a, float b, float t)
    {
        if (a > b)  //set a to lower
            (a, b) = (b, a);

        float dist = b - a;
        dist *= t;

        return a + dist;
    }
}
