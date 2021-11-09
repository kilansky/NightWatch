using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class GuardPathfinding : MonoBehaviour
{
    //Publics
    public Animator animator;
    public enum ControlMode { Idle, Patrol, Click, Manual, Chase }
    [Header("Control Options")]
    public ControlMode currControlMode = ControlMode.Idle;
    public ControlMode lastControlMode;
    public float PursuitSpeedMod;
    public float distToCatchThief;

    [Header("References")]
    public LayerMask FloorMask;
    public GameObject alertedIcon;
    public GameObject clickMoveUI;
    public GameObject clickMoveDestinationUI;

    [Header("Thief Tracking")]
    public List<GameObject> thievesSpotted = new List<GameObject>();

    [Header("Testing UI")]
    public bool displayPathfinding;

    [HideInInspector] public GameObject thiefToChase;
    [HideInInspector] public bool facingFrontDoor;



    //Privates
    private Vector3 CurrentPatrolPoint;
    private Vector3 ClickPoint;
    private Vector3 ManualPosition;
    private Vector3 offScreenPos = new Vector3(0, -10, 0);

    private Rigidbody rb;
    private NavMeshAgent Agent;
    private Camera mainCamera;
    private CameraController cameraScript;
    private DoorControl doorScript;
    private LineRenderer Line;

    private float doorOpenDelay;
    private int PatrolNumber;
    private bool DoorInteraction;
    private bool canManualMove;
    private bool speedIncreased;
    

    // Start is called before the first frame update
    void Start()
    {
        Line = GetComponent<LineRenderer>();
        canManualMove = true;
        DoorInteraction = false;
        mainCamera = Camera.main;
        cameraScript = CameraController.Instance;
        Agent = GetComponent<NavMeshAgent>();
        lastControlMode = currControlMode;
        PatrolNumber = 0;

        clickMoveUI.transform.parent = null;
        clickMoveDestinationUI.transform.parent = null;
        clickMoveUI.transform.position = offScreenPos;
        clickMoveDestinationUI.transform.position = offScreenPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.nightWatchPhase)
        {
            if (currControlMode == ControlMode.Idle)
            {
                if (thievesSpotted.Count > 0)
                {
                    print("Set to chase mode");
                    currControlMode = ControlMode.Chase;
                }
                Agent.isStopped = true;
                //print("Agent Can Not Move");
                //Do nothing
                IdleAnimation();
                ClickPoint = transform.position;
                clickMoveDestinationUI.transform.position = ClickPoint;
            }
            else if (currControlMode == ControlMode.Click)
            {
                if (thievesSpotted.Count > 0)
                {
                    print("Set to Chase mode");
                    currControlMode = ControlMode.Chase;
                }
                //Click to move
                ClickMovement();
                if (doorScript != null && doorScript.IsClosed)
                {
                    if (transform.position.x < doorScript.upperXBoundary && transform.position.x > doorScript.lowerXBoundary && transform.position.z > doorScript.lowerZBoundary && transform.position.z < doorScript.upperZBoundary)
                    {
                        //print("UpperBoundary X(" + doorScript.upperXBoundary + ") is < " + transform.position.x + " > LowerBoundary X(" + doorScript.lowerXBoundary + "), UpperBoundary Z(" + doorScript.upperZBoundary + ") is < " + transform.position.z + " > LowerBoundary Z(" + doorScript.lowerZBoundary + ")");
                        if (ClickPoint.x > doorScript.upperXBoundary || ClickPoint.x < doorScript.lowerXBoundary || ClickPoint.z < doorScript.lowerZBoundary || ClickPoint.z > doorScript.upperZBoundary)
                        {
                            DoorInteraction = true;
                            OpenDoorFunction();
                        }
                    }
                    else
                    {
                        if (ClickPoint.x < doorScript.upperXBoundary && ClickPoint.x > doorScript.lowerXBoundary && ClickPoint.z > doorScript.lowerZBoundary && ClickPoint.z < doorScript.upperZBoundary)
                        {
                            DoorInteraction = true;
                            OpenDoorFunction();
                        }
                        else
                        {
                            //print("UpperBoundary X(" + doorScript.upperXBoundary + ") is < " + ClickPoint.x + " > LowerBoundary X(" + doorScript.lowerXBoundary + "), UpperBoundary Z(" + doorScript.upperZBoundary + ") is < " + ClickPoint.z + " > LowerBoundary Z(" + doorScript.lowerZBoundary + ")");
                        }
                    }
                }
            }
            else if (currControlMode == ControlMode.Patrol)
            {
                if (thievesSpotted.Count > 0)
                {
                    print("Set to chase mode");
                    currControlMode = ControlMode.Chase;
                }
                //print("Patrol is Active");
                if (gameObject.GetComponent<GuardPatrolPoints>().PatrolPoints.Count > 0)
                {
                    //Patrol to set points
                    CurrentPatrolPoint = gameObject.GetComponent<GuardPatrolPoints>().PatrolPoints[PatrolNumber].transform.position;
                    Pathfinding();
                }
                if (doorScript != null && doorScript.IsClosed)
                {
                    if (transform.position.x < doorScript.upperXBoundary && transform.position.x > doorScript.lowerXBoundary && transform.position.z > doorScript.lowerZBoundary && transform.position.z < doorScript.upperZBoundary)
                    {
                        if (CurrentPatrolPoint.x > doorScript.upperXBoundary || CurrentPatrolPoint.x < doorScript.lowerXBoundary || CurrentPatrolPoint.z < doorScript.lowerZBoundary || CurrentPatrolPoint.z > doorScript.upperZBoundary)
                        {
                            DoorInteraction = true;
                            OpenDoorFunction();
                        }
                    }
                    else
                    {
                        if (CurrentPatrolPoint.x < doorScript.upperXBoundary && CurrentPatrolPoint.x > doorScript.lowerXBoundary && CurrentPatrolPoint.z > doorScript.lowerZBoundary && CurrentPatrolPoint.z < doorScript.upperZBoundary)
                        {
                            DoorInteraction = true;
                            OpenDoorFunction();
                        }
                    }
                }

            }
            else if (currControlMode == ControlMode.Manual)
            {
                if (canManualMove)
                {
                    //Full WASD and mouse control
                    ManualPosition = transform.position + PlayerInputs.Instance.WASDMovement * Agent.speed * Time.deltaTime;
                    if(PlayerInputs.Instance.WASDMovement == new Vector3(0, 0, 0))
                    {
                        IdleAnimation();
                    }
                    else
                    {
                        if (thiefToChase)
                        {
                            RunAnimation();
                        }
                        else
                        {
                            WalkAnimation();
                        }
                    }
                    GuardLookAtMouse();
                    cameraScript.BeginCameraFollow(transform, false);
                    cameraScript.selectedGuard = transform;
                    Agent.isStopped = true;
                }

                if (thiefToChase)
                    AttemptToCatchThief();

                if (doorScript != null)
                {
                    if (doorScript.GetComponent<DoorControl>().IsClosed)
                    {
                        if (DoorInteraction)
                        {
                            doorScript.uiNotification.SetActive(true);
                        }

                        //print("In Door Zone");
                        if (PlayerInputs.Instance.Interact)
                        {
                            canManualMove = false;
                            Agent.isStopped = false;
                            //print("Agent Can Move");
                            Vector3 waitPosition = transform.position;
                            Agent.SetDestination(waitPosition);

                            if (thiefToChase)
                            {
                                doorOpenDelay = doorScript.GetComponent<DoorControl>().chaseOpenDuration;
                            }
                            else
                            {
                                doorOpenDelay = doorScript.GetComponent<DoorControl>().openAnimationDuration;
                            }
                            StartCoroutine(OpenDelayCoroutine());
                        }
                    }
                }

            }
            else if (currControlMode == ControlMode.Chase)
            {
                if (thiefToChase)
                {
                    AttemptToCatchThief();

                    //print("Going after Thief");
                    if (DoorInteraction == false)
                    {
                        //Auto-Chase thieves
                        Agent.isStopped = false;
                        //print("Agent Can Move");
                        RunAnimation();
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
                if (doorScript != null && doorScript.IsClosed)
                {
                    if (transform.position.x < doorScript.upperXBoundary && transform.position.x > doorScript.lowerXBoundary && transform.position.z > doorScript.lowerZBoundary && transform.position.z < doorScript.upperZBoundary)
                    {
                        DoorInteraction = true;
                        OpenDoorFunction();
                    }
                    else
                    {
                        if (thievesSpotted.Count > 0)
                        {
                            if (thiefToChase.transform.position.x < doorScript.upperXBoundary && thiefToChase.transform.position.x > doorScript.lowerXBoundary && thiefToChase.transform.position.z > doorScript.lowerZBoundary && thiefToChase.transform.position.z < doorScript.upperZBoundary)
                            {
                                DoorInteraction = true;
                                OpenDoorFunction();
                            }
                        }
                    }
                }

                if (thievesSpotted.Count <= 0)
                {
                    print("Set to Last Control Mode(" + lastControlMode + ")");
                    currControlMode = lastControlMode;
                }
            }

            

            if (displayPathfinding)
                DrawPath();
        }
    }

    //Called whenever a button is pressed to change guard behaviors, or chase mode is entered
    public void ResetClickMoveUI()
    {
        clickMoveUI.transform.position = offScreenPos;
        clickMoveDestinationUI.transform.position = offScreenPos;

        if(currControlMode == ControlMode.Click)
        {
            ClickPoint = transform.position;
            clickMoveDestinationUI.transform.position = new Vector3(ClickPoint.x, 0.05f, ClickPoint.z);
        }
    }

    private void ClickMovement()
    {
        print("Click Movement being called");
        Ray ray = mainCamera.ScreenPointToRay(PlayerInputs.Instance.MousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, FloorMask))
        {
            clickMoveUI.transform.position = new Vector3(hit.point.x, 0.05f, hit.point.z);

            if (PlayerInputs.Instance.LeftClickPressed && !EventSystem.current.IsPointerOverGameObject())
            {
                NavMeshHit NavIsHit;
                int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
                if (NavMesh.SamplePosition(hit.point, out NavIsHit, 0.1f, walkableMask))
                {
                    ClickPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    clickMoveDestinationUI.transform.position = new Vector3(hit.point.x, 0.05f, hit.point.z);
                    //print("Placed New Click Point"); 
                }
            }
        }

        if (DoorInteraction == false)
        {
            
            Agent.isStopped = false;
            print("Movement allowed");
            if (Vector3.Distance(transform.position, ClickPoint) > 1)
            {
                WalkAnimation();
            }
            else
            {
                IdleAnimation();
            }
            print("Set to click");
            Agent.SetDestination(ClickPoint);
        }
        else
        {
            print("Door Interaction is true");
        }
    }

    //Follow set patrol points
    private void Pathfinding()
    {
        //print("Pathfinding On");
        if (Vector3.Distance(transform.position, CurrentPatrolPoint) < 0.5)
        {
            //print("Looking for new point");
            if (PatrolNumber < gameObject.GetComponent<GuardPatrolPoints>().PatrolPoints.Count - 1)
            {
                //print("Next Patrol Point");
                PatrolNumber += 1;
            }
            else
            {
                //print("Reset Patrol Points");
                PatrolNumber = 0;
            }
            Agent.SetDestination(CurrentPatrolPoint);
        }
        else
        {
            if (DoorInteraction == false)
            {
                Agent.isStopped = false;
                //print("Agent Can Move");
                WalkAnimation();
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
            target.gameObject.GetComponent<ThiefPathfinding>().alertedGuard.Add(gameObject);
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
            print("Set Last Control to " + currControlMode);
            lastControlMode = currControlMode;
            print("Set to Chase mode");
            currControlMode = ControlMode.Chase;
            GuardController.Instance.SetGuardBehaviorText(this, currControlMode);
        }

        alertedIcon.SetActive(true);
        GetComponent<AudioSource>().Play();
        SpeedIncrease();
        ResetClickMoveUI();
    }

    //Automatically chase thief
    private void AttemptToCatchThief()
    {
        SetNextThiefToChase();

        if (thiefToChase && Vector3.Distance(transform.position, thiefToChase.transform.position) < distToCatchThief)
            thiefToChase.GetComponent<ThiefPathfinding>().CaughtByGuard();
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
            SpeedDecrease();

            if(currControlMode != ControlMode.Manual)
            {
                print("Set to last control mode(" + lastControlMode + ")");
                currControlMode = lastControlMode;
                GuardController.Instance.SetGuardBehaviorText(this, currControlMode);
                ResetClickMoveUI();
                //print("currControlMode is " + currControlMode);
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
            Vector3 forward = new Vector3(hit.point.x, transform.position.y, hit.point.z) - transform.position;
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        transform.position += PlayerInputs.Instance.WASDMovement * Agent.speed * Time.deltaTime;
    }

    private void DrawPath()
    {
        Line.positionCount = Agent.path.corners.Length;
        Line.SetPosition(0, transform.position);

        if (Agent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < Agent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(Agent.path.corners[i].x, Agent.path.corners[i].y, Agent.path.corners[i].z);
            Line.SetPosition(i, pointPosition);
        }
    }

    //DOOR INTERACTIONS

    private void OpenDoorFunction()
    {
        print("Call Door Open Function");
        if (doorScript.IsClosed)
        {
            Vector3 waitPosition = doorScript.GetWaitPosition(transform.position);
            print("Set to wait position");
            Agent.SetDestination(waitPosition);

            if (thiefToChase)
            {
                doorOpenDelay = doorScript.GetComponent<DoorControl>().chaseOpenDuration;
            }
            else
            {
                doorOpenDelay = doorScript.GetComponent<DoorControl>().openAnimationDuration;
            }
            facingFrontDoor = false;
            StartCoroutine(OpenDelayCoroutine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Door enter while not in manual mode
        if (other.GetComponent<DoorControl>() && currControlMode != ControlMode.Manual)
        {
            doorScript = other.GetComponent<DoorControl>();
            //print("Enter Door Collider");
        }

        //Door enter while in manual mode
        if (other.GetComponent<DoorControl>() && other.GetComponent<DoorControl>().IsClosed && currControlMode == ControlMode.Manual)
        {
            DoorInteraction = true;
            doorScript = other.GetComponent<DoorControl>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GameManager.Instance.nightWatchPhase && other.GetComponent<DoorControl>())
        {
            if(other.gameObject == doorScript.gameObject)
            {
                DoorInteraction = false;
                //doorScript = other.GetComponent<DoorControl>();
                if (currControlMode == ControlMode.Manual)
                {
                    doorScript.uiNotification.SetActive(false);
                }

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
        //print("Open Door Coroutine starting");
        IdleAnimation();
        if (currControlMode != ControlMode.Manual)
        {
            while (Vector3.Distance(Agent.destination, transform.position) > Agent.stoppingDistance)
            {
                //print("Distance is " + Vector3.Distance(Agent.destination, transform.position));
                //print("Do Nothing");
                yield return null;
            }
        }
        //print("Nothing wrong with if statement");
        if (currControlMode == ControlMode.Chase)
        {
            OpenDoorAnimation();

            //print("Nothing wrong with animation");
            doorScript.GetComponent<DoorControl>().ChaseOpenDoor();
        }
        else
        {
            OpenDoorAnimation();
            //print("Nothing wrong with animation");
            doorScript.GetComponent<DoorControl>().OpenDoor();
        }
        //print("Open door function should have started");
        //print(doorOpenDelay);
        yield return new WaitForSeconds(doorOpenDelay);

        //DoorInteraction = false;
        //print("Door Interaction = " + DoorInteraction);
        if (currControlMode == ControlMode.Click)
        {
            WalkAnimation();
            print("Set Destination back to click");
            DoorInteraction = false;
            Agent.SetDestination(ClickPoint);
        }
        else if (currControlMode == ControlMode.Patrol)
        {
            WalkAnimation();
            DoorInteraction = false;
            Agent.SetDestination(CurrentPatrolPoint);
            //print("Patrol Destination = " + CurrentPatrolPoint);
        }
        else if (currControlMode == ControlMode.Chase)
        {
            if (thiefToChase)
            {
                RunAnimation();
                Agent.SetDestination(thiefToChase.transform.position);
            }
                
        }
        else if (currControlMode == ControlMode.Manual)
        {
            IdleAnimation();
            //print("Can Move");
            DoorInteraction = true;
            Agent.isStopped = true;
            //print("Agent Can Not Move");
            canManualMove = true;
        }

        doorOpenDelay = 0;
    }

    public void SpeedIncrease()
    {
        if (!speedIncreased)
        {
            //print("Speed Increase");
            Agent.speed = Agent.speed * PursuitSpeedMod;
            speedIncreased = true;
        }
    }
    public void SpeedDecrease()
    {
        if (speedIncreased)
        {
            //print("Speed Decrease");
            Agent.speed = Agent.speed / PursuitSpeedMod;
            speedIncreased = false;
        }
    }

    private void WalkAnimation()
    {
        animator.SetBool("Running", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Open Door", false);
        animator.SetBool("Walking", true);
    }

    private void RunAnimation()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Open Door", false);
        animator.SetBool("Walking", false);
        animator.SetBool("Running", true);
    }

    private void OpenDoorAnimation()
    {
        animator.SetBool("Running", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Walking", false);
        animator.SetBool("Open Door", true);
    }

    private void CelebrateAnimation()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Celebrate", true);
    }

    private void IdleAnimation()
    {
        animator.SetBool("Running", false);
        animator.SetBool("Open Door", false);
        animator.SetBool("Walking", false);
        animator.SetBool("Celebrate", false);
        animator.SetBool("Idle", true);
    }
}
