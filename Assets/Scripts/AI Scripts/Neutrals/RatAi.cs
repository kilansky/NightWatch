using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RatAi : MonoBehaviour
{
    public RatSpawner spawner;
    private bool staying;
    private Transform currentTarget;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        staying = true;
        agent = GetComponent<NavMeshAgent>();
        currentTarget = transform;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, currentTarget.position) <= 1)
        {
            currentTarget = spawner.spawnPoints[Random.Range(0, spawner.spawnPoints.Length - 1)];
            agent.SetDestination(currentTarget.position);
        }
    }
}
