using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SecurityPlacement : SingletonPattern<SecurityPlacement>
{
    public GameObject cctvPrefab;
    public GameObject laserPrefab;
    public GameObject guardPrefab;
    public GameObject audioPrefab;
    public LayerMask floorMask;
    public LayerMask placeableMask;
    public Transform targetTransform;
    public bool placementMode = false;

    private Vector2 aimPosition;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(placementMode)
            SetAimTargetPosition();
    }

    //Aim Gun Input
    public void PointMouse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            aimPosition = context.ReadValue<Vector2>();
        }
    }

    private void SetAimTargetPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(aimPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask))
        {
            targetTransform.position = hit.point;
            cctvPrefab.transform.position = targetTransform.position;
        }
    }
}
