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

    [HideInInspector] public GameObject thiefToChase;
    [HideInInspector] public bool BeginPatrol;
    public List<GameObject> thievesSpotted = new List<GameObject>();

    private int PatrolNumber;
    private Vector3 CurrentPatrolPoint;
    private Vector3 ClickPoint;
    private Rigidbody rb;
    private Camera mainCamera;
    private float doorOpenDelay;
    private DoorControl doorInteractingwith;
    private bool DoorInteraction;
    private ControlMode lastControlMode;

    // Start is called before the first frame update
    void Start()
    {
        DoorInteraction = false;
        BeginPatrol = false;
        mainCamera = Camera.main;
        //currControlMode = ControlMode.Idle;
        lastControlMode = currControlMode;
        PatrolNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.nightWatchPhase)
        {
            if (currControlMode == ControlMode.Idle)
            {
                //Do nothing
                ClickPoint = transform.position;
            }
            else if (currControlMode == ControlMode.Click)
            {
                
                
                //Click to move
                ClickMovement();
                
                
            }
            else if (currControlMode == ControlMode.Patrol)
            {
                if(gameObject.GetComponent<GuardPatrolPoints>().PatrolPoints.Count > 0)
                {
                    //Patrol to set points
                    CurrentPatrolPoint = gameObject.GetComponent<GuardPatrolPoints>().PatrolPoints[PatrolNumber].transform.position;
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

    //Called when a thief is spotted from the FOV script
    public void ThiefSpotted(GameObject target)
    {
        //Check if the spotten thief is already known about by the guard (has been seen recently)
        bool knownThief = false;
        foreach (GameObject thief in thievesSpotted)
        {
            if (thief == target)
            {
                knownThief = true;
                return;
            }
        }

        //If this thief is new, add it to the thieves spotted list and begin chasing interaction
        if (!knownThief)
        {
            thievesSpotted.Add(target);
            target.GetComponent<ThiefPathfinding>().SeenByGuard();

            if (!thiefToChase)
                BeginChasingThief();

            SetNextThiefToChase();
        }
    }

    //Activates the alerted icon, initiates the speed increase for the guard, and begins Chase behavior
    public void BeginChasingThief()
    {
        lastControlMode = currControlMode;
        currControlMode = ControlMode.Chase;
        alertedIcon.SetActive(true);
        GetComponent<AudioSource>().Play();
        SpeedIncrease();
    }

    //Called when multiple thieves have been spotted, and one of the thieves has escaped or been caught
    private void SetNextThiefToChase()
    {
        //If there is only one seen thief, set it as the thief to chase
        if (thievesSpotted.Count == 1)
        {
            thiefToChase = thievesSpotted[0];
            return;
        }

        //If there are multiple seen thieves, find the closest one to the guard set it as the thief to chase
        float closestThief = Mathf.Infinity;
        foreach (GameObject thief in thievesSpotted)
        {
            float distToThief = Vector3.Distance(transform.position, thief.transform.position);
            if(distToThief < closestThief)
            {
                thiefToChase = thief;
            }
        }
    }

    //Automatically chase thief
    private void Chase()
    {
        if (thiefToChase == null)
        {
            print("Thief Gone");
            alertedIcon.SetActive(false);
            SpeedDecrease();
            currControlMode = ControlMode.Idle;
        }
        else
        {
            print("Going after Thief");
            if (DoorInteraction == false)
            {

                Agent.SetDestination(thiefToChase.transform.position);
            }
            
            if (Vector3.Distance(transform.position, thiefToChase.transform.position) < distToCatchThief)
            {
                CatchThief();
            }               
        }
    }

    //Catching Thief
    private void CatchThief()
    {
        print("CatchThief");
        thiefToChase.GetComponent<ThiefPathfinding>().CaughtByGuard();
        thievesSpotted.Remove(thiefToChase);

        //Check to go after the next thief spotted
        if (thievesSpotted.Count > 0)
            SetNextThiefToChase();
        //If there are no other spotted thieves, return to last control mode
        else
            currControlMode = lastControlMode; 
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
            Agent.SetDestination(thiefToChase.transform.position);
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
