using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Alert))]
public class AudioSensor : MonoBehaviour
{
    public float detectionRange = 5f;
    public int detectionRating = 1;
    private TestDijkstraPath wayPointManager;
    private bool waypointsChecked;
   
    void Start()
    {
        //Set scale of object to size of the detection range
        SetSensorRange(detectionRange);
        wayPointManager = FindObjectOfType<TestDijkstraPath>();
        waypointsChecked = false;
    }

    private void Update()
    {
        if(GameManager.Instance.nightWatchPhase && !waypointsChecked)
        {
            print("Audio sensor on");
            for (int w = 0; w < wayPointManager.waypoints.Length; w++)
            {
                if (Vector3.Distance(transform.position, wayPointManager.waypoints[w].position) < ((detectionRange/2) + 1))
                {
                    print(Vector3.Distance(transform.position, wayPointManager.waypoints[w].position) + " < " + detectionRange);
                    wayPointManager.waypoints[w].GetComponent<Waypoints>().security.Add(transform.parent.gameObject);
                }
            }
            waypointsChecked = true;
        }
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
            if (other.GetComponent<ThiefPathfinding>().StealthStat <= detectionRating)
            {
                GetComponent<Alert>().SensorTriggered();
            }
            else
            {
                print("Thief is too stealthy");
            }
        }
    }
}
