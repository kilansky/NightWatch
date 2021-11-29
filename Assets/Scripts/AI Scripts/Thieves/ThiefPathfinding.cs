using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class ThiefPathfinding : MonoBehaviour
{
    public Animator animator;
    public enum BehaviorStates {Sneak, Escape, Evade}
    public enum ActionStates {Hacking, Lockpicking, Neutral}
    public BehaviorStates currBehavior = BehaviorStates.Sneak;
    public ActionStates currAction = ActionStates.Neutral;
    public float timeToSteal;    //Time it takes to steal objects
    public float stealTimeMod;
    public float StealRange;     //Distance from the target object that the thief will begin its steal action
    public float EvadeSpeedMod;
    public GameObject Target;    //Object the Thief is currently trying to steal
    public float TimeBeforeEscape; //Time before Thief will make its escape after it's stolen its last object
    public bool ShowPath; //Displays path
    public float baseSpeed;
    public float speedMod;
    public float hackingRange; //Determines how far a thief can hack
    public float hackingBaseDuration; //Determines the base duration of thief hacks
    public float hackingMod; //Determines how much each hacking tier changes the duration of hacking
    public float ExitBaseDuration;
    public float ExitDurationMod;
    public float weightModifier;
    public List<float> waypointWeights = new List<float>();

    [HideInInspector] public Transform SpawnPoint;    //The Entry Point the Thief entered the building in
    [HideInInspector] public List<GameObject> alertedGuard = new List<GameObject>();
    [Header("Thief Stats")]
    public int SpeedStat; //Int 0
    public int StealthStat; //Int 1
    public int PerceptionStat; //Int 2
    public int HackingStat; //Int 3
    public int LockpickingStat; //Int 4


    public List<Transform> ShortestPath = new List<Transform>(); //The shortest path of waypoints between the startpoint and the endpoint

    private NavMeshAgent Agent;
    private float timeRemainingToSteal;    //The progress of the steal timer
   
    private bool ObjectStolen; //Stolen
    private int ItemsHeld; //Number of target items the thief is holding
    private float doorOpenDelay; //Time the thief must stand still as the door opens
    private DoorControl doorScript; //References the DoorControl script that the thief is interacting with
    private bool DoorInteraction; //Marks that the thief is interacting with the door
    private LineRenderer Line;
    private NavMeshPath Path;
    private GameObject hackedObject; //Object thief is hacking into
    private bool Leaving;
    private int currentWaypoint; //The current waypoint the thief is traveling to
    private TestDijkstraPath wayPointManager; //The gameobject assigning the thief's paths
    private Transform lastTarget; //Previous target object
    private bool validWayPoint;
    // Start is called before the first frame update
    void Start()
    {
        wayPointManager = FindObjectOfType<TestDijkstraPath>();
        for(int w = 0; w < wayPointManager.GetComponent<TestDijkstraPath>().waypoints.Length; w++)
        {
            waypointWeights.Add(0);
        }
        wayPointManager.startPoint = SpawnPoint.GetComponent<TargetPoint>().nearestWaypoint[0];
        //Check which connected waypoint to the thief's spawn is closest to the target object
        for (int n = 0; n < SpawnPoint.GetComponent<TargetPoint>().nearestWaypoint.Length; n++)
        {
            if (Vector3.Distance(Target.transform.position, wayPointManager.startPoint.position) > Vector3.Distance(Target.transform.position, SpawnPoint.GetComponent<TargetPoint>().nearestWaypoint[n].position))
            {
                wayPointManager.startPoint = SpawnPoint.GetComponent<TargetPoint>().nearestWaypoint[n];
            }
        }
        GetPath(Target);
        
        Line = GetComponent<LineRenderer>();
        DoorInteraction = false;
        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = baseSpeed + (speedMod * SpeedStat);
        ObjectStolen = false;
        timeRemainingToSteal = timeToSteal - (stealTimeMod * HackingStat);
        StartCoroutine(EscapeTimer()); //Starts the Escape Timer
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Disables movement scripts if thief is performing an action
        if (currAction==ActionStates.Neutral)
        {
            ThiefMovementBehavior(); //Performs regular movement behavior
        }
        if (ShowPath) //Draws a line showing the thief's pathfinding
        {
            if (gameObject != null)
            {
                DrawPath();
            }
        }
    }


    //When this called at start, check which connected waypoints closest to the target object
    //When this is called after stealing an object, check which connected waypoints is closest to the target point
    //When this is called while in hall, check closest point to player
    private void GetPath(GameObject targetPoint)
    {
        ShortestPath.Clear();
        wayPointManager.endPoint = targetPoint.GetComponent<TargetPoint>().nearestWaypoint[0];
        for (int i = 0; i < targetPoint.GetComponent<TargetPoint>().nearestWaypoint.Length; i++)
        {
            //Check which connected waypoint to the thief's target point is closest to the thief
            if (Vector3.Distance(transform.position, wayPointManager.endPoint.position) > Vector3.Distance(transform.position, targetPoint.GetComponent<TargetPoint>().nearestWaypoint[i].position))
            {

                wayPointManager.endPoint = targetPoint.GetComponent<TargetPoint>().nearestWaypoint[i];
            }
        }
        wayPointManager.FindShortestPath(gameObject);
        currentWaypoint = ShortestPath.Count - 1;
    }

    private void moveToWaypoints()
    {
        Agent.SetDestination(ShortestPath[currentWaypoint].position);
        if (Vector3.Distance(transform.position, ShortestPath[currentWaypoint].position) < 1)
        {
            currentWaypoint -= 1;
        }
    }

    public void addWeight(int waypoint)
    {
        //print("add weight");
        waypointWeights[waypoint] = weightModifier;
    }
    public void removeWeight(int waypoint)
    {
        //print("Remove weight");
        waypointWeights[waypoint] = 0;
    }

    public void pathIsBlocked(GameObject BlockedPoint)
    {
        for (int n = 0; n < wayPointManager.waypoints.Length; n++)
        {
            if (Vector3.Distance(transform.position, wayPointManager.startPoint.position) > Vector3.Distance(transform.position, wayPointManager.waypoints[n].position))
            {
                wayPointManager.startPoint = wayPointManager.waypoints[n];
            }
        }
        if(currBehavior == BehaviorStates.Sneak)
        {
            GetPath(Target);
        }
        else
        {
            if (currBehavior == BehaviorStates.Escape)
            {
                GetPath(SpawnPoint.gameObject);
            }
            else
            {
                if (currBehavior == BehaviorStates.Evade)
                {
                    FindClosestEscapeRoute();
                    GetPath(SpawnPoint.gameObject);
                }
            }
        }
        
    } 

    private void ThiefMovementBehavior()
    {
        if (currBehavior == BehaviorStates.Sneak)
        {
            
            SneakBehavior();
            if (doorScript != null)
            {
                if (transform.position.x < doorScript.upperXBoundary && transform.position.x > doorScript.lowerXBoundary && transform.position.z > doorScript.lowerZBoundary && transform.position.z < doorScript.upperZBoundary)
                {
                    if (Target.transform.position.x > doorScript.upperXBoundary || Target.transform.position.x < doorScript.lowerXBoundary || Target.transform.position.z < doorScript.lowerZBoundary || Target.transform.position.z > doorScript.upperZBoundary)
                    {
                        DoorInteraction = true; //Marks that the thief is interacting with the door
                        OpenDoorFunction();
                    }
                }
                else
                {
                    if ((Target.transform.position.x < doorScript.upperXBoundary && Target.transform.position.x > doorScript.lowerXBoundary && Target.transform.position.z > doorScript.lowerZBoundary && Target.transform.position.z < doorScript.upperZBoundary) && currentWaypoint < 0)
                    {
                        DoorInteraction = true; //Marks that the thief is interacting with the door
                        OpenDoorFunction();
                    }
                    else
                    {
                        DoorInteraction = false;
                    }
                }
            }

        }
        //Escape
        else if (currBehavior == BehaviorStates.Escape)
        {
            EscapeBehavior();
            if (doorScript != null)
            {
                if (transform.position.x < doorScript.upperXBoundary && transform.position.x > doorScript.lowerXBoundary && transform.position.z > doorScript.lowerZBoundary && transform.position.z < doorScript.upperZBoundary)
                {
                    if (SpawnPoint.position.x > doorScript.upperXBoundary || SpawnPoint.position.x < doorScript.lowerXBoundary || SpawnPoint.position.z < doorScript.lowerZBoundary || SpawnPoint.position.z > doorScript.upperZBoundary)
                    {
                        DoorInteraction = true; //Marks that the thief is interacting with the door
                        OpenDoorFunction();
                    }
                }
                else
                {
                    if (SpawnPoint.position.x < doorScript.upperXBoundary && SpawnPoint.position.x > doorScript.lowerXBoundary && SpawnPoint.position.z > doorScript.lowerZBoundary && SpawnPoint.position.z < doorScript.upperZBoundary)
                    {
                        DoorInteraction = true; //Marks that the thief is interacting with the door
                        OpenDoorFunction();
                    }
                    else
                    {
                        DoorInteraction = false;
                    }
                }
            }

        }
        //Evade
        else if (currBehavior == BehaviorStates.Evade)
        {
            EvadeBehavior();
            if (doorScript != null)
            {
                if (transform.position.x < doorScript.upperXBoundary && transform.position.x > doorScript.lowerXBoundary && transform.position.z > doorScript.lowerZBoundary && transform.position.z < doorScript.upperZBoundary)
                {
                    if (SpawnPoint.position.x > doorScript.upperXBoundary || SpawnPoint.position.x < doorScript.lowerXBoundary || SpawnPoint.position.z < doorScript.lowerZBoundary || SpawnPoint.position.z > doorScript.upperZBoundary)
                    {
                        DoorInteraction = true; //Marks that the thief is interacting with the door
                        OpenDoorFunction();
                    }
                }
                else
                {
                    if (SpawnPoint.position.x < doorScript.upperXBoundary && SpawnPoint.position.x > doorScript.lowerXBoundary && SpawnPoint.position.z > doorScript.lowerZBoundary && SpawnPoint.position.z < doorScript.upperZBoundary)
                    {
                        DoorInteraction = true; //Marks that the thief is interacting with the door
                        OpenDoorFunction();
                    }
                    else
                    {
                        DoorInteraction = false;
                    }
                }
            }
        }
    }

    private void SneakBehavior()
    {
        //Checks if the Thief is close enough to steal the target object
        if (Vector3.Distance(transform.position, Target.transform.position) < StealRange)
        {
            movementAnimation();
            Agent.SetDestination(transform.position);
            StealAction();
        }
        else
        {
            if (DoorInteraction == false) //If thief is interacting with door, SetDestination does not reset
            {
                if (currentWaypoint < 0)
                {
                    Agent.SetDestination(Target.transform.position);
                }
                else
                {
                    moveToWaypoints();
                }
            }
        }
    }

    //Sneak out of the building
    private void EscapeBehavior()
    {
        if (DoorInteraction == false) //If thief is interacting with door, SetDestination does not reset
        {
            if (currentWaypoint < 0)
            {
                movementAnimation();
                Agent.SetDestination(SpawnPoint.position);
            }
            else
            {
                moveToWaypoints();
            }
        }
        if (Vector3.Distance(transform.position, SpawnPoint.position) < 0.5f && !Leaving)
        {
            StartCoroutine(ExitTimer());
        }
    }

    //Run from guard out of the building
    private void EvadeBehavior()
    {
        //print("Evading");
        if (DoorInteraction == false) //If thief is interacting with door, SetDestination does not reset
        {
            if (currentWaypoint < 0)
            {
                movementAnimation();
                Agent.SetDestination(SpawnPoint.position);
            }
            else
            {
                moveToWaypoints();
            }
        }
        //print("Distance = " + Vector3.Distance(transform.position, SpawnPoint.position));
        if (Vector3.Distance(transform.position, SpawnPoint.position) < 0.5f && !Leaving)
        {
            StartCoroutine(ExitTimer());
        }
    }

    private IEnumerator EscapeTimer()
    {
        while (TimeBeforeEscape > 0)
        {
            TimeBeforeEscape -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator ExitTimer()
    {
        //print("Exiting in " + (ExitBaseDuration - (ExitDurationMod * SpeedStat)));
        yield return new WaitForSeconds(ExitBaseDuration - (ExitDurationMod * SpeedStat));
        if (ObjectStolen == false) //Checks to see if the thief managed to steal its last object before readding it back to the target list
        {
            ThiefSpawnSystem.Instance.TargetObjects.Add(Target);
            NightHUDController.Instance.ThiefEscapedEvent();
        }
        else
            NightHUDController.Instance.ItemStolenEvent();

        ThiefSpawnSystem.Instance.ItemsLeft -= ItemsHeld; //Adjusts how many items are left after the thief stole some.

        //Remove this thief from the guards that are chasing it
        foreach (GuardPathfinding guard in FindObjectsOfType<GuardPathfinding>())
            guard.ThiefRemoved(gameObject);

        ShowPath = false;
        CheckForLevelEnd();
        Destroy(gameObject);
    }

    public void SeenByGuard()
    {
        if (currBehavior != BehaviorStates.Evade)
        {
            //Checks if hackedObject isn't null and if the currAction is Hacking
            if (hackedObject && currAction == ActionStates.Hacking)
            {
                //Allows thief to move again
                Agent.isStopped = false;
                //Sets hackedObject Hacked state to false
                hackedObject.GetComponent<HackedSecurityScript>().Hacked = false;
                //Sets currAction to Neutral
                currAction = ActionStates.Neutral;
            }
            //performAction = false;

            FindClosestEscapeRoute();
            for (int n = 0; n < wayPointManager.waypoints.Length; n++)
            {
                if (Vector3.Distance(transform.position, wayPointManager.startPoint.position) > Vector3.Distance(transform.position, wayPointManager.waypoints[n].position))
                {
                    validWayPoint = true;
                    for(int w = 0; w < wayPointManager.waypoints[n].GetComponent<Waypoints>().security.Count; w++)
                    {
                        for(int g = 0; g < alertedGuard.Count; g++)
                        {
                            //Check if waypoint is being spotted by guard
                            if (wayPointManager.waypoints[n].GetComponent<Waypoints>().security[w] != alertedGuard[g])
                            {
                                validWayPoint = false;
                                //print(gameObject + " should not go to " + wayPointManager.waypoints[n]);
                                break;
                            }
                        }   
                    }
                    if (validWayPoint == true)
                    {
                        wayPointManager.startPoint = wayPointManager.waypoints[n];
                    }
                }
            }
            GetPath(SpawnPoint.gameObject);
            currBehavior = BehaviorStates.Evade;
            Agent.speed *= EvadeSpeedMod;
        }
    }

    public void CaughtByGuard()
    {
        //Check to see if the thief managed to steal its last object before adding it back to the target list
        if (ObjectStolen == false) 
            ThiefSpawnSystem.Instance.TargetObjects.Add(Target);

        //Remove this thief from the guards that are chasing it
        foreach (GuardPathfinding guard in FindObjectsOfType<GuardPathfinding>())
            guard.ThiefRemoved(gameObject);

        NightHUDController.Instance.ThiefApprehendedEvent();
        ShowPath = false;
        CheckForLevelEnd();
        Destroy(gameObject);
    }

    //If all thieves have been spawned and this is the only thief remaining, then end the level
    private void CheckForLevelEnd()
    {
        if(ThiefSpawnSystem.Instance.allThievesSpawned)
        {
            if (FindObjectsOfType<ThiefPathfinding>().Length == 1)
                GameManager.Instance.EndLevel();
        }
    }

    //Steal Action
    private void StealAction()
    {
        //Checks to see if the steal timer is over
        if (timeRemainingToSteal > 0)
        {
            //print("Progress at " + timeRemainingToSteal);
            timeRemainingToSteal -= Time.deltaTime;
        }
        else
        {
            timeRemainingToSteal = timeToSteal - (stealTimeMod * HackingStat);
            print("Time it took " + gameObject + " to steal object: " + (timeToSteal - (stealTimeMod * HackingStat)));
            ItemsHeld += 1; //Adds one item to itemsheld
            if (TimeBeforeEscape <= 0 || ThiefSpawnSystem.Instance.TargetObjects.Count < 1) //Checks to if the Escape Timer is over and if there are any target objects left. If either are true, the thief begins the escape phase
            {
                ObjectStolen = true;
                //NEED TO CHECK IF IN BUILDING LONG ENOUGH FIRST
                wayPointManager.startPoint = Target.GetComponent<TargetPoint>().nearestWaypoint[0];
                for (int n = 0; n < Target.GetComponent<TargetPoint>().nearestWaypoint.Length; n++)
                {
                    if (Vector3.Distance(SpawnPoint.position, wayPointManager.startPoint.position) > Vector3.Distance(SpawnPoint.position, Target.GetComponent<TargetPoint>().nearestWaypoint[n].position))
                    {
                        wayPointManager.startPoint = Target.GetComponent<TargetPoint>().nearestWaypoint[n];
                    }
                }
                GetPath(SpawnPoint.gameObject);
                currBehavior = BehaviorStates.Escape;
            }
            else
            {
                lastTarget = Target.transform;
                int NextTarget;
                NextTarget = Random.Range(0, ThiefSpawnSystem.Instance.TargetObjects.Count - 1);
                Target = ThiefSpawnSystem.Instance.TargetObjects[NextTarget];
                ThiefSpawnSystem.Instance.TargetObjects.Remove(ThiefSpawnSystem.Instance.TargetObjects[NextTarget]);
                wayPointManager.startPoint = lastTarget.GetComponent<TargetPoint>().nearestWaypoint[0];
                for (int n = 0; n < lastTarget.GetComponent<TargetPoint>().nearestWaypoint.Length; n++)
                {
                    if (Vector3.Distance(Target.transform.position, wayPointManager.startPoint.position) > Vector3.Distance(Target.transform.position, lastTarget.GetComponent<TargetPoint>().nearestWaypoint[n].position))
                    {
                        wayPointManager.startPoint = lastTarget.GetComponent<TargetPoint>().nearestWaypoint[n];
                    }
                }
                GetPath(Target);
            }                    
        }      
    }

    private void FindClosestEscapeRoute()
    {
        ThiefSpawnSystem spawnSystem = FindObjectOfType<ThiefSpawnSystem>();

        for (var i = 0; i < spawnSystem.SpawnWeights.Length; i++)
        {
            if(Vector3.Distance(transform.position, SpawnPoint.position) > Vector3.Distance(transform.position, spawnSystem.Entry_Locations[i].position))
            {
                SpawnPoint = spawnSystem.Entry_Locations[i];
            }
        }
    }

    private void DrawPath()
    {
        Line.positionCount = Agent.path.corners.Length;
        Line.SetPosition(0, transform.position);

        if(Agent.path.corners.Length < 2)
        {
            return;
        }

        for(int i = 1; i < Agent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(Agent.path.corners[i].x, Agent.path.corners[i].y, Agent.path.corners[i].z);
            Line.SetPosition(i, pointPosition);
        }
    }
    //Saves reference to hackedObject and begins the hacking action
    public void CheckForHackableObjects(GameObject target)
    {
        hackedObject = target;
        //Insert function to check if hackable object is in between any of the thief's shortest path points
        hackedObject.GetComponent<HackedSecurityScript>().Hacked = true;
        StartCoroutine(HackingAction());
    }
    
    //Causes the thief to stop for a set amount of time, at the end of which will trigger the hackedObject to shut off
    private IEnumerator HackingAction() //Causes thief to stop and wait until their hacking is complete
    {
        //print("Begin Hacking");
        Agent.isStopped = true;
        currAction = ActionStates.Hacking;
        hackingAnimation();
        //Thief waits for a few seconds until the hacking duration is over
        yield return new WaitForSeconds((hackingBaseDuration - (HackingStat * hackingMod)));
        //Doesn't disable the object if the thief enters evade mode
        if (currBehavior != BehaviorStates.Evade)
        {
            //print("Finished Hacking");
            hackedObject.GetComponent<HackedSecurityScript>().HackedFunction(HackingStat);
        }
        Agent.isStopped = false;
        currAction = ActionStates.Neutral;
    }

    //Door Interactions

    private void OpenDoorFunction()
    {
        if (DoorInteraction && doorScript.IsClosed)
        {

            Vector3 waitPosition = doorScript.GetWaitPosition(transform.position); //Marks the position the thief is supposed to wait at as the door opens

            if (currBehavior == BehaviorStates.Evade)
            {
                doorOpenDelay = doorScript.chaseOpenDuration; //Saves a reference to how long the door animation lasts

            }
            else
            {
                doorOpenDelay = doorScript.openAnimationDuration; //Saves a reference to how long the door animation lasts

            }

            Agent.SetDestination(waitPosition); //Causes thief to go towards the waitPosition

            StartCoroutine(OpenDelayCoroutine());
            //print("Thief Opens Door");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DoorControl>()) //Checks if thief enters a closed door's collider
        {
            doorScript = other.GetComponent<DoorControl>(); //Creates a reusable reference for the door's script
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DoorControl>())
        {
            DoorInteraction = false;
        }
    }

    private IEnumerator OpenDelayCoroutine()
    {
        while (Vector3.Distance(Agent.destination, transform.position) > Agent.stoppingDistance + 0.5f)
        {
            //print("Do Nothing");
            yield return null;
        }

        if (currBehavior == BehaviorStates.Evade)
        {
            doorScript.GetComponent<DoorControl>().ChaseOpenDoor();
        }
        else
        {
            doorScript.GetComponent<DoorControl>().OpenDoor();
        }

        doorInteractAnimation();
        yield return new WaitForSeconds(doorOpenDelay);

        if(currBehavior == BehaviorStates.Sneak)
        {
            Agent.SetDestination(Target.transform.position);
            
        }
        else
        {
            Agent.SetDestination(SpawnPoint.position); //Triggered Error once
            
        }
        
        doorOpenDelay = 0;
    }



    private void movementAnimation()
    {
        if (currBehavior == BehaviorStates.Evade)
        {
            animator.SetBool("Sneaking", false);
            animator.SetBool("Running", true);
            animator.SetBool("DoorInteract", false);
            animator.SetBool("Hack", false);
        }
        else
        {
            animator.SetBool("Sneaking", true);
            animator.SetBool("Running", false);
            animator.SetBool("DoorInteract", false);
            animator.SetBool("Hack", false);
        }
    }
    private void doorInteractAnimation()
    {
        animator.SetBool("Sneaking", false);
        animator.SetBool("Running", false);
        animator.SetBool("DoorInteract", true);
        animator.SetBool("Hack", false);
    }
    private void hackingAnimation()
    {
        animator.SetBool("Sneaking", false);
        animator.SetBool("Running", false);
        animator.SetBool("DoorInteract", false);
        animator.SetBool("Hack", true);
    }
}
