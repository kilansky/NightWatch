using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecuritySelection : SingletonPattern<SecuritySelection>
{
    public GameObject selectionIcon;
    public GameObject selectionButtons;
    public GameObject sellButton;
    public GameObject moveButton;
    public GameObject rotateButton;
    public GameObject patrolPointsButton;
    public Vector3 selectionButtonsOffset = new Vector3(0, 0, -1.6f);
    public float selectionScaleMod = 1.25f;
    public LayerMask securityMeasureMask;

    [HideInInspector] public SecurityMeasure selectedObject;
    [HideInInspector] public Vector3 offScreenPos = new Vector3(0, -10, 0);

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        selectionIcon.transform.position = offScreenPos;
        selectionButtons.transform.position = offScreenPos;
    }

    // Update is called once per frame
    void Update()
    {
        //If not in placeing or moving another security measure, allow highlighting and clicking on security measures to select them    
        if (!SecurityPlacement.Instance.placementMode && !SecurityPlacement.Instance.movementMode)
            HoverOverSecurityMeasure();

        //Check if the player right clicks to exit placement mode
        if (selectedObject && PlayerInputs.Instance.RightClickPressed)
            CloseSelection();
    }

    private void HoverOverSecurityMeasure()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;

        //check if mouse is over a security measure
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, securityMeasureMask))
        {
            if(!selectedObject)
                selectionIcon.transform.position = hit.transform.position;

            if (PlayerInputs.Instance.LeftClickPressed)
                SelectSecurityMeasure(hit.transform);
        }
        else if (!selectedObject)
            selectionIcon.transform.position = offScreenPos;
    }

    private void SelectSecurityMeasure(Transform selected)
    {
        if (selectedObject)
            CloseSelection();

        selectionIcon.transform.position = selected.position;
        selectionIcon.transform.localScale *= selectionScaleMod;
        selectionButtons.transform.position = selectionIcon.transform.position + selectionButtonsOffset;
        selectedObject = selected.parent.GetComponent<SecurityMeasure>();
        ActivateButtons();
    }

    //De-select the selected object
    public void CloseSelection()
    {
        if (!selectedObject)
            return;

        selectedObject = null;
        selectionIcon.transform.localScale /= selectionScaleMod;
        selectionIcon.transform.position = offScreenPos;
        selectionButtons.transform.position = offScreenPos;
    }

    private void ActivateButtons()
    {
        if(selectedObject.securityType == SecurityMeasure.SecurityType.camera)
        {
            //Activate camera buttons: Sell, Move, Rotate
            sellButton.SetActive(true);
            moveButton.SetActive(true);
            rotateButton.SetActive(false);
            patrolPointsButton.SetActive(false);
        }
        else if (selectedObject.securityType == SecurityMeasure.SecurityType.laser)
        {
            //Activate camera buttons: Sell, Move
            sellButton.SetActive(true);
            moveButton.SetActive(true);
            rotateButton.SetActive(false);
            patrolPointsButton.SetActive(false);
        }
        else if (selectedObject.securityType == SecurityMeasure.SecurityType.guard)
        {
            //Activate camera buttons: Sell, Move, Rotate, Patrol Points
            sellButton.SetActive(true);
            moveButton.SetActive(true);
            rotateButton.SetActive(false);
            patrolPointsButton.SetActive(true);
        }
        else if (selectedObject.securityType == SecurityMeasure.SecurityType.audio)
        {
            //Activate camera buttons: Sell, Move
            sellButton.SetActive(true);
            moveButton.SetActive(true);
            rotateButton.SetActive(false);
            patrolPointsButton.SetActive(false);
        }
    }
}
