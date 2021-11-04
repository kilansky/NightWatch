using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCheckManager : SingletonPattern<SkillCheckManager>
{
    public GameObject cameraControlsPanel;

    private bool cameraMoveSkillGate = false;
    private bool cameraMoved = false;
    private bool cameraZoomed = false;

    private void Update()
    {
        if (cameraMoveSkillGate)
        {
            if (!cameraMoved && PlayerInputs.Instance.WASDMovement.x != 0)
                cameraMoved = true;

            if (!cameraZoomed && PlayerInputs.Instance.ScrollingInput != 0)
                cameraZoomed = true;

            if (cameraMoved && cameraZoomed)
            {
                DialogueManager.Instance.StartNextDialogue();
                cameraMoveSkillGate = false;
                cameraControlsPanel.SetActive(false);
            }
        }
    }

    public void StartSkillCheck(SkillCheck skillCheck)
    {
        switch(skillCheck)
        {
            case SkillCheck.AudioPlacement:
                //PlacementSkillGate();
                break;
            case SkillCheck.BuyUpgrade:
                //PlacementSkillGate();
                break;
            case SkillCheck.CameraControls:
                CameraControlsSkillGate();
                break;
            case SkillCheck.CancelPlacement:
                CancelPlacementSkillGate();
                break;
            case SkillCheck.CCTVPlacement:
                CCTVPlacementSkillGate();
                break;
            case SkillCheck.GuardPatrolRoute:
                //PlacementSkillGate();
                break;
            case SkillCheck.GuardPlacement:
                GuardPlacementSkillGate();
                break;
            case SkillCheck.GuardSelection:
                //PlacementSkillGate();
                break;
            case SkillCheck.LaserPlacement:
                //PlacementSkillGate();
                break;
            case SkillCheck.SelectCCTV:
                SelectionSkillGate();
                break;
            case SkillCheck.SellCCTV:
                SellingSkillGate();
                break;
            default:
                Debug.LogError("Skill Check Not Found");
                break;
        }
    }

    public void CameraControlsSkillGate()
    {
        cameraMoveSkillGate = true;
        cameraControlsPanel.SetActive(true);
        //Time.timeScale = 1;
    }

    //Enables CCTV Camera button, and check for player to place a Camera
    public void CCTVPlacementSkillGate()
    {
        HUDController.Instance.SetPlanningUIActive(true, true, false);
        HUDController.Instance.SetButtonsActive(true, false, false, false);
        HUDController.Instance.EnableButtons(true, false, false, false);
        SecurityPlacement.Instance.placementSkillGate = true;
    }

    //Enables Guard button, and check for player to place a Guard
    public void GuardPlacementSkillGate()
    {
        HUDController.Instance.SetPlanningUIActive(true, true, false);
        HUDController.Instance.SetButtonsActive(true, true, false, false);
        HUDController.Instance.EnableButtons(false, true, false, false);
        //SecurityPlacement.Instance.placementSkillGate = true;
    }

    //Enables Laser button, and check for player to place a Laser
    public void LaserPlacementSkillGate()
    {
        HUDController.Instance.SetPlanningUIActive(true, true, false);
        HUDController.Instance.SetButtonsActive(true, true, true, false);
        HUDController.Instance.EnableButtons(false, false, true, false);
        //SecurityPlacement.Instance.placementSkillGate = true;
    }

    //Enables Audio button, and check for player to place a Audio
    public void AudioPlacementSkillGate()
    {
        HUDController.Instance.SetPlanningUIActive(true, true, false);
        HUDController.Instance.SetButtonsActive(true, true, true, true);
        HUDController.Instance.EnableButtons(false, false, false, true);
        //SecurityPlacement.Instance.placementSkillGate = true;
    }

    public void CancelPlacementSkillGate()
    {
        SecurityPlacement.Instance.cancelPlacementSkillGate = true;
    }

    //Check for player to click and select the placed CCTV camera
    public void SelectionSkillGate()
    {
        SecuritySelection.Instance.selectionSkillGate = true;
    }

    //Check for player to sell the selected CCTV camera
    public void SellingSkillGate()
    {
        SelectedObjectButtons.Instance.sellingSkillGate = true;
    }
}
