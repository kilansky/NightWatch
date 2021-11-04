using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SkillCheck
{
    None, CCTVPlacement, CancelPlacement, SelectCCTV, SellCCTV, CameraControls, UpgradePurchased,
    GuardPlacement, GuardPatrolRoute, LaserPlacement, AudioPlacement, PatrolPoints
}

public class SkillCheckManager : SingletonPattern<SkillCheckManager>
{
    public GameObject cameraControlsPanel;
    public GameObject cctvPlacementArrow;

    private bool cameraMoveSkillGate = false;
    private bool cameraMoved = false;
    private bool cameraZoomed = false;

    private bool patrolPointsSkillGate = false;
    private bool upgradePurchasedSkillGate = false;

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
                AudioPlacementSkillGate();
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
                PatrolRouteSkillGate();
                break;
            case SkillCheck.PatrolPoints:
                PatrolPointsSkillGate();
                break;
            case SkillCheck.GuardPlacement:
                GuardPlacementSkillGate();
                break;
            case SkillCheck.LaserPlacement:
                LaserPlacementSkillGate();
                break;
            case SkillCheck.SelectCCTV:
                SelectionSkillGate();
                break;
            case SkillCheck.SellCCTV:
                SellingSkillGate();
                break;
            case SkillCheck.UpgradePurchased:
                UpgradePurchasedSkillGate();
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
        cctvPlacementArrow.SetActive(true);
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
        SecurityPlacement.Instance.placementSkillGate = true;
    }

    //Enables Laser button, and check for player to place a Laser
    public void LaserPlacementSkillGate()
    {
        HUDController.Instance.SetPlanningUIActive(true, true, false);
        HUDController.Instance.SetButtonsActive(true, true, true, false);
        HUDController.Instance.EnableButtons(false, false, true, false);
        SecurityPlacement.Instance.placementSkillGate = true;
    }

    //Enables Audio button, and check for player to place a Audio
    public void AudioPlacementSkillGate()
    {
        HUDController.Instance.SetPlanningUIActive(true, true, false);
        HUDController.Instance.SetButtonsActive(true, true, true, true);
        HUDController.Instance.EnableButtons(false, false, false, true);
        SecurityPlacement.Instance.placementSkillGate = true;
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

    //Check for player to sell the selected CCTV camera
    public void PatrolRouteSkillGate()
    {
        SelectedObjectButtons.Instance.patrolRouteSkillGate = true;
    }

    public void PatrolPointsSkillGate()
    {
        patrolPointsSkillGate = true;
        StartCoroutine(CheckForPatrolPoints());
    }

    private IEnumerator CheckForPatrolPoints()
    {
        while(patrolPointsSkillGate)
        {
            if (FindObjectsOfType<PatrolMarker>().Length >= 4)
            {
                DialogueManager.Instance.StartNextDialogue();
                patrolPointsSkillGate = false;
            }
            yield return new WaitForSeconds(0.25f);
        }

        HUDController.Instance.SetPlanningUIActive(true, true, true);
        HUDController.Instance.nightWatchButton.interactable = false;
    }

    private void UpgradePurchasedSkillGate()
    {
        upgradePurchasedSkillGate = true;
    }

    public void UpgradePurchased()
    {
        if(upgradePurchasedSkillGate)
        {
            DialogueManager.Instance.StartNextDialogue();
            upgradePurchasedSkillGate = false;
        }
    }
}
