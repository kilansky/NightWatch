using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class GuardPathfinding : MonoBehaviour
{
    public enum ControlMode { Idle, Patrol, Click, Manual, Chase }
    public ControlMode currControlMode = ControlMode.Idle;
    public LayerMask FloorMask;
    public NavMeshAgent Agent;
    public GameObject alertedIcon;
    public float PursuitSpeedMod;
    public float distToCatchThief;

    public List<GameObject> thievesSpotted = new List<GameObject>();
    [HideInInspector] public GameObject thiefToChase;
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
    private CameraController cameraScript;
    private bool canManualMove;
    private bool speedIncreased;
    private ControlMode lastControlMode;
    // Start is called before the first frame update
    void Start()
    {
        canManualMove = true;
        DoorInteraction = false;
        BeginPatrol = false;
        mainCamera = Camera.main;
        cameraScript = CameraController.Instance;
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
                Agent.isStopped = false;
                //Do nothing
                ClickPoint = transform.position;
            }
            else if (currControlMode == ControlMode.Click)
            {
                //Click to move
                ClickMovement();
                OpenDoorFunction();
            }
            else if (currControlMode == ControlMode.Patrol)
            {
                print("Patrol is Active");
                if (gameObject.GetComponent<GuardPatrolPoints>().PatrolPoints.Count > 0)
                {
                    //Patrol to set points
                    CurrentPatrolPoint = gameObject.GetComponent<GuardPatrolPoints>().PatrolPoints[PatrolNumber].transform.position;
                    Pathfinding();
                }
                OpenDoorFunction();
            }
            else if (currControlMode == ControlMode.Manual)
            {
                if (canManualMove)
                {
                    //Full WASD and mouse control
                    ManualPosition = transform.position + PlayerInputs.Instance.WASDMovement * Agent.speed * Time.deltaTime;
                    GuardLookAtMouse();
                    cameraScript.followGuard = true;
                    cameraScript.CameraFollow(transform);
                    cameraScript.selectedGuard = transform;
                    Agent.isStopped = true;
                }

                if (thiefToChase)
                    AttemptToCatchThief();
                //DoorInteraction && 
                if (doorInteractingwith.GetComponent<DoorControl>().IsClosed)
                {
                    if (DoorInteraction)
                    {
                        doorInteractingwith.uiNotification.SetActive(true);
                    }
                    
                    print("In Door Zone");
                    Keyboard kb = InputSystem.GetDevice<Keyboard>();
                    if (kb.eKey.wasPressedThisFrame)
                    {
                        print("E Pressed");
                        canManualMove = false;
                        Agent.isStopped = false;
                        Vector3 waitPosition = transform.position;
                        Agent.SetDestination(waitPosition);

                        if (thiefToChase)
                        {
                            doorOpenDelay = doorInteractingwith.GetComponent<DoorControl>().chaseOpenDuration;
                        }
                        else
                        {
                            doorOpenDelay = doorInteractingwith.GetComponent<DoorControl>().openAnimationDuration;
                        }
                        StartCoroutine(OpenDelayCoroutine());
                    }
                }
            }
            else if (currControlMode == ControlMode.Chase)
            {
                if (thiefToChase)
                {
                    AttemptToCatchThief();

                    print("Going after Thief");
                    if (DoorInteraction == false)
                    {
                        //Auto-Chase thieves
                        Agent.isStopped = false;
                        Agent.SetDestination(thiefToChase.transform.position);
                    }
                }

                List<GameObject> nullThieves = new List<GameObject>();
                foreach (GameObject thief in thievesSpotted)
                {
                    if (!thief)
                        nullThieves.Add(thief);
                }
                foreach (GameObject thief in nullThieves)
                {
                    thievesSpotted.Remove(thief);
                }
                OpenDoorFunction();
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
                    if (DoorInteraction == false)
                    {
                        Agent.isStopped = false;
                        Agent.SetDestination(ClickPoint);
                        print("Set Destination is " + Agent.destination);
                    }
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
                Agent.isStopped = false;
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

    public void ThiefRemoved(GameObject target)
    {
        thievesSpotted.Remove(target);
        CheckToEndChase();
    }

    //Called when multiple thieves have been spotted, and one of the thieves has escaped or been caught
    private void SetNextThiefToChase()
    {
        //If there are no thieves, return null
        if(thievesSpotted.Count == 0)
        {
            thiefToChase = null;
            CheckToEndChase();
            return;
        }
        //If there is only one seen thief and it is not null, set it as the thief to chase
        else if (thievesSpotted.Count == 1 && thievesSpotted[0])
        {
            thiefToChase = thievesSpotted[0];
            return;
        }

        //If there are multiple seen thieves, find the closest one to the guard set it as the thief to chase
        float closestThief = Mathf.Infinity;
        List<GameObject> nullThieves = new List<GameObject>();
        foreach (GameObject thief in thievesSpotted)
        {
            if (thief)
            {
                float distToThief = Vector3.Distance(transform.position, thief.transform.position);
                if (distToThief < closestThief)
                    thiefToChase = thief;
            }
        }
    }

    //Activates the alerted icon, initiates the speed increase for the guard, and begins Chase behavior
    public void BeginChasingThief()
    {
        if(currControlMode != ControlMode.Manual)
        {
            lastControlMode = currControlMode;
            currControlMode = ControlMode.Chase;
        }

        alertedIcon.SetActive(true);
        GetComponent<AudioSource>().Play();
        print("First Thief");
        SpeedIncrease();
    }

    //Automatically chase thief
    private void AttemptToCatchThief()
    {
        SetNextThiefToChase();

        if (thiefToChase && Vector3.Distance(transform.position, thiefToChase.transform.position) < distToCatchThief)
        {
            print("CatchThief");
            thiefToChase.GetComponent<ThiefPathfinding>().CaughtByGuard();
            ThiefRemoved(thiefToChase);
        }
    }

    //Checks if there are more thieves to chase, or to end the chase
    private void CheckToEndChase()
    {
        //Check to go after the next thief spotted
        if (thievesSpotted.Count > 0)
            SetNextThiefToChase();
        //If there are no other spotted thieves, return to last control mode
        else
        {
            print("Chase Ended");
            SpeedDecrease();

            if(currControlMode != ControlMode.Manual)
            {
                currControlMode = lastControlMode;
                print("currControlMode is " + currControlMode);
            }

            alertedIcon.SetActive(false);
        }
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
    }

    //DOOR INTERACTIONS

    private void OpenDoorFunction()
    {
        if (DoorInteraction && doorInteractingwith.IsClosed)
        {
            Vector3 waitPosition = doorInteractingwith.GetWaitPosition(transform.position);

            Debug.LogWarning("KNOWN ERROR: NullReferenceException");
            Agent.SetDestination(waitPosition);

            if (thiefToChase)
            {
                doorOpenDelay = doorInteractingwith.GetComponent<DoorControl>().chaseOpenDuration;
            }
            else
            {
                doorOpenDelay = doorInteractingwith.GetComponent<DoorControl>().openAnimationDuration;
            }

            StartCoroutine(OpenDelayCoroutine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Door enter while not in manual mode
        if (other.GetComponent<DoorControl>() && other.GetComponent<DoorControl>().IsClosed && currControlMode != ControlMode.Manual && GameManager.Instance.nightWatchPhase)
        {
            DoorInteraction = true;
            doorInteractingwith = other.GetComponent<DoorControl>();
            
        }

        //Door enter while in manual mode
        if (other.GetComponent<DoorControl>() && other.GetComponent<DoorControl>().IsClosed && currControlMode == ControlMode.Manual)
        {
            
            DoorInteraction = true;
            doorInteractingwith = other.GetComponent<DoorControl>();
            
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DoorControl>())
        {
            DoorInteraction = false;
            doorInteractingwith = other.GetComponent<DoorControl>();
            if (currControlMode == ControlMode.Manual)
            {
                doorInteractingwith.uiNotification.SetActive(false);
            }
            
        }

        

        /* //Exit door collider w/o opening it
         if (other.GetComponent<DoorControl>() && other.GetComponent<DoorControl>().IsClosed)
             DoorInteraction = false;

         //Exit door collider after opeing it
         if (other.GetComponent<DoorControl>() && other.GetComponent<DoorControl>().IsOpened)
         {
             DoorInteraction = true;
             if (currControlMode == ControlMode.Manual)
             {
                 canManualMove = false;
                 Agent.isStopped = false;
                 Vector3 waitPosition = transform.position;
                 Agent.SetDestination(waitPosition);
             }
             else
             {              
                 Agent.isStopped = true;
                 print("Stop Moving");
             }

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
         }*/
    }

    private IEnumerator OpenDelayCoroutine()
    {
        if (currControlMode != ControlMode.Manual)
        {
            while (Vector3.Distance(Agent.destination, transform.position) > Agent.stoppingDistance)
            {
                print("Do Nothing");
                yield return null;
            }
        }        

        if(currControlMode == ControlMode.Chase)
        {
            doorInteractingwith.GetComponent<DoorControl>().ChaseOpenDoor();
        }
        else
        {
            doorInteractingwith.GetComponent<DoorControl>().OpenDoor();
        }

        print(doorOpenDelay);
        yield return new WaitForSeconds(doorOpenDelay);
       
        //DoorInteraction = false;
        print("Door Interaction = " + DoorInteraction);
        if (currControlMode == ControlMode.Click)
        {
            Agent.SetDestination(ClickPoint);
        }
        else if (currControlMode == ControlMode.Patrol)
        {
            Agent.SetDestination(CurrentPatrolPoint);
            print("Patrol Destination = " + CurrentPatrolPoint);
        }
        else if (currControlMode == ControlMode.Chase)
        {          
            if(thiefToChase)
                Agent.SetDestination(thiefToChase.transform.position);
        }
        else if (currControlMode == ControlMode.Manual)
        {
            print("Can Move");
            DoorInteraction = true;
            Agent.isStopped = true;
            canManualMove = true;
        }

        doorOpenDelay = 0;
    }

    /*private IEnumerator CloseDelayCoroutine()
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

        if (currControlMode == ControlMode.Manual)
        {
            canManualMove = true;
            Agent.isStopped = true;
        }
        else
        {
            canManualMove = true;
            DoorInteraction = false;
            Agent.isStopped = false;
        }      

        doorOpenDelay = 0;
    }*/

    public void SpeedIncrease()
    {
        if (!speedIncreased)
        {
            print("Speed Increase");
            Agent.speed = Agent.speed * PursuitSpeedMod;
            speedIncreased = true;
        }
    }
    public void SpeedDecrease()
    {
        if (speedIncreased)
        {
            print("Speed Decrease");
            Agent.speed = Agent.speed / PursuitSpeedMod;
            speedIncreased = false;
        }
    }
}
