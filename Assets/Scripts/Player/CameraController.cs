using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonPattern<CameraController>
{
    public float camMoveSpeed = 5f;
    public float camZoomSpeed = 5f;
    public Transform boundingBox;
    public Vector3 distanceFromGuard;
    public float smoothSpeed;

    [HideInInspector] public Transform selectedGuard;
    [HideInInspector] public bool followGuard;

    private Vector3 newCamPos;

    // Update is called once per frame
    void Update()
    {
        if (followGuard == false)
        {
            //Get edges of bounding box
            float minX = boundingBox.position.x - (boundingBox.localScale.x / 2);
            float maxX = boundingBox.position.x + (boundingBox.localScale.x / 2);
            float minZ = boundingBox.position.z - (boundingBox.localScale.z / 2);
            float maxZ = boundingBox.position.z + (boundingBox.localScale.z / 2);
            float minY = boundingBox.position.y - (boundingBox.localScale.y / 2);
            float maxY = boundingBox.position.y + (boundingBox.localScale.y / 2);

            //Set newCamPos to the current camera position + input
            newCamPos = transform.position + PlayerInputs.Instance.WASDMovement * camMoveSpeed * Time.deltaTime;
            newCamPos.y = transform.position.y + PlayerInputs.Instance.ScrollingInput * camZoomSpeed * Time.deltaTime;

            //Clamp newCamPos within the bounding box edges
            newCamPos.x = Mathf.Clamp(newCamPos.x, minX, maxX);
            newCamPos.z = Mathf.Clamp(newCamPos.z, minZ, maxZ);
            newCamPos.y = Mathf.Clamp(newCamPos.y, minY, maxY);

            //Update the camera's position
            transform.position = newCamPos;
        }
    }

    private void LateUpdate()
    {
        if (followGuard)
        {
            Vector3 desiredPosition = selectedGuard.position + distanceFromGuard;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }       
    }
}
