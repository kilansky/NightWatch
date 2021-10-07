using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : SingletonPattern<TutorialController>
{
    public GameObject tutorialPanel;
    public GameObject[] tutorialPanels;

    public Button cctvButton;
    public Button laserButton;
    public Button guardButton;
    public Button beginNightWatchButton;

    private int activePanelIndex;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        tutorialPanel.SetActive(true);
        cctvButton.interactable = false;
        laserButton.interactable = false;
        guardButton.interactable = false;
        beginNightWatchButton.interactable = false;

        foreach (GameObject panel in tutorialPanels)
            panel.SetActive(false);

        activePanelIndex = 0;
        tutorialPanels[activePanelIndex].SetActive(true);
    }

    //Goes to the next tutorial panel
    public void NextButton()
    {
        tutorialPanels[activePanelIndex].SetActive(false); //disable current panel

        //Go to next panel or end tutorial
        if (activePanelIndex < tutorialPanels.Length - 1)
        {
            //Activate the next panel and enable the prev button
            activePanelIndex++;
            tutorialPanels[activePanelIndex].SetActive(true);
        }
        else
            EndTutorial();
    }

    //Goes to the previous tutorial panel
    public void PrevButton()
    {
        //Disable current panel and enable the previous one
        tutorialPanels[activePanelIndex].SetActive(false);
        activePanelIndex--;
        tutorialPanels[activePanelIndex].SetActive(true);
    }

    //Enables camera controls and CCTV Camera button, and check for player to place a camera
    public void PlacementSkillGate()
    {
        Time.timeScale = 1;
        cctvButton.interactable = true;
        SecurityPlacement.Instance.placementSkillGate = true;
    }

    //Check for player to click and select the placed CCTV camera
    public void SelectionSkillGate()
    {
        SecuritySelection.Instance.tutorialMode = true;
    }

    //Check for player to sell the selected CCTV camera
    public void SellingSkillGate()
    {
        SelectedObjectButtons.Instance.tutorialMode = true;
    }

    //Close the tutorial UI and allow the player to play the game
    public void EndTutorial()
    {
        Time.timeScale = 1;
        beginNightWatchButton.interactable = true;
        tutorialPanel.SetActive(false);
    }
}
