using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : SingletonPattern<CameraController>
{
    public float camMoveSpeed = 5f;
    public float camZoomSpeed = 5f;
    public Transform boundingBox;
    public Transform camFollowPoint;
    public float minFollowDist = 10f;
    public float maxFollowDist = 50f;

    [HideInInspector] public Transform selectedGuard;
    [HideInInspector] public bool looseFollow; //While true, allow WASD input to break away from the current follow target

    private Vector3 newCamFollowPos;
    private float newFollowDist;
    private CinemachineVirtualCamera vcam;
    private float distToPanWidth;
    private float distToPanHeight;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = camFollowPoint;

        distToPanWidth = Screen.width * 0.05f;
        distToPanHeight = Screen.height * 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        //If there was WASD input on the last frame and the camera is loosely following a target, end the follow
        if (looseFollow && PlayerInputs.Instance.WASDMovement != Vector3.zero)
            EndCameraFollow();

        //Get edges of bounding box
        float minX = boundingBox.position.x - (boundingBox.localScale.x / 2);
        float maxX = boundingBox.position.x + (boundingBox.localScale.x / 2);
        float minZ = boundingBox.position.z - (boundingBox.localScale.z / 2);
        float maxZ = boundingBox.position.z + (boundingBox.localScale.z / 2);

        //Debug.Log(mouseScreenEdgeInput());
        Vector3 mouseEdgeInput = mouseScreenEdgeInput().normalized;

        //Set newCamPos to the current camera position, + input if not following a guard
        if (PlayerInputs.Instance.WASDMovement != Vector3.zero) //Prioritize WASD movement
            newCamFollowPos = camFollowPoint.position + PlayerInputs.Instance.WASDMovement * camMoveSpeed * Time.deltaTime;
        else //Allow Mouse Panning if not pressing WASD
            newCamFollowPos = camFollowPoint.position + mouseEdgeInput * camMoveSpeed * Time.deltaTime;

        //Clamp newCamPos within the bounding box edges
        newCamFollowPos.x = Mathf.Clamp(newCamFollowPos.x, minX, maxX);
        newCamFollowPos.z = Mathf.Clamp(newCamFollowPos.z, minZ, maxZ);

        //Update the camera's position
        camFollowPoint.position = newCamFollowPos;

        //Set follow distance (zoom) based on scroll wheel input
        var transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
        newFollowDist = transposer.m_FollowOffset.y + PlayerInputs.Instance.ScrollingInput * camZoomSpeed * Time.deltaTime;
        newFollowDist = Mathf.Clamp(newFollowDist, minFollowDist, maxFollowDist);
        transposer.m_FollowOffset.y = newFollowDist;
    }

    public Vector3 mouseScreenEdgeInput()
    {
        Vector3 mouseEdgeInput = Vector3.zero;

        //Horizontal Mouse Input
        if (PlayerInputs.Instance.MousePosition.x < Screen.width && PlayerInputs.Instance.MousePosition.x > Screen.width - distToPanWidth)
            mouseEdgeInput += new Vector3(1, 0, 0); //Move Right
        else if (PlayerInputs.Instance.MousePosition.x > 0 && PlayerInputs.Instance.MousePosition.x < distToPanWidth)
            mouseEdgeInput += new Vector3(-1, 0, 0); //Move Left

        //Vertical Mouse Input
        if (PlayerInputs.Instance.MousePosition.y < Screen.height && PlayerInputs.Instance.MousePosition.y > Screen.height - distToPanHeight)
            mouseEdgeInput += new Vector3(0, 0, 1); //Move Up
        else if (PlayerInputs.Instance.MousePosition.y > 0 && PlayerInputs.Instance.MousePosition.y < distToPanHeight)
            mouseEdgeInput += new Vector3(0, 0, -1); //Move Down

        return mouseEdgeInput;
    }

    //Begin following a set target, lockFollow will prevent camera controls with WASD if true
    public void BeginCameraFollow(Transform target, bool loose)
    {
        vcam.Follow = target;
        looseFollow = loose;
    }

    //End following a target and return to normal camera controls
    public void EndCameraFollow()
    {
        camFollowPoint.position = new Vector3(vcam.Follow.position.x, 0, vcam.Follow.position.z);
        vcam.Follow = camFollowPoint;
        looseFollow = false;
    }
}
