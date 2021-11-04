using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedObjectButtons : SingletonPattern<SelectedObjectButtons>
{
    [HideInInspector] public bool sellingSkillGate = false;
    [HideInInspector] public bool patrolRouteSkillGate = false;

    private SecuritySelection selectionScript;

    private void Start()
    {
        selectionScript = SecuritySelection.Instance;
    }

    //Sells the security measure that is currently selected
    public void SellButton()
    {
        MoneyManager.Instance.AddMoney(selectionScript.selectedObject.cost);

        bool destroyedGuard = false;
        //Check if the security measure to sell is a guard, and do some cleanup
        if(selectionScript.selectedObject.gameObject.GetComponent<GuardPatrolPoints>())
        {
            destroyedGuard = true; //Used to check to disable the Night Watch Button
            GameObject selectedGuard = selectionScript.selectedObject.gameObject;

            //Return patrol color to the Patrol Colors list
            PatrolColors.Instance.RemoveGuardRouteColor(selectedGuard.GetComponent<GuardPatrolPoints>().patrolMarkerColor);

            //Destroy the patrol routes attached to this guard
            GameObject[] patrolPoints = selectedGuard.GetComponent<GuardPatrolPoints>().PatrolPoints.ToArray();
            for (int i = 0; i < patrolPoints.Length; i++)
                Destroy(patrolPoints[i]);

            //Remove guard panel UI from the night canvas
            GuardController.Instance.DeactivateGuardPanel(selectedGuard.GetComponent<GuardPathfinding>());
        }

        //Destroy the selected security measure and deselect it
        Destroy(selectionScript.selectedObject.gameObject);
        selectionScript.CloseSelection();

        //Check to disable the Night Watch Button if there are now 0 guards
        if (destroyedGuard)
            StartCoroutine(KillGuard());

        if (sellingSkillGate) //If in the tutorial, selling an object will move to the next panel and freeze camera movement
        {
            //TutorialController.Instance.NextButton();
            //TutorialController.Instance.cctvButton.interactable = false;

            sellingSkillGate = false;
            DialogueManager.Instance.StartNextDialogue();
        }
    }

    private IEnumerator KillGuard()
    {
        yield return new WaitForSeconds(.05f);
        HUDController.Instance.SetNightWatchButtonInteractability();
    }

    //Moves the security measure that is currently selected
    public void MoveButton()
    {
        SecurityPlacement.Instance.MovePlacedObject();
    }

    //Rotates the security measure that is currently selected
    public void RotateButton()
    {
        selectionScript.selectedObject.GetComponent<CameraRotation>().rotationUI.SetActive(true);
        selectionScript.CloseSelection();
    }

    //Allows setting up patrol points for the selected guard
    public void PatrolPointsButton()
    {
        selectionScript.selectedObject.GetComponent<GuardPatrolPoints>().patrolPlacementMode = true;
        selectionScript.selectionButtons.transform.position = selectionScript.offScreenPos;
        selectionScript.HideUpgradePanels();
        selectionScript.canSelect = false;

        if (patrolRouteSkillGate)
        {
            patrolRouteSkillGate = false;
            DialogueManager.Instance.StartNextDialogue();
        }
    }

    //Moves a patrol point from the GuardPatrolPoints script
    public void MovePatrolPointButton()
    {
        GameObject patrolPoint = selectionScript.selectedObject.gameObject;
        GuardPatrolPoints guardPatrol = patrolPoint.GetComponent<PatrolMarker>().connectedGuard;
        guardPatrol.BeginMovingPatrolPoint(patrolPoint);
    }


    //Removes a patrol point from the GuardPatrolPoints script
    public void RemovePatrolPointButton()
    {
        GameObject patrolPoint = selectionScript.selectedObject.gameObject;
        GuardPatrolPoints guardPatrol = patrolPoint.GetComponent<PatrolMarker>().connectedGuard;
        guardPatrol.RemovePatrolPoint(patrolPoint);
        selectionScript.CloseSelection();
    }
}
