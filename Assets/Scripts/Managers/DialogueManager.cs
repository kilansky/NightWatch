using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum SkillCheck { None, CCTVPlacement, CancelPlacement, SelectCCTV, SellCCTV, CameraControls,
    GuardPlacement, GuardSelection, GuardPatrolRoute, LaserPlacement, BuyUpgrade, AudioPlacement}

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
    }

    private void Update()
    {
        if(inConversation && !skillCheckStarted && PlayerInputs.Instance.LeftClickPressed)
        {
            DisplayNextSentence();
        }
    }

    //Activates a specified dialogue, sets up a queue of sentences, and displays them 
    public void StartDialogue(Dialogue dialogue)
    {
        HUDController.Instance.SetPlanningUIActive(false, false, false);
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
        Debug.Log("currDialogueIndex: " + currDialogueIndex);
        Debug.Log("dialogues length: " + dialogues.Length);
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

        string sentence = sentencesQueue.Dequeue();
        sentenceText.text = sentence;
    }


    //Checks to close the dialogue box and end the conversation, or start a skill check or new dialogue
    private void EndDialogue()
    {
        if(currDialogueIndex == dialogues.Length - 1)
            HUDController.Instance.SetPlanningUIActive(true, true, true);

        //Begin the next dialogue automatically if there is no skill check and the last dialogue has not been seen
        if (currDialogue.startSkillCheck == SkillCheck.None && currDialogueIndex < dialogues.Length - 1)
        {
            StartNextDialogue();
            return;
        }
        //Begin a skill check if one is set up on this dialogue
        else if (currDialogue.startSkillCheck != SkillCheck.None)
        {
            SkillCheckManager.Instance.StartSkillCheck(currDialogue.startSkillCheck);
            skillCheckStarted = true;
        }

        dialogueBox.SetActive(false);
        inConversation = false;
    }
}
