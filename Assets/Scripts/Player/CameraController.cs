using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonPattern<CameraController>
{
    public float camMoveSpeed = 5f;
    public Transform boundingBox;
    private Vector3 newCamPos;

    // Update is called once per frame
    void Update()
    {
        //Get edges of bounding box
        float minX = boundingBox.position.x - (boundingBox.localScale.x / 2);
        float maxX = boundingBox.position.x + (boundingBox.localScale.x / 2);
        float minZ = boundingBox.position.z - (boundingBox.localScale.z / 2);
        float maxZ = boundingBox.position.z + (boundingBox.localScale.z / 2);
        float minY = boundingBox.position.y - (boundingBox.localScale.y / 2);
        float maxY = boundingBox.position.y + (boundingBox.localScale.y / 2);

        //Set newCamPos to the current camera position + input
        newCamPos = transform.position + PlayerInputs.Instance.CameraMovement * camMoveSpeed * Time.deltaTime;

        //Clamp newCamPos within the bounding box edges
        newCamPos.x = Mathf.Clamp(newCamPos.x, minX, maxX);
        newCamPos.z = Mathf.Clamp(newCamPos.z, minZ, maxZ);
        newCamPos.y = Mathf.Clamp(newCamPos.y, minY, maxY);

        //Update the camera's position
        transform.position = newCamPos;
    }
}
