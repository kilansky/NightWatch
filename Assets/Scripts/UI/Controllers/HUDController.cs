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

    [Header("Night Watch Button")]
    public Button nightWatchButton;

    private GameObject heldObject;
    private SecurityPlacement securityScript;

    private void Start()
    {
        nightWatchButton.interactable = false;

        securityScript = SecurityPlacement.Instance;
        SetSecurityCosts();
    }

    //Activated when the CCTV Camera Button is pressed to place a camera
    public void CameraButton()
    {
        GameObject objectToPlace = securityScript.cctvCamera;
        securityScript.heldObjectCost = securityScript.cctvCamera.GetComponent<SecurityMeasure>().cost;
        TogglePlacementMode(objectToPlace);
    }

    //Activated when the Laser Sensor Button is pressed to place a camera
    public void LaserSensorButton()
    {
        GameObject objectToPlace = securityScript.laserSensor;
        securityScript.heldObjectCost = securityScript.laserSensor.GetComponent<SecurityMeasure>().cost;
        TogglePlacementMode(objectToPlace);
    }

    //Activated when the Guard Button is pressed to place a camera
    public void GuardButton()
    {
        GameObject objectToPlace = securityScript.guard;
        securityScript.heldObjectCost = securityScript.guard.GetComponent<SecurityMeasure>().cost;
        TogglePlacementMode(objectToPlace);
    }

    //Activated when the Audio Sensor Button is pressed to place a camera
    public void AudioSensorButton()
    {
        GameObject objectToPlace = securityScript.audioSensor;
        securityScript.heldObjectCost = securityScript.audioSensor.GetComponent<SecurityMeasure>().cost;
        TogglePlacementMode(objectToPlace);
    }

    //Toggles whether an object is being placed, or changes the object being placed
    private void TogglePlacementMode(GameObject objectToPlace)
    {
        bool inPlacementMode = securityScript.placementMode;

        if (!inPlacementMode || (inPlacementMode && objectToPlace != securityScript.heldObject))
        {
            securityScript.placementMode = true;
            SecuritySelection.Instance.canSelect = false;

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

    //Activates a warning panel when the player presses the Begin Night Watch button w/o a set patrol route
    public void ShowNoPatrolWarning()
    {
        Debug.Log("Show Warning");
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
        for (int i = 0; i < FindObjectsOfType<GuardPathfinding>().Length - 1; i++)
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
}
