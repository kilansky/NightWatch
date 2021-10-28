using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform rotationPoint;
    public GameObject rotationUI;
    public Transform uiMarker;
    public LayerMask uiLayerMask;

    public float minRotation = 0f;
    public float maxRotation = 180f;

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

            //SetMinMaxAngles();
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
    }

    private void SetMinMaxAngles()
    {
        //Prevent camera from facing backwards toward the wall
        Vector3 lookAngle = lookDir.eulerAngles - transform.root.rotation.eulerAngles;
        if (lookAngle.y < 0)
            lookAngle += new Vector3(0, 360f, 0);

        float rootAngle = transform.root.rotation.eulerAngles.y;

        //Debug.Log("lookDir.eulerAngles: " + lookDir.eulerAngles);
        //Debug.Log("rootAngle: " + rootAngle);
        //Debug.Log("\n");

        float midRangeValue = (maxRotation + minRotation) / 2f; //90 degrees in standard case, halfway between min and max values

        float adjustedMinAngle = minRotation + rootAngle;
        if (adjustedMinAngle > 360)
            adjustedMinAngle -= 360f;

        float adjustedMidRangeValue = midRangeValue + rootAngle;
        if (adjustedMidRangeValue > 360)
            adjustedMidRangeValue -= 360f;

        float adjustedMaxAngle = maxRotation + rootAngle;
        if (adjustedMaxAngle > 360)
            adjustedMaxAngle -= 360f;

        Debug.Log("adjustedMinAngle: " + adjustedMinAngle);
        Debug.Log("adjustedMidRangeValue: " + adjustedMidRangeValue);
        Debug.Log("adjustedMaxAngle: " + adjustedMaxAngle);
        Debug.Log("lookAngle: " + lookAngle);
        Debug.Log("\n");

        if (lookAngle.y < minRotation)
        {
            if (lookAngle.y >= maxRotation + ((minRotation + maxRotation) / 2) - rootAngle) //Snap to min rotation
                lookDir = Quaternion.Euler(0, minRotation - rootAngle, 0);
            else //snap to max rotation
                lookDir = Quaternion.Euler(0, maxRotation - rootAngle, 0);
        }
    }

    //Disables the rotation UI object that this script is attached to
    public void DisableUI()
    {
        rotationUI.SetActive(false);
    }
}
