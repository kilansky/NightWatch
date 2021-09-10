using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPathfinding : MonoBehaviour
{
    public NavMeshAgent Agent;

    public GameObject PatrolSource;

    private int PatrolNumber;

    private Transform CurrentPatrolPoint;

    private bool ThiefSpotted;

    private GameObject Thief;

    // Start is called before the first frame update
    void Start()
    {
        ThiefSpotted = false;
        PatrolNumber = 0;
        CurrentPatrolPoint = PatrolSource.GetComponent<GuardPatrolPoints>().PatrolPoints[PatrolNumber];
    }

    // Update is called once per frame
    void Update()
    {
        if (ThiefSpotted == false)
        {
            if (Vector3.Distance(transform.position, CurrentPatrolPoint.position) < 0.5)
            {
                if (PatrolNumber < PatrolSource.GetComponent<GuardPatrolPoints>().PatrolPoints.Length - 1)
                {
                    PatrolNumber += 1;
                }
                else
                {
                    PatrolNumber = 0;
                }
                CurrentPatrolPoint = PatrolSource.GetComponent<GuardPatrolPoints>().PatrolPoints[PatrolNumber];
            }
            else
            {
                Agent.SetDestination(CurrentPatrolPoint.position);
            }
        }
        else
        {

            Agent.SetDestination(Thief.transform.position);
        }
    }
}
