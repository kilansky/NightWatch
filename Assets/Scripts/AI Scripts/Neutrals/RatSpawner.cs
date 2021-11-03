using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject ratPrefab;
    public int numberOfRatsSpawned;
    public float baseSpawnRate;
    public float spawnRateMod;
    private bool start;

    private void Start()
    {
        start = true;
    }

    private void Update()
    {
        if (GameManager.Instance.nightWatchPhase && start)
        {
            start = false;
            StartCoroutine(SpawnTimer());
        }
            
    }

    private IEnumerator SpawnTimer()
    {
        float mod = Random.Range(0, spawnRateMod);
        yield return new WaitForSeconds(baseSpawnRate + mod);
        if (numberOfRatsSpawned > 0)
        {
            SpawnSequence();
            numberOfRatsSpawned -= 1;
            StartCoroutine(SpawnTimer());
        }
    }

    private void SpawnSequence()
    {
        GameObject obj = Instantiate(ratPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity) as GameObject;
        obj.GetComponent<RatAi>().spawner = GetComponent<RatSpawner>();
    }
}
