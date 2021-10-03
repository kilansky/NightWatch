using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThiefPathfinding : MonoBehaviour
{
    

    public enum BehaviorStates {Sneak, Escape, Evade, SkillCheck }
    public BehaviorStates currBehavior = BehaviorStates.Sneak;
    public float timeToSteal;    //Time it takes to steal objects
    public float StealRange;     //Distance from the target object that the thief will begin its steal action
    public float EvadeSpeedMod;
    public GameObject Target;    //Object the Thief is currently trying to steal
    public float TimeBeforeEscape; //Time before Thief will make its escape after it's stolen its last object

    [HideInInspector] public Transform SpawnPoint;    //The Entry Point the Thief entered the building in
    public int SpeedStat; //Int 0
    public int StealthStat; //Int 1
    public int PerceptionStat; //Int 2
    public int HackingStat; //Int 3
    public int LockpickingStat; //Int 4


    private NavMeshAgent Agent;
    private float timeRemainingToSteal;    //The progress of the steal timer
    private bool ObjectStolen; //Stolen
    private int ItemsHeld; //Number of target items the thief is holding
    private float doorOpenDelay; //Time the thief must stand still as the door opens
    private DoorControl doorInteractingwith; //References the DoorControl script that the thief is interacting with
    private bool DoorInteraction; //Marks that the thief is interacting with the door
    // Start is called before the first frame update
    void Start()
    {
        DoorInteraction = false;
        Agent = GetComponent<NavMeshAgent>();
        ObjectStolen = false;
        timeRemainingToSteal = timeToSteal;
        StartCoroutine(EscapeTimer()); //Starts the Escape Timer
    }

    // Update is called once per frame
    void Update()
    {
        //print("Thief speed is " + Agent.speed);

        //Sneak
        if (currBehavior == BehaviorStates.Sneak)
        {
            SneakBehavior();
        }
        //Escape
        else if (currBehavior == BehaviorStates.Escape)
        {
            EscapeBehavior();
        }
        //Evade
        else if (currBehavior == BehaviorStates.Evade)
        {
            EvadeBehavior();
        }
        else if(currBehavior == BehaviorStates.SkillCheck)
        {
            //Stealing, hacking, lockpicking...
        }
    }

    private void SneakBehavior()
    {
        //Checks if the Thief is close enough to steal the target object
        if (Vector3.Distance(transform.position, Target.transform.position) < StealRange)
        {
            Agent.SetDestination(transform.position);
            StealAction();
        }
        else
        {
            if (DoorInteraction == false) //If thief is interacting with door, SetDestination does not reset
            {
                Agent.SetDestination(Target.transform.position);
            }
        }
    }

    //Sneak out of the building
    private void EscapeBehavior()
    {
        if (DoorInteraction == false) //If thief is interacting with door, SetDestination does not reset
        {
            Agent.SetDestination(SpawnPoint.position);
        }
        if (Vector3.Distance(transform.position, SpawnPoint.position) < 0.5f)
        {
            if (ObjectStolen == false) //Checks to see if the thief managed to steal its last object before readding it back to the target list
            {
                ThiefSpawnSystem.Instance.TargetObjects.Add(Target);
            }
            ThiefSpawnSystem.Instance.ItemsLeft -= ItemsHeld; //Adjusts how many items are left after the thief stole some.
            Debug.Log("Thief Escaped");
            Destroy(gameObject);
        }
    }

    //Run from guard out of the building
    private void EvadeBehavior()
    {
        print("Evading");
        if (DoorInteraction == false) //If thief is interacting with door, SetDestination does not reset
        {
            Agent.SetDestination(SpawnPoint.position);
        }
        if (Vector3.Distance(transform.position, SpawnPoint.position) < 0.5f)
        {
            if (ObjectStolen == false) //Checks to see if the thief managed to steal its last object before readding it back to the target list
            {
                ThiefSpawnSystem.Instance.TargetObjects.Add(Target);
            }
            ThiefSpawnSystem.Instance.ItemsLeft -= ItemsHeld; //Adjusts how many items are left after the thief stole some.

            //Remove this thief from the guards that are chasing it
            foreach (GuardPathfinding guard in FindObjectsOfType<GuardPathfinding>())
            {
                guard.thievesSpotted.Remove(gameObject);
            }

            Destroy(gameObject);
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

    public void SeenByGuard()
    {
        if (currBehavior != BehaviorStates.Evade)
        {
            print("Detected");

            currBehavior = BehaviorStates.Evade;
            FindClosestEscapeRoute();
            Agent.speed *= EvadeSpeedMod;
        }
    }

    public void CaughtByGuard()
    {

        print("Captured");

        if (ObjectStolen == false) //Checks to see if the thief managed to steal its last object before readding it back to the target list
        {
            ThiefSpawnSystem.Instance.TargetObjects.Add(Target);
        }
        print("Deleted");
        Destroy(gameObject);
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
            timeRemainingToSteal = timeToSteal;
            Destroy(Target);
            ItemsHeld += 1; //Adds one item to itemsheld
            if (TimeBeforeEscape <= 0 || ThiefSpawnSystem.Instance.TargetObjects.Count < 1) //Checks to if the Escape Timer is over and if there are any target objects left. If either are true, the thief begins the escape phase
            {
                ObjectStolen = true;
                //NEED TO CHECK IF IN BUILDING LONG ENOUGH FIRST

                currBehavior = BehaviorStates.Escape;
            }
            else
            {
                int NextTarget;
                NextTarget = Random.Range(0, ThiefSpawnSystem.Instance.TargetObjects.Count - 1);
                Target = ThiefSpawnSystem.Instance.TargetObjects[NextTarget];
                ThiefSpawnSystem.Instance.TargetObjects.Remove(ThiefSpawnSystem.Instance.TargetObjects[NextTarget]);
            }
            
            print("ObjectStolen");

            
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

    //Door Interactions

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Door" && other.GetComponent<DoorControl>().IsClosed) //Checks if thief enters a closed door's collider
        {
            DoorInteraction = true; //Marks that the thief is interacting with the door

            doorInteractingwith = other.GetComponent<DoorControl>(); //Creates a reusable reference for the door's script

            Vector3 waitPosition = doorInteractingwith.GetWaitPosition(transform.position); //Marks the position the thief is supposed to wait at as the door opens

            if(currBehavior == BehaviorStates.Evade)
            {
                doorOpenDelay = other.GetComponent<DoorControl>().chaseOpenDuration; //Saves a reference to how long the door animation lasts

            }
            else
            {
                doorOpenDelay = other.GetComponent<DoorControl>().openAnimationDuration; //Saves a reference to how long the door animation lasts

            }

            Agent.SetDestination(waitPosition); //Causes thief to go towards the waitPosition

           
            StartCoroutine(OpenDelayCoroutine());
            print("Thief Opens Door");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Door" && other.GetComponent<DoorControl>().IsOpened)
        {
            
            Agent.isStopped = true;

            doorInteractingwith = other.GetComponent<DoorControl>();

            if (currBehavior == BehaviorStates.Evade)
            {
                doorOpenDelay = other.GetComponent<DoorControl>().chaseCloseDuration; //Saves a reference to how long the door animation lasts

            }
            else
            {
                doorOpenDelay = other.GetComponent<DoorControl>().closeAnimationDuration; //Saves a reference to how long the door animation lasts

            }
            
            
            StartCoroutine(CloseDelayCoroutine());
            print("Thief Closes Door");
        }
    }

    private IEnumerator OpenDelayCoroutine()
    {
        while (Vector3.Distance(Agent.destination, transform.position) > Agent.stoppingDistance + 0.5f)
        {
            print("Do Nothing");
            yield return null;
        }

        if (currBehavior == BehaviorStates.Evade)
        {
            doorInteractingwith.GetComponent<DoorControl>().ChaseOpenDoor();
        }
        else
        {
            doorInteractingwith.GetComponent<DoorControl>().OpenDoor();
        }

        yield return new WaitForSeconds(doorOpenDelay);

        if(currBehavior == BehaviorStates.Sneak)
        {
            Agent.SetDestination(Target.transform.position);
            DoorInteraction = false;
        }
        else
        {
            Agent.SetDestination(SpawnPoint.position);
            DoorInteraction = false;
        }
        
        doorOpenDelay = 0;
    }
    private IEnumerator CloseDelayCoroutine()
    {
        if (currBehavior == BehaviorStates.Evade)
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
}
