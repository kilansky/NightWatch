using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonPattern<CameraController>
{
    public float camMoveSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.position += PlayerInputs.Instance.CameraMovement * camMoveSpeed * Time.deltaTime;
    }
}
