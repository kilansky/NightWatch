using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NightHUDController : SingletonPattern<NightHUDController>
{
    [Header("Text Events")]
    public TextMeshProUGUI[] eventText;
    public float eventTextTime = 6;

    [Header("Game Stats")]
    public TextMeshProUGUI itemStolenText;
    public TextMeshProUGUI thievesEscapedText;
    public TextMeshProUGUI thievesApprehendedText;

    [Header("Tutorial Tip")]
    public GameObject guardSelectionTipText;

    [Header("Pause")]
    public GameObject pausePanel;

    [Header("Game End")]
    public GameObject gameEndPanel;
    public TextMeshProUGUI stolenFinalNum;
    public TextMeshProUGUI escapedFinalNum;
    public TextMeshProUGUI apprehendedFinalNum;
    public TextMeshProUGUI startingMoneyText;
    public TextMeshProUGUI penaltyMoneyText;
    public TextMeshProUGUI awardedMoneyText;
    public TextMeshProUGUI endOfNightPaymentText;
    public TextMeshProUGUI finalMoneyText;

    [Header("Fade To Black Overlay")]
    public Image blackOverlay;
    public float fadeToBlackTime;

    [Header("Event Audio")]
    public AudioClip itemStolen;
    public AudioClip thiefEscaped;
    public AudioClip thiefApprehended;

    private int itemStolenNum = 0;
    private int thievesEscapedNum = 0;
    private int thievesApprehendedNum = 0;
    private AudioSource audioSource;

    //Save System
    [HideInInspector] public int difficulty;
    [HideInInspector] public int lastPlayedScene;
    [HideInInspector] public bool LevelOneCompletion;

    private int selectedLevel;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //Disable all event text objects
        for (int i = 0; i < eventText.Length; i++)
        {
            eventText[i].gameObject.SetActive(false);
        }

        //Initialize stats to zero
        itemStolenText.text = itemStolenNum.ToString();
        thievesEscapedText.text = thievesEscapedNum.ToString();
        thievesApprehendedText.text = thievesApprehendedNum.ToString();
    }

    public void StartNight()
    {
        if (GameManager.Instance.currentLevel == 1)
            guardSelectionTipText.SetActive(true);
    }

    //Activates and sets the text of the first inactive text object in the eventText array
    public void ActivateEventText(string textToDisplay)
    {
        GameObject eventTextObject = eventText[0].gameObject;
        //Look through each event text object and activate the first inactive one
        for (int i = 0; i < eventText.Length; i++)
        {
            //Check if this event text object is inactive
            if(!eventText[i].gameObject.activeSelf)
            {
                //Activate text object, set text value, and exit the loop
                eventTextObject = eventText[i].gameObject;
                eventTextObject.SetActive(true);
                eventText[i].text = textToDisplay;
                StartCoroutine(DeactivateEventText(eventTextObject));
                return;
            }
        }
    }

    //Increases the number of items stolen by 1 and displays text for the event
    public void ItemStolenEvent()
    {
        itemStolenNum++;
        itemStolenText.text = itemStolenNum.ToString();
        ActivateEventText("Item Stolen: -$" + Mathf.Abs(MoneyManager.Instance.itemStolenPenalty).ToString());
        audioSource.PlayOneShot(itemStolen);
    }

    //Increases the number of thieves escaped by 1 and displays text for the event
    public void ThiefEscapedEvent()
    {
        thievesEscapedNum++;
        thievesEscapedText.text = thievesEscapedNum.ToString();
        
        ActivateEventText("Theft Prevented: +$0");
        audioSource.PlayOneShot(thiefEscaped);
    }

    //Increases the number of thieves apprehended by 1 and displays text for the event
    public void ThiefApprehendedEvent()
    {
        thievesApprehendedNum++;
        thievesApprehendedText.text = thievesApprehendedNum.ToString();

        ActivateEventText("Thief Caught: +$" + MoneyManager.Instance.thiefApprehendedAward);
        audioSource.PlayOneShot(thiefApprehended);
    }

    //Enables the game end panel and sets the text values for all end game stats
    public void SetGameEndStats()
    {
        PlayerInputs.Instance.canPause = false;
        CameraController.Instance.canPanWithMouse = false;
        SecuritySelection.Instance.DeactivateAllButtons();

        gameEndPanel.SetActive(true);
        stolenFinalNum.text = itemStolenNum.ToString();
        escapedFinalNum.text = thievesEscapedNum.ToString();
        apprehendedFinalNum.text = thievesApprehendedNum.ToString();

        int unspentMoney = MoneyManager.Instance.money;
        int penaltyMoney = MoneyManager.Instance.itemStolenPenalty * itemStolenNum;
        int awardedMoney = MoneyManager.Instance.thiefApprehendedAward * thievesApprehendedNum;
        int endOfNightPayment = MoneyManager.Instance.endOfNightPayment;
        int totalAddedMoney = penaltyMoney + awardedMoney + endOfNightPayment;
        int finalMoney = unspentMoney + totalAddedMoney;

        MoneyManager.Instance.AddMoney(totalAddedMoney);

        startingMoneyText.text = "+$" + unspentMoney.ToString();
        penaltyMoneyText.text = "-$" + Mathf.Abs(penaltyMoney).ToString();
        awardedMoneyText.text = "+$" + awardedMoney.ToString();
        endOfNightPaymentText.text = "+$" + endOfNightPayment.ToString();
        finalMoneyText.text = "$" + finalMoney.ToString();
    }

    public void AddMoneyForCurrentSecurity()
    {
        foreach (GuardController guard in FindObjectsOfType<GuardController>())
            MoneyManager.Instance.AddMoney(500);

        foreach (HackedSecurityScript security in FindObjectsOfType<HackedSecurityScript>())
            MoneyManager.Instance.AddMoney(security.GetComponent<SecurityMeasure>().cost);
    }

    //Waits to disable an activated event text object
    private IEnumerator DeactivateEventText(GameObject eventTextObject)
    {
        yield return new WaitForSecondsRealtime(eventTextTime);
        eventTextObject.SetActive(false);
    }

    public void ShowPauseScreen()
    {
        pausePanel.SetActive(true);
    }

    public void HidePauseScreen()
    {
        pausePanel.SetActive(false);
    }

    public void NextLevel()
    {
        LevelManager.Instance.LoadNextLevel();
    }

    public void QuitLevel()
    {
        LevelManager.Instance.LoadMenu();
    }
}
