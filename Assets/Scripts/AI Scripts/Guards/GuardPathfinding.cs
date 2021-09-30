using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPathfinding : MonoBehaviour
{
    public enum ControlMode { Idle, Patrol, Click, Manual, Chase }
    public ControlMode currControlMode = ControlMode.Idle;
    public LayerMask FloorMask;
    public NavMeshAgent Agent;
    public float PursuitSpeedMod;
    public float distToCatchThief;

    [HideInInspector] public GameObject Thief;
    [HideInInspector] public bool ThiefSpotted;
    [HideInInspector] public bool BeginPatrol;

    private int PatrolNumber;
    private Vector3 CurrentPatrolPoint;
    private Rigidbody rb;
    private Camera mainCamera;
    


    // Start is called before the first frame update
    void Start()
    {
        BeginPatrol = false;
        ThiefSpotted = false;
        //currControlMode = ControlMode.Idle;
        PatrolNumber = 0;
        //CurrentPatrolPoint = new Vector3(gameObject.GetComponent<GuardPatrolPoints>().Points[PatrolNumber].x, gameObject.GetComponent<GuardPatrolPoints>().Points[PatrolNumber].y, gameObject.GetComponent<GuardPatrolPoints>().Points[PatrolNumber].z);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.nightWatchPhase)
        {
            if (currControlMode == ControlMode.Idle)
            {
                //Do nothing
            }
            else if (currControlMode == ControlMode.Click)
            {
                //Click to move
                ClickMovement();
            }
            else if (currControlMode == ControlMode.Patrol)
            {
                if(gameObject.GetComponent<GuardPatrolPoints>().Points.Count > 0)
                {
                    //Patrol to set points
                    CurrentPatrolPoint = new Vector3(gameObject.GetComponent<GuardPatrolPoints>().Points[PatrolNumber].x, gameObject.GetComponent<GuardPatrolPoints>().Points[PatrolNumber].y, gameObject.GetComponent<GuardPatrolPoints>().Points[PatrolNumber].z);
                    Pathfinding();

                }



            }
            else if (currControlMode == ControlMode.Manual)
            {
                //Full WASD and mouse control
                GuardLookAtMouse();
            }
            else if (currControlMode == ControlMode.Chase)
            {
                //Auto-Chase thieves
                Chase();
            }
        }
    }

    private void ClickMovement()
    {
        if (PlayerInputs.Instance.LeftClickPressed)
        {
            Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, FloorMask))
            {
                NavMeshHit NavIsHit;
                int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
                if (NavMesh.SamplePosition(hit.point, out NavIsHit, 0.1f, walkableMask))
                {

                    Vector3 target = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    

                }
            }
        }
    }

    //Follow set patrol points
    private void Pathfinding()
    {
        print("Pathfinding On");
        print("Current Patrol Point is at " + CurrentPatrolPoint);
        print("Distance from Patrol Point is " + Vector3.Distance(transform.position, CurrentPatrolPoint));
        if (Vector3.Distance(transform.position, CurrentPatrolPoint) < 0.5)
        {
            print("Looking for new point");
            if (PatrolNumber < gameObject.GetComponent<GuardPatrolPoints>().Points.Count - 1)
            {
                print("Next Patrol Point");
                PatrolNumber += 1;
            }
            else
            {
                print("Reset Patrol Points");
                PatrolNumber = 0;
            }
        }
        else
        {
            Agent.SetDestination(CurrentPatrolPoint);
        }
        
    }

    //Automatically chase thief
    private void Chase()
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

            if (Vector3.Distance(transform.position, Thief.transform.position) < distToCatchThief)
                CatchThief();
        }
    }

    private void CatchThief()
    {
        Thief.GetComponent<ThiefPathfinding>().CaughtByGuard();
        currControlMode = ControlMode.Idle;
    }

    //Rotates guard in direction of mouse pointer
    private void GuardLookAtMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, SecurityPlacement.Instance.floorMask))
        {
            Vector3 forward = hit.point - transform.position;
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        transform.position += PlayerInputs.Instance.WASDMovement * Agent.speed * Time.deltaTime;

        //SWITCH CAMERA TO FOLLOW THE GUARD HERE!!
    }

    public void SpeedIncrease()
    {
        //print("Speed Increase");
        Agent.speed = Agent.speed * PursuitSpeedMod;
    }
    public void SpeedDecrease()
    {
        //print("Speed Decrease");
        Agent.speed = Agent.speed / PursuitSpeedMod;
    }
}
