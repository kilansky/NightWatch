using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedObjectButtons : SingletonPattern<SelectedObjectButtons>
{
    [HideInInspector] public bool guardIsInManualMode = false;
    [HideInInspector] public bool tutorialMode = false;

    //Sells the security measure that is currently selected
    public void SellButton()
    {
        MoneyManager.Instance.AddMoney(SecuritySelection.Instance.selectedObject.cost);

        bool destroyedGuard = false;
        //Check if the security measure to sell is a guard, and do some cleanup
        if(SecuritySelection.Instance.selectedObject.gameObject.GetComponent<GuardPatrolPoints>())
        {
            destroyedGuard = true; //Used to check to disable the Night Watch Button

            //Return patrol color to the Patrol Colors list
            PatrolColors.Instance.RemoveGuardRouteColor(SecuritySelection.Instance.selectedObject.gameObject.GetComponent<GuardPatrolPoints>().patrolMarkerColor);

            //Destroy the patrol routes attached to this guard
            GameObject[] patrolPoints = SecuritySelection.Instance.selectedObject.gameObject.GetComponent<GuardPatrolPoints>().PatrolPoints.ToArray();
            for (int i = 0; i < patrolPoints.Length; i++)
                Destroy(patrolPoints[i]);
        }

        //Destroy the selected security measure and deselect it
        Destroy(SecuritySelection.Instance.selectedObject.gameObject);
        SecuritySelection.Instance.CloseSelection();

        //Check to disable the Night Watch Button if there are now 0 guards
        if (destroyedGuard)
            HUDController.Instance.SetNightWatchButtonInteractability();

        if (tutorialMode) //If in the tutorial, selling an object will move to the next panel and freeze camera movement
        {
            TutorialController.Instance.NextButton();
            TutorialController.Instance.cctvButton.interactable = false;
        }
    }

    //Moves the security measure that is currently selected
    public void MoveButton()
    {
        SecurityPlacement.Instance.MovePlacedObject();
    }

    //Rotates the security measure that is currently selected
    public void RotateButton()
    {
        Debug.Log("Rotate Button Pressed");
    }

    //Allows setting up patrol points for the selected guard
    public void PatrolPointsButton()
    {
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPatrolPoints>().patrolPlacementMode = true;
        SecuritySelection.Instance.selectionButtons.transform.position = SecuritySelection.Instance.offScreenPos;
        SecuritySelection.Instance.canSelect = false;
    }

    //Moves a patrol point from the GuardPatrolPoints script
    public void MovePatrolPointButton()
    {
        GameObject patrolPoint = SecuritySelection.Instance.selectedObject.gameObject;
        GuardPatrolPoints guardPatrol = patrolPoint.GetComponent<PatrolMarker>().connectedGuard;
        guardPatrol.BeginMovingPatrolPoint(patrolPoint);
    }


    //Removes a patrol point from the GuardPatrolPoints script
    public void RemovePatrolPointButton()
    {
        GameObject patrolPoint = SecuritySelection.Instance.selectedObject.gameObject;
        GuardPatrolPoints guardPatrol = patrolPoint.GetComponent<PatrolMarker>().connectedGuard;
        guardPatrol.RemovePatrolPoint(patrolPoint);
        SecuritySelection.Instance.CloseSelection();
    }

    //Sets guard control mode to idle state
    public void GuardIdleButton()
    {
        ResetManualGuard();
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode = GuardPathfinding.ControlMode.Idle;
        SecuritySelection.Instance.CloseSelection();
    }

    //Sets guard control mode to patrol state
    public void GuardPatrolButton()
    {
        ResetManualGuard();
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode = GuardPathfinding.ControlMode.Patrol;
        SecuritySelection.Instance.CloseSelection();
    }

    //Sets guard control mode to click state
    public void GuardClickMoveButton()
    {
        ResetManualGuard();
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode = GuardPathfinding.ControlMode.Click;
        SecuritySelection.Instance.CloseSelection();
    }

    //Sets guard control mode to manual state
    public void GuardManualButton()
    {
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode = GuardPathfinding.ControlMode.Manual;
        SecuritySelection.Instance.CloseSelection();
        guardIsInManualMode = true;
    }

    private void ResetManualGuard()
    {
        //If this guard was in manual mode, disable it
        if (guardIsInManualMode && SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode == GuardPathfinding.ControlMode.Manual)
        {
            CameraController.Instance.followGuard = false;
            CameraController.Instance.CameraFollow(null);
            guardIsInManualMode = false;
        }
    }
}
