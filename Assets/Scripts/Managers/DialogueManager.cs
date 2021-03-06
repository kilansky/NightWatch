using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Dialogue
{
    public string name;

    [TextArea(3, 10)]
    public string[] sentences;
    public SkillCheck startSkillCheck;
}

public class DialogueManager : SingletonPattern<DialogueManager>
{
    [Header("Object References")]
    public GameObject dialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI sentenceText;
    public GameObject clickToContinueText;
    public GameObject SkipButton;

    [Header("Dialogue")]
    public Dialogue[] dialogues;

    private Queue<string> sentencesQueue;
    private Dialogue currDialogue;
    private int currDialogueIndex = 0;
    private bool inConversation;
    private bool skillCheckStarted;

    private void Start()
    {
        sentencesQueue = new Queue<string>();
        StartDialogue(dialogues[0]);
        HUDController.Instance.SetPlanningUIActive(false, false, false);
    }

    private void Update()
    {
        if(inConversation && !skillCheckStarted && PlayerInputs.Instance.LeftClickPressed && !PlayerInputs.Instance.IsPaused)
        {
            DisplayNextSentence();
        }
    }

    //Activates a specified dialogue, sets up a queue of sentences, and displays them 
    public void StartDialogue(Dialogue dialogue)
    {
        dialogueBox.SetActive(true);
        inConversation = true;

        sentencesQueue.Clear();
        foreach (string sentence in dialogue.sentences)
            sentencesQueue.Enqueue(sentence);

        nameText.text = dialogue.name;
        currDialogue = dialogue;
        DisplayNextSentence();
    }

    //Activates the next dialogue box in index order
    public void StartNextDialogue()
    {
        currDialogueIndex++;
        skillCheckStarted = false;
        StartDialogue(dialogues[currDialogueIndex]);
    }

    //Displays the next sentence in the sentences queue
    public void DisplayNextSentence()
    {
        if(sentencesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }
        //Check to begin the camera controls skill check
        else if (sentencesQueue.Count == 1 && currDialogue.startSkillCheck != SkillCheck.None)
        {
            SkillCheckManager.Instance.StartSkillCheck(currDialogue.startSkillCheck);
            skillCheckStarted = true;
            clickToContinueText.SetActive(false);
        }
        else
        {
            clickToContinueText.SetActive(true);
        }

        string sentence = sentencesQueue.Dequeue();
        sentenceText.text = sentence;
    }

    //Checks to close the dialogue box and end the conversation, or start a skill check or new dialogue
    private void EndDialogue()
    {
        if(currDialogueIndex == dialogues.Length - 1)
        {
            HUDController.Instance.SetPlanningUIActive(true, true, true);
            HUDController.Instance.SetButtonsActive(true, true, false, false);
        }

        //Begin the next dialogue automatically if there is no skill check and the last dialogue has not been seen
        if (currDialogue.startSkillCheck == SkillCheck.None && currDialogueIndex < dialogues.Length - 1)
        {
            StartNextDialogue();
            return;
        }

        SkipAllDialogue();
    }

    public void SkipAllDialogue()
    {
        dialogueBox.SetActive(false);
        inConversation = false;

        SkillCheckManager.Instance.cameraControlsPanel.SetActive(false);
        SkillCheckManager.Instance.cctvPlacementArrow.SetActive(false);
        SkillCheckManager.Instance.CancelAllSkillGates();
        SkipButton.SetActive(false);

        HUDController.Instance.SetNightWatchButtonInteractability();
        HUDController.Instance.SetPlanningUIActive(true, true, true);
        HUDController.Instance.EnableButtons(true, true, true, true);

        if (GameManager.Instance.currentLevel == 1)
            HUDController.Instance.SetButtonsActive(true, true, false, false);

        if (GameManager.Instance.currentLevel == 2)
            HUDController.Instance.SetButtonsActive(true, true, true, false);

        if (GameManager.Instance.currentLevel == 3)
            HUDController.Instance.SetButtonsActive(true, true, true, true);
    }
}
