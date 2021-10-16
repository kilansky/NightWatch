using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAwareness : FieldOfView
{
    public override void FindVisibleTargets()
    {
        //Place For loop here
        visibleTargets.Clear(); //clear the current list of existing targets

        //Get an array of all targets within a sphere radius
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        //Check each target found to see if they are within view
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform; //Get target transform
            Vector3 dirToTarget = (target.position - transform.position).normalized; //Get vector towards target

            //Check if target is within the 'viewAngle'
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                //Perform raycast to make sure target is not behind a wall
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    transform.parent.GetComponent<FieldOfView>().visibleTargets.Add(target); //Target is visible to parent FOV class
                    visibleTargets.Add(target); //Target is visible to this class - used only for debugging
                }
            }
        }
    }
}
