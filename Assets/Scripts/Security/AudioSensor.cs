using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Alert))]
public class AudioSensor : MonoBehaviour
{
    public float detectionRange = 5f;
    public int detectionRating = 1;
   
    void Start()
    {
        //Set scale of object to size of the detection range
        SetSensorRange(detectionRange);
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
