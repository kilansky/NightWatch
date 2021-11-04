using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecuritySelection : SingletonPattern<SecuritySelection>
{
    [Header("Selection Objects")]
    public GameObject selectionIcon;
    public GameObject selectionButtons;
    public GameObject cameraUpgradePanel;
    public GameObject laserUpgradePanel;
    public GameObject guardUpgradePanel;
    public GameObject audioUpgradePanel;

    [Header("Selection Modifiers")]
    public Vector3 selectionButtonsOffset = new Vector3(0, 0, -1.6f);
    public Vector3 upgradePanelOffset = new Vector3(0, 0, 1.6f);
    public float selectionScaleMod = 1.25f;
    public LayerMask securityMeasureMask;

    [Header("Buttons")]
    public GameObject sellButton;
    public GameObject moveButton;
    public GameObject rotateButton;
    public GameObject patrolPointsButton;
    public GameObject movePatrolPointButton;
    public GameObject removePatrolPointButton;
    public GameObject guardIdleButton;
    public GameObject guardPatrolButton;
    public GameObject guardPointClickButton;
    public GameObject guardManualButton;

    [HideInInspector] public SecurityMeasure selectedObject;
    [HideInInspector] public Vector3 offScreenPos = new Vector3(0, -10, 0);
    [HideInInspector] public bool canSelect;
    [HideInInspector] public bool selectionSkillGate = false;

    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        selectionIcon.transform.position = offScreenPos;
        selectionButtons.transform.position = offScreenPos;
        HideUpgradePanels();
        canSelect = true;
    }

    // Update is called once per frame
    void Update()
    {
        //If not placing or moving another security measure, allow highlighting and clicking on security measures to select them    
        if (canSelect)
            HoverOverSecurityMeasure();

        //Check if the player right clicks to exit placement mode
        if (selectedObject && PlayerInputs.Instance.RightClickPressed)
            CloseSelection();

        //Check if a guard is selected at night, and update the position of the selection icon and buttons as the guard moves
        if (selectedObject && selectedObject.securityType == SecurityMeasure.SecurityType.guard && GameManager.Instance.nightWatchPhase)
            UpdateSelectionPosition();
    }

    private void HoverOverSecurityMeasure()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;

        //check if mouse is over a security measure
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, securityMeasureMask))
        {
            //Check if either: The game is in the planning phase - select anything; Or, the game is in the night phase - only allow selection of guards
            if (!GameManager.Instance.nightWatchPhase || hit.transform.parent.GetComponent<SecurityMeasure>().securityType == SecurityMeasure.SecurityType.guard)
            {
                if (!selectedObject)
                    selectionIcon.transform.position = hit.transform.position;

                if (PlayerInputs.Instance.LeftClickPressed)
                    SelectSecurityMeasure(hit.transform);
            }
        }
        else if (!selectedObject)
            selectionIcon.transform.position = offScreenPos;
    }

    //Updates the position of the selection icon and buttons while a guard moves
    private void UpdateSelectionPosition()
    {
        selectionIcon.transform.position = selectedObject.transform.position;
        selectionButtons.transform.position = selectionIcon.transform.position + selectionButtonsOffset;
    }

    //Enter the selected state and show a button panel for the selected object
    public void SelectSecurityMeasure(Transform selected)
    {
        if (selectedObject)
            CloseSelection();

        selectionIcon.transform.position = selected.position;
        selectionIcon.transform.localScale *= selectionScaleMod;
        selectionButtons.transform.position = selectionIcon.transform.position + selectionButtonsOffset;
        selectedObject = selected.parent.GetComponent<SecurityMeasure>();
        ActivateButtons();

        //Activate the upgrade panel for the selected object if NOT in the night phase
        if(!GameManager.Instance.nightWatchPhase)
            ActivateUpgradePanel();

        //If a guard was selected during the night watch, activate the HUD selection icon and set the camera to follow the guard loosely
        if (selectedObject.GetComponent<GuardPathfinding>() && GameManager.Instance.nightWatchPhase)
        {
            GuardController.Instance.ActivateHUDSelectionIcon(selectedObject.GetComponent<GuardPathfinding>());
            CameraController.Instance.BeginCameraFollow(selectedObject.transform, true);
        }

        if (selectionSkillGate) //If in the tutorial, selecting an object will move to the next panel and activate the Selling skill gate
        {
            //TutorialController.Instance.NextButton();
            //TutorialController.Instance.SellingSkillGate();
            DialogueManager.Instance.StartNextDialogue();
            selectionSkillGate = false;
        }
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
        HideUpgradePanels();
        DeactivateAllButtons();
        GuardController.Instance.DeactivateHUDSelectionIcon();
    }

    public void ActivateButtons()
    {
        DeactivateAllButtons();

        if (selectedObject.securityType == SecurityMeasure.SecurityType.camera)
        {
            //Activate camera buttons: Sell, Move, Rotate
            sellButton.SetActive(true);
            moveButton.SetActive(true);
            rotateButton.SetActive(true);
        }
        else if (selectedObject.securityType == SecurityMeasure.SecurityType.laser)
        {
            //Activate laser buttons: Sell, Move
            sellButton.SetActive(true);
            moveButton.SetActive(true);
        }
        else if (selectedObject.securityType == SecurityMeasure.SecurityType.guard)
        {
            if(!GameManager.Instance.nightWatchPhase)//Planning Phase Buttons
            {
                //Activate guard buttons: Sell, Move, Rotate, Patrol Points
                sellButton.SetActive(true);
                moveButton.SetActive(true);
                patrolPointsButton.SetActive(true);
            }
            else//Night Phase Buttons
            {
                ActivateGuardButtons();
                guardIdleButton.GetComponent<Button>().interactable = true;
                guardPatrolButton.GetComponent<Button>().interactable = true;
                guardPointClickButton.GetComponent<Button>().interactable = true;
                guardManualButton.GetComponent<Button>().interactable = true;

                //Turn off button for mode the guard is currently in already
                switch (selectedObject.GetComponent<GuardPathfinding>().currControlMode)
                {
                    case GuardPathfinding.ControlMode.Idle:
                        guardIdleButton.GetComponent<Button>().interactable = false;
                        break;
                    case GuardPathfinding.ControlMode.Patrol:
                        guardPatrolButton.GetComponent<Button>().interactable = false;
                        break;
                    case GuardPathfinding.ControlMode.Click:
                        guardPointClickButton.GetComponent<Button>().interactable = false;
                        break;
                    case GuardPathfinding.ControlMode.Manual:
                        guardManualButton.GetComponent<Button>().interactable = false;
                        break;
                    default:
                        break;
                }
            }
        }
        else if (selectedObject.securityType == SecurityMeasure.SecurityType.audio)
        {
            //Activate audio buttons: Sell, Move
            sellButton.SetActive(true);
            moveButton.SetActive(true);
        }
        else if (selectedObject.securityType == SecurityMeasure.SecurityType.patrolMarker)
        {
            //Activate patrolMarker buttons: Move, Remove
            movePatrolPointButton.SetActive(true);
            removePatrolPointButton.SetActive(true);
        }
    }

    private void ActivateUpgradePanel()
    {
        GameObject selectedUpgradePanel;
        switch(selectedObject.securityType)
        {
            case (SecurityMeasure.SecurityType.camera):
                selectedUpgradePanel = cameraUpgradePanel;
                break;
            case (SecurityMeasure.SecurityType.laser):
                selectedUpgradePanel = laserUpgradePanel;
                break;
            case (SecurityMeasure.SecurityType.guard):
                selectedUpgradePanel = guardUpgradePanel;
                break;
            case (SecurityMeasure.SecurityType.audio):
                selectedUpgradePanel = audioUpgradePanel;
                break;
            default:
                selectedUpgradePanel = null;
                break;
        }

        if(selectedUpgradePanel)
        {
            selectedUpgradePanel.transform.position = selectionIcon.transform.position + upgradePanelOffset;
            selectedUpgradePanel.GetComponent<UpgradePanel>().SetActiveButtons();
        }
    }

    //Turn on all buttons to control the guards
    private void ActivateGuardButtons()
    {
        guardIdleButton.SetActive(true);
        guardPatrolButton.SetActive(true);
        guardPointClickButton.SetActive(true);
        guardManualButton.SetActive(true);
    }

    //Turn off all active buttons
    private void DeactivateAllButtons()
    {
        sellButton.SetActive(false);
        moveButton.SetActive(false);
        rotateButton.SetActive(false);
        patrolPointsButton.SetActive(false);
        movePatrolPointButton.SetActive(false);
        removePatrolPointButton.SetActive(false);
        guardIdleButton.SetActive(false);
        guardPatrolButton.SetActive(false);
        guardPointClickButton.SetActive(false);
        guardManualButton.SetActive(false);
    }

    public void HideUpgradePanels()
    {
        cameraUpgradePanel.transform.position = offScreenPos;
        laserUpgradePanel.transform.position = offScreenPos;
        guardUpgradePanel.transform.position = offScreenPos;
        audioUpgradePanel.transform.position = offScreenPos;
    }
}
