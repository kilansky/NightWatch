using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class GuardPanel
{
    public GameObject panel;
    public GameObject selectedIcon;
    public Image guardPortrait;
    public TextMeshProUGUI behaviorText;
    public GuardPathfinding guard;
}

public class GuardController : SingletonPattern<GuardController>
{
    public GuardPanel[] guardPanels;

    [HideInInspector] public GuardPathfinding guardInManualMode;

    //Enables a guard panel on the night canvas when a guard is placed into the scene
    public void ActivateGuardPanel(Color guardColor, GuardPathfinding guardScript)
    {
        //Search through each panel and activate the first inactive one
        for (int i = 0; i < guardPanels.Length; i++)
        {
            //If the panel is inactive, activate it
            if (!guardPanels[i].panel.activeSelf)
            {
                guardPanels[i].guard = guardScript;
                guardPanels[i].guardPortrait.color = guardColor;
                guardPanels[i].selectedIcon.SetActive(false);
                guardPanels[i].behaviorText.text = "Patrolling";
                guardPanels[i].panel.SetActive(true);
                return;
            }
        }
    }

    //Removes a guard panel from the night canvas when a guard is sold
    public void DeactivateGuardPanel(GuardPathfinding guardScript)
    {
        for (int i = 0; i < guardPanels.Length; i++)
        {
            if(guardPanels[i].guard == guardScript)
            {
                guardPanels[i].panel.SetActive(false);
                return;
            }
        }
    }

    private TextMeshProUGUI GetSelectedGuardText()
    {
        for (int i = 0; i < guardPanels.Length; i++)
        {
            if (guardPanels[i].guard == SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>())
            {
                return guardPanels[i].behaviorText;
            }
        }
        Debug.LogError("Guard Not Found");

        return null;
    }

    //Sets guard control mode to idle state
    public void IdleButton()
    {
        ResetManualGuard();
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode = GuardPathfinding.ControlMode.Idle;
        GetSelectedGuardText().text = "Idling";
        //SecuritySelection.Instance.CloseSelection();
    }

    //Sets guard control mode to patrol state
    public void PatrolButton()
    {
        ResetManualGuard();
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode = GuardPathfinding.ControlMode.Patrol;
        GetSelectedGuardText().text = "Patrolling";
        //SecuritySelection.Instance.CloseSelection();
    }

    //Sets guard control mode to click state
    public void ClickMoveButton()
    {
        ResetManualGuard();
        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode = GuardPathfinding.ControlMode.Click;
        GetSelectedGuardText().text = "Moving to Point";
        //SecuritySelection.Instance.CloseSelection();
    }

    //Sets guard control mode to manual state
    public void ManualButton()
    {
        if (guardInManualMode)
            guardInManualMode.currControlMode = guardInManualMode.lastControlMode;

        SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode = GuardPathfinding.ControlMode.Manual;
        GetSelectedGuardText().text = "Controlling";
        //SecuritySelection.Instance.CloseSelection();
        guardInManualMode = SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>();
    }

    private void ResetManualGuard()
    {
        //If this guard was in manual mode, disable it
        if (guardInManualMode && SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode == GuardPathfinding.ControlMode.Manual)
        {
            CameraController.Instance.followGuard = false;
            CameraController.Instance.CameraFollow(null);
            guardInManualMode = null;
        }
    }
}
