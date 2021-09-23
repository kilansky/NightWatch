using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardPathfinding : MonoBehaviour
{
    public enum ControlMode { Idle, Patrol, Click, Manual, Chase }
    public ControlMode currControlMode = ControlMode.Idle;

    public NavMeshAgent Agent;
    public float PursuitSpeedMod;
    public float distToCatchThief;

    [HideInInspector] public GameObject Thief;
    [HideInInspector] public bool ThiefSpotted;

    private GameObject PatrolSource;
    private int PatrolNumber;
    private Vector3 CurrentPatrolPoint;
    private Rigidbody rb;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        ThiefSpotted = false;
        currControlMode = ControlMode.Idle;
        //PatrolNumber = 0;
        //CurrentPatrolPoint = PatrolSource.GetComponent<GuardPatrolPoints>().Points[PatrolNumber];
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
            }
            else if (currControlMode == ControlMode.Patrol)
            {
                //Patrol to set points
                Pathfinding();
            }
            else if (currControlMode == ControlMode.Manual)
            {
                //Full WASD and mouse control
                GuardLookAtMouse();
            }
            else if (currControlMode == ControlMode.Chase)
            {
                //Auto-Chase thieves
            }
        }
    }

    //Follow set patrol points
    private void Pathfinding()
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

        transform.position += PlayerInputs.Instance.CameraMovement * Agent.speed * Time.deltaTime;

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
