using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform rotationPoint;
    public GameObject rotationUI;
    public Transform uiMarker;
    public LayerMask uiLayerMask;

    [Range(0, 360)] public float minRotation = 0f;
    [Range(0, 360)] public float maxRotation = 180f;

    public float autoRotateSpeed = 1f;
    public float autoRotateHoldTime = 1.5f;

    [HideInInspector] public bool autoRotateUpgrade = false; //True if this camera has the auto-rotation upgrade
    [HideInInspector] public bool canRotate = true; //True if this camera is not disabled through hacking

    private Camera mainCamera;
    private Vector3 rotateDir;
    private Quaternion lookDir;


    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if(rotationUI.activeSelf)
        {
            SetRotationDirection();
            SetUIMarkerPosition();

            //Rotate the camera to the mouse position while left click is held and another object is NOT selected
            if (PlayerInputs.Instance.LeftClickHeld && !SecuritySelection.Instance.selectedObject)
                SetCameraRotation();

            //Disable the rotation UI when right click is pressed or another object is selected
            if (PlayerInputs.Instance.RightClickPressed || SecuritySelection.Instance.selectedObject)
                DisableUI();
        }
    }

    //Cast a ray to the mouse position to find the direction to rotate towards
    private void SetRotationDirection()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;

        //Check where the mouse was clicked in worldspace
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, uiLayerMask))
        {
            //Set direction for the camera to look towards
            rotateDir = (hit.point - rotationPoint.position).normalized;
            rotateDir = new Vector3(rotateDir.x, 0, rotateDir.z);
            lookDir = Quaternion.LookRotation(rotateDir, Vector3.up);
            lookDir *= Quaternion.Euler(0, -90f, 0);
        }
    }

    //Positions the UI Marker on the rotation UI to show the angle the camera will be rotated
    private void SetUIMarkerPosition()
    {
        float yRotValue = lookDir.eulerAngles.y;
        float rootAngle = transform.root.rotation.eulerAngles.y;

        uiMarker.localRotation = Quaternion.Euler(0, 0, yRotValue - rootAngle);
    }

    //Sets the rotation of the camera
    private void SetCameraRotation()
    {
        rotationPoint.rotation = lookDir;
        SetMinMaxAngles();
    }

    //Prevent camera from facing backwards toward the wall
    private void SetMinMaxAngles()
    {
        //Get the invalid midpoint between the min and max
        float invalidMidpoint = ((maxRotation + minRotation) / 2f) + 180f;
        float currCamRotation = rotationPoint.localRotation.eulerAngles.y;

        if (currCamRotation < minRotation || currCamRotation > maxRotation)
        {
            if(minRotation > 0 && currCamRotation < minRotation || currCamRotation >= invalidMidpoint)
                rotationPoint.localRotation = Quaternion.Euler(0, minRotation, 0);//Snap to min rotation
            else if (currCamRotation < invalidMidpoint)
                rotationPoint.localRotation = Quaternion.Euler(0, maxRotation, 0);//snap to max rotation
        }
    }

    //Disables the rotation UI object that this script is attached to
    public void DisableUI()
    {
        rotationUI.SetActive(false);
    }

    //Begins the coroutines to start rotating back and forth on a loop
    public void BeginAutoRotating()
    {
        autoRotateUpgrade = true;
        canRotate = true;

        minRotation = 45f;
        maxRotation = 135f;

        StartCoroutine(AutoRotateCameraToMax());
    }

    //Rotate to the maxRotation value
    public IEnumerator AutoRotateCameraToMax()
    {
        while(canRotate && rotationPoint.localRotation.eulerAngles.y < maxRotation - 0.1f)
        {
            rotationPoint.Rotate(0, 0.1f * autoRotateSpeed, 0, Space.Self);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(autoRotateHoldTime);
        StartCoroutine(AutoRotateCameraToMin());
    }

    //Rotate to the minRotation value
    public IEnumerator AutoRotateCameraToMin()
    {
        while (canRotate && rotationPoint.localRotation.eulerAngles.y > minRotation + 0.1f)
        {
            rotationPoint.Rotate(0, -0.1f * autoRotateSpeed, 0, Space.Self);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(autoRotateHoldTime);
        StartCoroutine(AutoRotateCameraToMax());
    }
}
