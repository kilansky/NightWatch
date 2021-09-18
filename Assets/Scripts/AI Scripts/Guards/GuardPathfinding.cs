using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPathfinding : MonoBehaviour
{
    public bool NightPhase;

    public NavMeshAgent Agent;

    public GameObject PatrolSource;

    private int PatrolNumber;

    private Vector3 CurrentPatrolPoint;

    public bool ThiefSpotted;

    public GameObject Thief;

    public float PursuitIncrease;

    // Start is called before the first frame update
    void Start()
    {
        ThiefSpotted = false;
        PatrolNumber = 0;
        CurrentPatrolPoint = PatrolSource.GetComponent<GuardPatrolPoints>().Points[PatrolNumber];
    }

    // Update is called once per frame
    void Update()
    {
        if (NightPhase == true)
        {
            Pathfinding();
        }
    }

    private void Pathfinding()
    {
        if (ThiefSpotted == false)
        {
            if (Vector3.Distance(transform.position, CurrentPatrolPoint) < 0.5)
            {
                if (PatrolNumber < PatrolSource.GetComponent<GuardPatrolPoints>().Points.Count - 1)
                {
                    PatrolNumber += 1;
                }
                else
                {
                    PatrolNumber = 0;
                }
                CurrentPatrolPoint = new Vector3(PatrolSource.GetComponent<GuardPatrolPoints>().Points[PatrolNumber].x, 0, PatrolSource.GetComponent<GuardPatrolPoints>().Points[PatrolNumber].z);
            }
            else
            {
                Agent.SetDestination(CurrentPatrolPoint);
            }
        }
        else
        {


            if (Thief == null)
            {
                print("Thief Gone");
                ThiefSpotted = false;
                SpeedDecrease();
                Agent.SetDestination(CurrentPatrolPoint);
            }
            else
            {
                Agent.SetDestination(Thief.transform.position);
            }
        }
    }

    public void SpeedIncrease()
    {
        print("Speed Increase");
        Agent.speed = Agent.speed * PursuitIncrease;
    }
    public void SpeedDecrease()
    {
        print("Speed Decrease");
        Agent.speed = Agent.speed / PursuitIncrease;
    }
}
