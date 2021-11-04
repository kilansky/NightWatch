using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : SingletonPattern<TutorialController>
{
    public GameObject tutorialPanel;
    public GameObject[] tutorialPanels;

    private int activePanelIndex;

    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 0;
        //tutorialPanel.SetActive(true);
        //PlayerInputs.Instance.canPause = false;

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

    //Close the tutorial UI and allow the player to play the game
    public void EndTutorial()
    {
        Time.timeScale = 1;
        tutorialPanel.SetActive(false);
        PlayerInputs.Instance.canPause = true;
    }
}
