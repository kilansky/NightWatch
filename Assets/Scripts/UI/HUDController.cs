using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : SingletonPattern<HUDController>
{
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
        GameObject objectToPlace = securityScript.cctvCamera.prefab;
        securityScript.heldObjectCost = securityScript.cctvCamera.cost;
        TogglePlacementMode(objectToPlace, true);
    }

    //Activated when the Laser Sensor Button is pressed to place a camera
    public void LaserSensorButton()
    {
        GameObject objectToPlace = securityScript.laserSensor.prefab;
        securityScript.heldObjectCost = securityScript.laserSensor.cost;
        TogglePlacementMode(objectToPlace, true);
    }

    //Activated when the Guard Button is pressed to place a camera
    public void GuardButton()
    {
        GameObject objectToPlace = securityScript.guard.prefab;
        securityScript.heldObjectCost = securityScript.guard.cost;
        TogglePlacementMode(objectToPlace, false);
    }

    //Activated when the Audio Sensor Button is pressed to place a camera
    public void AudioSensorButton()
    {
        GameObject objectToPlace = securityScript.audioSensor.prefab;
        securityScript.heldObjectCost = securityScript.audioSensor.cost;
        TogglePlacementMode(objectToPlace, true);
    }

    //Toggles whether an object is being placed, or changes the object being placed
    private void TogglePlacementMode(GameObject objectToPlace, bool placedOnWalls)
    {
        bool inPlacementMode = securityScript.placementMode;

        if (!inPlacementMode || (inPlacementMode && objectToPlace != securityScript.heldObject))
        {
            securityScript.placementMode = true;
            securityScript.placeOnWalls = placedOnWalls;

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
        securityScript.cctvCamera.costText.text = "$" + securityScript.cctvCamera.cost.ToString();
        securityScript.laserSensor.costText.text = "$" + securityScript.laserSensor.cost.ToString();
        securityScript.guard.costText.text = "$" + securityScript.guard.cost.ToString();
        securityScript.audioSensor.costText.text = "$" + securityScript.audioSensor.cost.ToString();
    }

    public void ShowPauseScreen()
    {

    }

    public void HidePauseScreen()
    {

    }
}
