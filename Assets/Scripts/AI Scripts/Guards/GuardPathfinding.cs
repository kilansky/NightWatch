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
    public GameObject alertedIcon;
    public float PursuitSpeedMod;
    public float distToCatchThief;


    [HideInInspector] public GameObject Thief;
    [HideInInspector] public bool ThiefSpotted;
    [HideInInspector] public bool BeginPatrol;

    private int PatrolNumber;
    private Vector3 CurrentPatrolPoint;
    private Vector3 ClickPoint;
    private Rigidbody rb;
    private Camera mainCamera;
    private float doorOpenDelay;
    private DoorControl doorInteractingwith;
    private bool DoorInteraction;
    private Vector3 ManualPosition;
    private GameObject cameraScript;
    // Start is called before the first frame update
    void Start()
    {

        DoorInteraction = false;
        BeginPatrol = false;
        ThiefSpotted = false;
        mainCamera = Camera.main;
        cameraScript = GameObject.FindGameObjectWithTag("CameraScript");
        //currControlMode = ControlMode.Idle;
        PatrolNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.nightWatchPhase)
        {
            if (ThiefSpotted == true)
            {
                currControlMode = ControlMode.Chase;
            }
            if (currControlMode == ControlMode.Idle)
            {
                cameraScript.GetComponent<CameraController>().followGuard = false;
                //Do nothing
                ClickPoint = transform.position;
            }
            else if (currControlMode == ControlMode.Click)
            {

                cameraScript.GetComponent<CameraController>().followGuard = false;
                //Click to move
                ClickMovement();
                
                
            }
            else if (currControlMode == ControlMode.Patrol)
            {
                cameraScript.GetComponent<CameraController>().followGuard = false;
                if (gameObject.GetComponent<GuardPatrolPoints>().PatrolPoints.Count > 0)
                {
                    //Patrol to set points
                    CurrentPatrolPoint = gameObject.GetComponent<GuardPatrolPoints>().PatrolPoints[PatrolNumber].transform.position;
                    Pathfinding();

                }



            }
            else if (currControlMode == ControlMode.Manual)
            {
                //Full WASD and mouse control
                ManualPosition = transform.position + PlayerInputs.Instance.WASDMovement * Agent.speed * Time.deltaTime;
                GuardLookAtMouse();
                cameraScript.GetComponent<CameraController>().followGuard = true;
                cameraScript.GetComponent<CameraController>().selectedGuard = transform;
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

                    ClickPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    Agent.SetDestination(ClickPoint);
                    print("Set Destination is " + Agent.destination);

                }
            }
        }
        
    }

    //Follow set patrol points
    private void Pathfinding()
    {
        //print("Pathfinding On");
        if (Vector3.Distance(transform.position, CurrentPatrolPoint) < 0.5)
        {
            print("Looking for new point");
            if (PatrolNumber < gameObject.GetComponent<GuardPatrolPoints>().PatrolPoints.Count - 1)
            {
                print("Next Patrol Point");
                PatrolNumber += 1;
            }
            else
            {
                print("Reset Patrol Points");
                PatrolNumber = 0;
            }
            Agent.SetDestination(CurrentPatrolPoint);
        }
        else
        {
            if (DoorInteraction == false)
            {
                Agent.SetDestination(CurrentPatrolPoint);
            }
            
        }
        
    }

    //Activates the alerted icon, initiates the speed increase for the guard, and begins Chase behavior
    public void BeginChasingThief()
    {
        ThiefSpotted = true;
        alertedIcon.SetActive(true);
        SpeedIncrease();
    }

    //Automatically chase thief
    private void Chase()
    {
        if (Thief == null)
        {
            print("Thief Gone");
            ThiefSpotted = false;
            alertedIcon.SetActive(false);
            SpeedDecrease();
            currControlMode = ControlMode.Idle;
        }
        else
        {
            print("Going after Thief");
            if (DoorInteraction == false)
            {

                Agent.SetDestination(Thief.transform.position);
            }
            
            if (Vector3.Distance(transform.position, Thief.transform.position) < distToCatchThief)
            {
                CatchThief();
            }
                
        }
    }

    //Catching Thief
    private void CatchThief()
    {
        print("CatchThief");
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

    //DOOR INTERACTIONS

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Door" && other.GetComponent<DoorControl>().IsClosed)
        {
            DoorInteraction = true;

            doorInteractingwith = other.GetComponent<DoorControl>();
            Vector3 waitPosition = doorInteractingwith.GetWaitPosition(transform.position);

            if (currControlMode == ControlMode.Chase)
            {
                doorOpenDelay = other.GetComponent<DoorControl>().chaseOpenDuration;
            }
            else
            {
                doorOpenDelay = other.GetComponent<DoorControl>().openAnimationDuration;
            }
            

            Agent.SetDestination(waitPosition);
            print("Set Destination is " + Agent.destination + ", waitPosition is " + waitPosition);

            StartCoroutine(OpenDelayCoroutine());
            print("Guard Opens Door");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Door" && other.GetComponent<DoorControl>().IsOpened)
        {
            Agent.isStopped = true;
            if (currControlMode == ControlMode.Chase)
            {
                doorOpenDelay = other.GetComponent<DoorControl>().chaseCloseDuration;
            }
            else
            {
                doorOpenDelay = other.GetComponent<DoorControl>().closeAnimationDuration;
            }
                
            StartCoroutine(CloseDelayCoroutine());
            print("Guard Closes Door");
        }
    }

    private IEnumerator OpenDelayCoroutine()
    {
        while(Vector3.Distance(Agent.destination, transform.position) > Agent.stoppingDistance)
        {
            print("Do Nothing");
            yield return null;
        }

        if(currControlMode == ControlMode.Chase)
        {
            doorInteractingwith.GetComponent<DoorControl>().ChaseOpenDoor();
        }
        else
        {
            doorInteractingwith.GetComponent<DoorControl>().OpenDoor();
        }
        

        yield return new WaitForSeconds(doorOpenDelay);

        if(currControlMode == ControlMode.Click)
        {
            DoorInteraction = false;
            Agent.SetDestination(ClickPoint);
        }
        else if (currControlMode == ControlMode.Patrol)
        {
            DoorInteraction = false;
            Agent.SetDestination(ClickPoint);
        }
        else if (currControlMode == ControlMode.Chase)
        {
            DoorInteraction = false;
            Agent.SetDestination(Thief.transform.position);
        }

        doorOpenDelay = 0;
    }
    private IEnumerator CloseDelayCoroutine()
    {
        if (currControlMode == ControlMode.Chase)
        {
            doorInteractingwith.GetComponent<DoorControl>().ChaseCloseDoor();
        }
        else
        {
            doorInteractingwith.GetComponent<DoorControl>().CloseDoor();
        }
        

        yield return new WaitForSeconds(doorOpenDelay);


        Agent.isStopped = false;

        doorOpenDelay = 0;
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
