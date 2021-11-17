using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Alert))]
public class Laser : MonoBehaviour
{
    public LayerMask laserHitMask;
    public LayerMask thiefMask;
    public LayerMask wayPointMask;
    public GameObject FieldOfView;
    public bool pinpointAlert = false;


    private LineRenderer lineRenderer;
    private float longestDis;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50f, laserHitMask))
        {
            lineRenderer.SetPosition(1, new Vector3(0, 0, hit.distance));

            //Check for thieves
            if (Physics.Raycast(ray, out hit, hit.distance + 0.5f, thiefMask))
            {
                //If pinpoint upgrade has been purchased: Sets the alert spawn position to the 
                //position of the triggered thief, and updates the alert position as the thief moves
                if (pinpointAlert)
                {
                    GetComponent<Alert>().spawnPosition = hit.point;

                    if (GetComponent<Alert>().spawnedAlert)//Update position of spawned alert
                        GetComponent<Alert>().spawnedAlert.transform.position = hit.point;
                }

                GetComponent<Alert>().SensorTriggered();
            }

        }
        if (Physics.Raycast(ray, out hit, hit.distance + 0.5f, wayPointMask))
        {
            print("Hit " + hit.transform.gameObject);
        }



        //Draw laser line renderer

    }


    /**/
}
