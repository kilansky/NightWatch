using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Alert))]
public class AudioSensor : MonoBehaviour
{
    public float detectionRange = 5f;
    public GameObject directionalSignal;

    [HideInInspector] public bool directionalSignalUpgrade = false;

    private TestDijkstraPath wayPointManager;
    private bool waypointsChecked;
    private Camera mainCamera;
    private List<ThiefPathfinding> nearbyThieves = new List<ThiefPathfinding>();
    private Alert alert;
   
    void Start()
    {
        //Set scale of object to size of the detection range
        SetSensorRange(detectionRange);
        wayPointManager = FindObjectOfType<TestDijkstraPath>();
        waypointsChecked = false;
        alert = GetComponent<Alert>();
    }

    private void Update()
    {
        if(GameManager.Instance.nightWatchPhase && !waypointsChecked)
        {
            //print("Audio sensor on");
            for (int w = 0; w < wayPointManager.waypoints.Length; w++)
            {
                if (Vector3.Distance(transform.position, wayPointManager.waypoints[w].position) < ((detectionRange/2) + 1))
                {
                    //print(Vector3.Distance(transform.position, wayPointManager.waypoints[w].position) + " < " + detectionRange);
                    wayPointManager.waypoints[w].GetComponent<Waypoints>().security.Add(transform.parent.gameObject);
                }
            }
            waypointsChecked = true;
        }

        //Check if the directional signal upgrade has been purchased and there are nearby thieves
        if (directionalSignalUpgrade && nearbyThieves.Count > 0)
        {
            directionalSignal.SetActive(true);
            SetRotationDirection();
        }
        //Check if the directional signal upgrade has been purchased, there are 0 thieves, and the directional signal is active
        else if (directionalSignalUpgrade && directionalSignal.activeSelf)
            directionalSignal.SetActive(false);

        //If one of the nearby thieves no longer exists, remove it from the list
        if (nearbyThieves.Count > 0 && !nearbyThieves[0])
            nearbyThieves.Remove(nearbyThieves[0]);

        //Deactivate a spawned alert if there are no nearby thieves
        if (alert.spawnedAlert && nearbyThieves.Count == 0)
            alert.DeactivateAlert();
    }

    public void SetSensorRange(float range)
    {
        transform.localScale = new Vector3(range, transform.localScale.y, range);
        detectionRange = range;
    }


    //Check for any thieves that enter the sphere collider of this audio sensor
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ThiefPathfinding>())
        {
            nearbyThieves.Add(other.GetComponent<ThiefPathfinding>());
            alert.SensorTriggered();

            if (directionalSignalUpgrade)
                directionalSignal.SetActive(true);
        }
    }

    //Remove any thieves from the nearbyThieves list when they leave the trigger area
    public void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<ThiefPathfinding>())
        {
            nearbyThieves.Remove(other.GetComponent<ThiefPathfinding>());
        }
    }

    //Set the direction for the directional signal to rotate towards based on the thief position
    private void SetRotationDirection()
    {
        if (nearbyThieves[0])
        {
            Vector3 rotateDir = (directionalSignal.transform.position - nearbyThieves[0].transform.position).normalized;
            rotateDir = new Vector3(-rotateDir.x, 0, rotateDir.z);

            Quaternion lookDir = Quaternion.LookRotation(rotateDir, Vector3.up);
            lookDir *= Quaternion.Euler(0, -90f, 0);

            float yRotValue = lookDir.eulerAngles.y;
            float rootAngle = transform.root.rotation.eulerAngles.y;

            directionalSignal.transform.localRotation = Quaternion.Euler(0, 0, yRotValue + rootAngle);
        }
    }
}
