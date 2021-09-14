using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThiefPathfinding : MonoBehaviour
{
    public NavMeshAgent Agent;
    //Object the Thief is currently trying to steal
    public GameObject Target;

    public GameObject LevelControl;
    //Time it takes to steal objects
    public float TBD;
    //Distance from the target object that the thief will begin its steal action
    public float StealRange;
    //The progress of the steal timer
    private float TBDProgress;
    //The phase in which the thief will attempt to steal objects
    private bool SneakPhase;
    //The phase in which the thief will attempt to sneak out of the building
    private bool EscapePhase;

    private bool EvadePhase;
    //The Entry Point the Thief entered the building in
    public Transform SpawnPoint;


    // Start is called before the first frame update
    void Start()
    {
        Target = GameObject.FindGameObjectWithTag("Target");
        LevelControl = GameObject.FindGameObjectWithTag("LevelControl");

        TBDProgress = TBD;
        SneakPhase = true;
        EscapePhase = false;
        
    }

    // Update is called once per frame
    void Update()
    {

        print("Spawn Point position is at " + SpawnPoint.position);
        //Sneak Phase
        if (SneakPhase == true)
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
        //Escape Phase
        if (EscapePhase == true)
        {
            if (Vector3.Distance(transform.position, SpawnPoint.position) < 0.1)
            {
                Destroy(gameObject);
            }
            Agent.SetDestination(SpawnPoint.position);
        }

        if (EvadePhase == true)
        {
            if (Vector3.Distance(transform.position, SpawnPoint.position) < 0.1)
            {
                Destroy(gameObject);
            }
            Agent.SetDestination(SpawnPoint.position);
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GuardDetection")
        {
            print("Detected");
            if (EvadePhase == false)
            {
                SneakPhase = false;
                EscapePhase = false;
                EvadePhase = true;
                FindClosestEscapeRoute();
                Agent.speed = Agent.speed * 1.5f;
            }
            
        }
    }

    //Steal Action
    private void StealAction()
    {
        //Checks to see if the steal timer is over
        if (TBDProgress > 0)
        {
            print("Progress at " + TBDProgress);
            TBDProgress -= Time.deltaTime;
        }
        else
        {
            TBDProgress = TBD;
            Destroy(Target);
            SneakPhase = false;
            EscapePhase = true;
        }
        
    }

    private void FindClosestEscapeRoute()
    {
        for (var i = 0; i < LevelControl.GetComponent<ThiefSpawnSystem>().SpawnWeights.Length; i++)
        {
            if(Vector3.Distance(transform.position, SpawnPoint.position) > Vector3.Distance(transform.position, LevelControl.GetComponent<ThiefSpawnSystem>().Entry_Locations[i].position))
            {
                SpawnPoint = LevelControl.GetComponent<ThiefSpawnSystem>().Entry_Locations[i];
            }
        }
    }
}
