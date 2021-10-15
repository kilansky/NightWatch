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
    [HideInInspector] public bool followGuard;

    private Vector3 newCamFollowPos;
    private float newFollowDist;
    private CinemachineVirtualCamera vcam;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = camFollowPoint;
    }

    // Update is called once per frame
    void Update()
    {
        //If there was WASD input on the last frame and the camera is loosely following a target, end the follow
        if (PlayerInputs.Instance.WASDMovement != Vector3.zero && vcam.Follow)
            EndCameraFollow();

        //Get edges of bounding box
        float minX = boundingBox.position.x - (boundingBox.localScale.x / 2);
        float maxX = boundingBox.position.x + (boundingBox.localScale.x / 2);
        float minZ = boundingBox.position.z - (boundingBox.localScale.z / 2);
        float maxZ = boundingBox.position.z + (boundingBox.localScale.z / 2);

        //Set newCamPos to the current camera position, + input if not following a guard
        newCamFollowPos = camFollowPoint.position + PlayerInputs.Instance.WASDMovement * camMoveSpeed * Time.deltaTime;

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

    //Begin following a set target, lockFollow will prevent camera controls with WASD if true
    public void BeginCameraFollow(Transform target, bool lockFollow)
    {
        vcam.Follow = target;
        followGuard = lockFollow;
    }

    //End following a target and return to normal camera controls
    public void EndCameraFollow()
    {
        camFollowPoint.position = new Vector3(vcam.Follow.position.x, 0, vcam.Follow.position.z);
        vcam.Follow = camFollowPoint;
        followGuard = false;
    }

    /*
    private void LateUpdate()
    {
        if (followGuard)
        {
            Vector3 desiredPosition = selectedGuard.position + distanceFromGuard;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }       
    }
    */
}
