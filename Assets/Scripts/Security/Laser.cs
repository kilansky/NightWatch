using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Alert))]
public class Laser : MonoBehaviour
{
    public LayerMask laserHitMask;
    public LayerMask thiefMask;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //Draw laser line renderer
        if (Physics.Raycast(ray, out hit, 50f, laserHitMask))
        {
            lineRenderer.SetPosition(1, new Vector3(0, 0, hit.distance));

            //Check for thieves
            if (Physics.Raycast(ray, out hit, hit.distance + 0.5f, thiefMask))
            {
                GetComponent<Alert>().SensorTriggered();
            }
        }
    }
}
