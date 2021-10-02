using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedObjectButtons : SingletonPattern<SelectedObjectButtons>
{
    //Sells the security measure that is currently selected
    public void SellButton()
    {
        MoneyManager.Instance.AddMoney(SecuritySelection.Instance.selectedObject.cost);

        //Check if the security measure to sell is a guard, if it is, return its color to the Patrol Colors list
        if(SecuritySelection.Instance.selectedObject.gameObject.GetComponent<GuardPatrolPoints>())
            PatrolColors.Instance.RemoveGuardRouteColor(SecuritySelection.Instance.selectedObject.gameObject.GetComponent<GuardPatrolPoints>().patrolMarkerColor);

        //Destroy the selected security measure and deselect it
        Destroy(SecuritySelection.Instance.selectedObject.gameObject);
        SecuritySelection.Instance.CloseSelection();
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
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode = GuardPathfinding.ControlMode.Idle;
        SecuritySelection.Instance.CloseSelection();
    }

    //Sets guard control mode to patrol state
    public void GuardPatrolButton()
    {
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode = GuardPathfinding.ControlMode.Patrol;
        SecuritySelection.Instance.CloseSelection();
    }

    //Sets guard control mode to click state
    public void GuardClickMoveButton()
    {
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode = GuardPathfinding.ControlMode.Click;
        SecuritySelection.Instance.CloseSelection();
    }

    //Sets guard control mode to manual state
    public void GuardManualButton()
    {
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode = GuardPathfinding.ControlMode.Manual;
        SecuritySelection.Instance.CloseSelection();
    }
}
