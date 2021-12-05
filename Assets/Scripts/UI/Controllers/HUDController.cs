using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : SingletonPattern<HUDController>
{
    [Header("Money & Costs Text")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI cameraCostText;
    public TextMeshProUGUI laserCostText;
    public TextMeshProUGUI guardCostText;
    public TextMeshProUGUI audioCostText;

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject noPatrolWarningPanel;
    public GameObject securityButtonsPanel;

    [Header("Fade To Black Overlay")]
    public Image blackOverlay;
    public TextMeshProUGUI currNightText;
    public float fadeInTextTime = 2f;
    public float textHoldTime = 1f;
    public float fadeToBlackTime = 2f;

    [Header("Night Watch Button")]
    public Button nightWatchButton;

    [Header("Security Buttons")]
    public GameObject cctvButton;
    public GameObject guardButton;
    public GameObject laserButton;
    public GameObject audioButton;

    private Button cctv;
    private Button guard;
    private Button laser;
    private Button audioSensor;

    private GameObject heldObject;
    private SecurityPlacement securityScript;

    private void Start()
    {
        cctv = cctvButton.GetComponent<Button>();
        guard = guardButton.GetComponent<Button>();
        laser = laserButton.GetComponent<Button>();
        audioSensor = audioButton.GetComponent<Button>();

        nightWatchButton.interactable = false;

        securityScript = SecurityPlacement.Instance;
        SetSecurityCosts();
        SetCurrentNightText();

        PlayerInputs.Instance.canPause = false;
        blackOverlay.gameObject.SetActive(true);
        StartCoroutine(FadeTextToWhite());
    }

    //Activated when the CCTV Camera Button is pressed to place a camera
    public void CameraButton()
    {
        GameObject objectToPlace = securityScript.cctvCamera;
        securityScript.heldObjectCost = objectToPlace.GetComponent<SecurityMeasure>().cost;
        TogglePlacementMode(objectToPlace);
    }

    //Activated when the Laser Sensor Button is pressed to place a camera
    public void LaserSensorButton()
    {
        GameObject objectToPlace = securityScript.laserSensor;
        securityScript.heldObjectCost = objectToPlace.GetComponent<SecurityMeasure>().cost;
        TogglePlacementMode(objectToPlace);
    }

    //Activated when the Guard Button is pressed to place a camera
    public void GuardButton()
    {
        GameObject objectToPlace = securityScript.guard;
        securityScript.heldObjectCost = objectToPlace.GetComponent<SecurityMeasure>().cost;
        TogglePlacementMode(objectToPlace);
    }

    //Activated when the Audio Sensor Button is pressed to place a camera
    public void AudioSensorButton()
    {
        GameObject objectToPlace = securityScript.audioSensor;
        securityScript.heldObjectCost = objectToPlace.GetComponent<SecurityMeasure>().cost;
        TogglePlacementMode(objectToPlace);
    }

    //Toggles whether an object is being placed, or changes the object being placed
    private void TogglePlacementMode(GameObject objectToPlace)
    {
        bool inPlacementMode = securityScript.placementMode;

        //Cancel the placement of any patrol points
        foreach (GuardPatrolPoints guardPatrol in FindObjectsOfType<GuardPatrolPoints>())
            guardPatrol.CancelPlacingPatrolPoint();

        if (!inPlacementMode || (inPlacementMode && objectToPlace != securityScript.heldObject))
        {
            securityScript.placementMode = true; //enter placement mode
            SecuritySelection.Instance.canSelect = false; //disable the ability to select objects
            SecuritySelection.Instance.CloseSelection(); //close any current selections

            //Remove held object
            if (heldObject != null)
                Destroy(heldObject);

            //Spawn new object to try and place
            heldObject = Instantiate(objectToPlace, Vector3.zero, Quaternion.identity);
            securityScript.heldObject = heldObject;
            securityScript.StoreOriginalMaterials();
        }
        else
        {
            securityScript.ExitPlacementMode();
        }
    }

    //Sets the text display for the cost of each security measure
    public void SetSecurityCosts()
    {
        cameraCostText.text = "$" + securityScript.cctvCamera.GetComponent<SecurityMeasure>().cost.ToString();
        laserCostText.text = "$" + securityScript.laserSensor.GetComponent<SecurityMeasure>().cost.ToString();
        guardCostText.text = "$" + securityScript.guard.GetComponent<SecurityMeasure>().cost.ToString();
        audioCostText.text = "$" + securityScript.audioSensor.GetComponent<SecurityMeasure>().cost.ToString();
    }

    //Called when the Begin Night Watch button is pressed - checks to display any warnings
    public void BeginNightButton()
    {
        bool showWarning = false;
        //Check if any guards are missing a patrol route. If any are, show the no patrol route warning
        foreach (GuardPatrolPoints guardPatrolScript in FindObjectsOfType<GuardPatrolPoints>())
        {
            if (!guardPatrolScript.patrolRouteSet)
                showWarning = true;
        }

        if (showWarning)
            ShowNoPatrolWarning();
        else
            GameManager.Instance.BeginNightPhase();
    }

    //Sets the active security measure buttons based on the current level
    public void SetAvailableStartingButtons(int currLevel)
    {
        if (currLevel >= 2)
        {
            cctvButton.SetActive(true);
            guardButton.SetActive(true);
        }

        if (currLevel >= 3)
            laserButton.SetActive(true);

        if (currLevel >= 4)
            audioButton.SetActive(true);
    }

    //Activates or Deactivates security measure buttons
    public void SetButtonsActive(bool cctvEnabled, bool guardEnabled, bool laserEnabled, bool audioEnabled)
    {
        cctvButton.SetActive(cctvEnabled);
        guardButton.SetActive(guardEnabled);
        laserButton.SetActive(laserEnabled);
        audioButton.SetActive(audioEnabled);
    }

    //Enables or Disables interaction with security measure buttons
    public void EnableButtons(bool cctvEnabled, bool guardEnabled, bool laserEnabled, bool audioEnabled)
    {
        cctv.interactable = cctvEnabled;
        guard.interactable = guardEnabled;
        laser.interactable = laserEnabled;
        audioSensor.interactable = audioEnabled;
    }

    //Activates a warning panel when the player presses the Begin Night Watch button w/o a set patrol route
    public void ShowNoPatrolWarning()
    {
        //Debug.Log("Show Warning");
        noPatrolWarningPanel.SetActive(true);
        nightWatchButton.interactable = false;
    }

    //Closes the warning panel, and either begins the night watch or returns to the planning phase
    public void CloseNoPatrolWarning(bool beginNightWatch)
    {
        noPatrolWarningPanel.SetActive(false);
        nightWatchButton.interactable = true;

        if (beginNightWatch)
            GameManager.Instance.BeginNightPhase();
    }

    //Enables or disables the Night Watch Button based on the number of guards
    public void SetNightWatchButtonInteractability()
    {
        //Count the number of guards in the scene
        int numGuards = 0;
        for (int i = 0; i < FindObjectsOfType<GuardPathfinding>().Length; i++)
            numGuards++;

        //Disable the Night Watch button if there are no guards
        if (numGuards == 0)
            nightWatchButton.interactable = false;
        else
            nightWatchButton.interactable = true;
    }

    public void ShowPauseScreen()
    {
        pausePanel.SetActive(true);
    }

    public void HidePauseScreen()
    {
        pausePanel.SetActive(false);
    }

    public void SetCurrentNightText()
    {
        currNightText.text = "Day " + GameManager.Instance.currentLevel;
    }

    //Set the states of the money text, security buttons panel, and night watch button
    public void SetPlanningUIActive(bool moneyActive, bool securityButtonsActive, bool nightWatchButtonActive)
    {
        moneyText.gameObject.SetActive(moneyActive);
        securityButtonsPanel.SetActive(securityButtonsActive);
        nightWatchButton.gameObject.SetActive(nightWatchButtonActive);
    }

    private IEnumerator FadeTextToWhite()
    {
        currNightText.color = new Color(1, 1, 1, 0);
        float timeElaped = 0;

        yield return new WaitForSeconds(0.5f);

        while (timeElaped < fadeInTextTime)
        {
            float alpha = Mathf.Lerp(0, 1, timeElaped / fadeInTextTime);
            currNightText.color = new Color(1, 1, 1, alpha);
            timeElaped += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        currNightText.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(textHoldTime);
        StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeFromBlack()
    {
        blackOverlay.color = new Color(0, 0, 0, 1);
        float timeElaped = 0;

        while (timeElaped < fadeToBlackTime)
        {
            float alpha = Mathf.Lerp(1, 0, timeElaped / fadeToBlackTime);
            blackOverlay.color = new Color(0, 0, 0, alpha);
            currNightText.color = new Color(alpha, alpha, alpha, alpha);
            timeElaped += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        blackOverlay.color = new Color(0, 0, 0, 0);
        currNightText.color = new Color(0, 0, 0, 0);

        yield return new WaitForEndOfFrame();
        blackOverlay.gameObject.SetActive(false);

        CameraController.Instance.canPanWithMouse = true;
        PlayerInputs.Instance.canPause = true;
    }
}
