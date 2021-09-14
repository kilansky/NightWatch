using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : SingletonPattern<HUDController>
{
    private GameObject heldObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Activated when the CCTV Camera Button is pressed to place a camera
    public void CameraButton()
    {
        GameObject objectToPlace = SecurityPlacement.Instance.cctvPrefab;
        TogglePlacementMode(objectToPlace, true);
    }

    //Activated when the Laser Sensor Button is pressed to place a camera
    public void LaserSensorButton()
    {
        GameObject objectToPlace = SecurityPlacement.Instance.laserPrefab;
        TogglePlacementMode(objectToPlace, true);
    }

    //Activated when the Guard Button is pressed to place a camera
    public void GuardButton()
    {
        GameObject objectToPlace = SecurityPlacement.Instance.guardPrefab;
        TogglePlacementMode(objectToPlace, false);
    }

    //Activated when the Audio Sensor Button is pressed to place a camera
    public void AudioSensorButton()
    {
        GameObject objectToPlace = SecurityPlacement.Instance.audioPrefab;
        TogglePlacementMode(objectToPlace, true);
    }

    //Toggles whether an object is being placed, or changes the object being placed
    private void TogglePlacementMode(GameObject objectToPlace, bool placedOnWalls)
    {      
        if (!SecurityPlacement.Instance.placementMode || objectToPlace != SecurityPlacement.Instance.heldObject)
        {
            SecurityPlacement.Instance.placementMode = true;
            SecurityPlacement.Instance.placeOnWalls = placedOnWalls;

            //Remove held object
            if (heldObject != null)
                Destroy(heldObject);

            //Spawn new object to try and place
            heldObject = Instantiate(objectToPlace, Vector3.zero, Quaternion.identity);
            SecurityPlacement.Instance.heldObject = heldObject;
        }
        else
        {
            SecurityPlacement.Instance.placementMode = false;

            //Remove held object
            if (heldObject != null)
                Destroy(heldObject);
        }
    }

    public void ShowPauseScreen()
    {

    }

    public void HidePauseScreen()
    {

    }
}
