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

    private void Update()
    {
        //If a guard is currently selected, check for hotkey inputs to activate buttons
        if (SecuritySelection.Instance.selectedObject && SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>())
        {
            if (PlayerInputs.Instance.Hotkey1)
            {
                IdleButton();
                SecuritySelection.Instance.ActivateButtons();
            }
            else if (PlayerInputs.Instance.Hotkey2)
            {
                PatrolButton();
                SecuritySelection.Instance.ActivateButtons();
            }
            else if (PlayerInputs.Instance.Hotkey3)
            {
                ClickMoveButton();
                SecuritySelection.Instance.ActivateButtons();
            }
            else if (PlayerInputs.Instance.Hotkey4)
            {
                ManualButton();
                SecuritySelection.Instance.ActivateButtons();
            }
        }
    }

    //Activate the selection icon of the currently selected guard
    public void ActivateHUDSelectionIcon(GuardPathfinding guard)
    {
        for (int i = 0; i < guardPanels.Length; i++)
        {
            if (guardPanels[i].guard == guard)
            {
                guardPanels[i].selectedIcon.SetActive(true);
            }
            else
            {
                guardPanels[i].selectedIcon.SetActive(false);
            }
        }
    }

    //Disable all guard selection icons
    public void DeactivateHUDSelectionIcon()
    {
        for (int i = 0; i < guardPanels.Length; i++)
        {
            guardPanels[i].selectedIcon.SetActive(false);
        }
    }

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

    private TextMeshProUGUI GetGuardText(GuardPathfinding guard)
    {
        for (int i = 0; i < guardPanels.Length; i++)
        {
            if (guardPanels[i].guard == guard)
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
        GuardPathfinding selectedGuard = SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>();
        selectedGuard.currControlMode = GuardPathfinding.ControlMode.Idle;
        selectedGuard.lastControlMode = GuardPathfinding.ControlMode.Idle;

        GuardBehaviorChanged(selectedGuard);
        CheckToChaseThief(selectedGuard);
    }

    //Sets guard control mode to patrol state
    public void PatrolButton()
    {
        ResetManualGuard();
        GuardPathfinding selectedGuard = SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>();
        selectedGuard.currControlMode = GuardPathfinding.ControlMode.Patrol;
        selectedGuard.lastControlMode = GuardPathfinding.ControlMode.Patrol;

        GuardBehaviorChanged(selectedGuard);
        CheckToChaseThief(selectedGuard);
    }

    //Sets guard control mode to click state
    public void ClickMoveButton()
    {
        ResetManualGuard();
        GuardPathfinding selectedGuard = SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>();
        selectedGuard.currControlMode = GuardPathfinding.ControlMode.Click;
        selectedGuard.lastControlMode = GuardPathfinding.ControlMode.Click;

        GuardBehaviorChanged(selectedGuard);
        CheckToChaseThief(selectedGuard);
    }

    //Sets guard control mode to manual state
    public void ManualButton()
    {
        if (guardInManualMode)
            guardInManualMode.currControlMode = guardInManualMode.lastControlMode;

        GuardPathfinding selectedGuard = SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>();
        selectedGuard.currControlMode = GuardPathfinding.ControlMode.Manual;
        guardInManualMode = selectedGuard;
        GuardBehaviorChanged(selectedGuard);
    }

    //Switches guard to chase mode if there is a current thief to chase
    public void CheckToChaseThief(GuardPathfinding selectedGuard)
    {
        if(selectedGuard.thiefToChase)
        {
            selectedGuard.currControlMode = GuardPathfinding.ControlMode.Chase;
            SetGuardBehaviorText(selectedGuard, selectedGuard.currControlMode); //Sets the behavior text of the selected guard
        }
    }

    //Do these things when ANY guard behavior button is pressed
    public void GuardBehaviorChanged(GuardPathfinding selectedGuard)
    {
        selectedGuard.ResetClickMoveUI(); //Sets the location of UI for click movement
        SecuritySelection.Instance.ActivateButtons(); //Changes the active guard behavior buttons
        SetGuardBehaviorText(selectedGuard, selectedGuard.currControlMode); //Sets the behavior text of the selected guard
    }

    //Set the behavior text of the guard
    public void SetGuardBehaviorText(GuardPathfinding guard, GuardPathfinding.ControlMode currBehavior)
    {      
        switch (currBehavior)
        {
            case GuardPathfinding.ControlMode.Idle:
                GetGuardText(guard).text = "Idling";
                break;
            case GuardPathfinding.ControlMode.Patrol:
                GetGuardText(guard).text = "Patrolling";
                break;
            case GuardPathfinding.ControlMode.Click:
                GetGuardText(guard).text = "Moving to Point";
                break;
            case GuardPathfinding.ControlMode.Manual:
                GetGuardText(guard).text = "Manual Control";
                break;
            case GuardPathfinding.ControlMode.Chase:
                GetGuardText(guard).text = "Chasing Thief";
                break;
            default:
                break;
        }
    }

    private void ResetManualGuard()
    {
        //If this guard was in manual mode, disable it
        if (guardInManualMode && SecuritySelection.Instance.selectedObject.GetComponent<GuardPathfinding>().currControlMode == GuardPathfinding.ControlMode.Manual)
        {
            CameraController.Instance.EndCameraFollow();
            guardInManualMode = null;
        }
    }
}
