using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    public enum ControlMode { Idle, Patrol, Click, Manual }

    public float moveSpeed = 6f;

    public ControlMode currControlMode = ControlMode.Idle;

    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, SecurityPlacement.Instance.floorMask))
        {
            Vector3 forward = hit.point - transform.position;
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        velocity = PlayerInputs.Instance.CameraMovement * moveSpeed;
        transform.position += PlayerInputs.Instance.CameraMovement * moveSpeed * Time.deltaTime;
    }
}
