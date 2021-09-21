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

    [HideInInspector] public Transform SpawnPoint;    //The Entry Point the Thief entered the building in

    private NavMeshAgent Agent;
    private GameObject Target;    //Object the Thief is currently trying to steal
    private float timeRemainingToSteal;    //The progress of the steal timer

    // Start is called before the first frame update
    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Target = GameObject.FindGameObjectWithTag("Target");

        timeRemainingToSteal = timeToSteal; 
    }

    // Update is called once per frame
    void Update()
    {
        //print(currBehavior);

        //Sneak
        if (currBehavior == BehaviorStates.Sneak)
        {
            //Checks if the Thief is close enough to steal the target object
            if (Vector3.Distance(transform.position, Target.transform.position) < StealRange)
            {
                Agent.SetDestination(transform.position);
                StealAction();
            }
            else
            {
                Agent.SetDestination(Target.transform.position);
            }
        }
        //Escape
        else if (currBehavior == BehaviorStates.Escape)
        {
            Agent.SetDestination(SpawnPoint.position);

            if (Vector3.Distance(transform.position, SpawnPoint.position) < 0.5f)
            {
                Destroy(gameObject);
            }
        }
        //Evade
        else if (currBehavior == BehaviorStates.Evade)
        {
            Agent.SetDestination(SpawnPoint.position);

            if (Vector3.Distance(transform.position, SpawnPoint.position) < 0.5f)
            {
                Destroy(gameObject);
            }
        }
        else if(currBehavior == BehaviorStates.SkillCheck)
        {
            //Stealing, hacking, lockpicking...
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
            //Destroy(Target);

            //NEED TO CHECK IF IN BUILDING LONG ENOUGH FIRST

            currBehavior = BehaviorStates.Escape;
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
}
