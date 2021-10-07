using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : SingletonPattern<HUDController>
{
    public TextMeshProUGUI cameraCostText;
    public TextMeshProUGUI laserCostText;
    public TextMeshProUGUI guardCostText;
    public TextMeshProUGUI audioCostText;

    public TextMeshProUGUI moneyText;

    public GameObject pausePanel;

    private GameObject heldObject;
    private SecurityPlacement securityScript;

    private void Start()
    {
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

    public void SetSecurityCosts()
    {
        cameraCostText.text = "$" + securityScript.cctvCamera.GetComponent<SecurityMeasure>().cost.ToString();
        laserCostText.text = "$" + securityScript.laserSensor.GetComponent<SecurityMeasure>().cost.ToString();
        guardCostText.text = "$" + securityScript.guard.GetComponent<SecurityMeasure>().cost.ToString();
        audioCostText.text = "$" + securityScript.audioSensor.GetComponent<SecurityMeasure>().cost.ToString();
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
