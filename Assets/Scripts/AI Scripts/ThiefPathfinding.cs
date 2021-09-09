using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThiefPathfinding : MonoBehaviour
{

    public NavMeshAgent Agent;
    public GameObject Target;
    public float TBD;
    private float TBDProgress;
    private bool Busy;


    // Start is called before the first frame update
    void Start()
    {
        TBDProgress = TBD;

        Target = GameObject.FindGameObjectWithTag("Target");
    }

    // Update is called once per frame
    void Update()
    {
        if (Busy == false)
        {
            Agent.SetDestination(Target.transform.position);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == Target)
        {
            print("steal");
            StealAction();

        }
    }

    private void StealAction()
    {
        if (TBDProgress > 0)
        {
            print("Progress");
            TBDProgress -= Time.deltaTime;
        }
        else
        {
            TBDProgress = TBD;
            Destroy(Target);
        }
        
    }
}
