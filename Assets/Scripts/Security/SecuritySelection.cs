using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecuritySelection : SingletonPattern<SecuritySelection>
{
    public GameObject selectionIcon;
    public float selectionScaleMod = 1.25f;
    public LayerMask securityMeasureMask;

    [HideInInspector] public bool selectionMode = false;
    [HideInInspector] public GameObject selectedObject;

    private Vector3 offScreenPos = new Vector3(0, -10, 0);
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        selectionIcon.transform.position = offScreenPos;
    }

    // Update is called once per frame
    void Update()
    {
        //If not in placement mode, allow highlighting and clicking on security measures to select them    
        if (!selectionMode && !SecurityPlacement.Instance.placementMode)
            HoverOverSecurityMeasure();

        //Check if the player right clicks to exit placement mode
        if (selectionMode && PlayerInputs.Instance.RightClickPressed)
            CloseSelection();
    }

    private void HoverOverSecurityMeasure()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;

        //check if mouse is over a security measure
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, securityMeasureMask))
        {
            selectionIcon.transform.position = hit.transform.position;

            if (PlayerInputs.Instance.LeftClickPressed)
                SelectSecurityMeasure();
        }
        else
            selectionIcon.transform.position = offScreenPos;
    }

    private void SelectSecurityMeasure()
    {
        selectionIcon.transform.localScale *= selectionScaleMod;
        selectionMode = true;
    }

    //Turn off placement mode and remove the held object
    private void CloseSelection()
    {
        selectionMode = false;
        selectionIcon.transform.localScale /= selectionScaleMod;
        selectionIcon.transform.position = offScreenPos;
    }
}