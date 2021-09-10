using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonPattern<CameraController>
{
    public float camMoveSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += PlayerInputs.Instance.CameraMovement * camMoveSpeed * Time.deltaTime;
    }
}
