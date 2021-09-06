using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefSpawnSystem : MonoBehaviour
{
    public float BaseSpawnTimer;
    public float Timer_Mod;

    public int[] SpawnWeights;

    public Transform[] Entry_Locations;

    public GameObject Thief;

    private int Chance;
    private int Position;
    private int TotalChance;
    
    private float Timer;

    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < SpawnWeights.Length; i++)
        {
            TotalChance += SpawnWeights[i];
        }
        SpawnSequence();
        Timer = BaseSpawnTimer + Random.Range(0, Timer_Mod);
    }

    // Update is called once per frame
    void Update()
    {
        SpawnTimer();
    }

    private void SpawnTimer()
    {
        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
        }
        else
        {
            SpawnSequence();
            Timer = BaseSpawnTimer + Random.Range(0, Timer_Mod);
        }

    }

    private void SpawnSequence()
    {
        
        Chance = Random.Range(1, TotalChance);
        print("Number Generated = " + Chance);
        for (var i = 0; i < SpawnWeights.Length; i++)
        {
            Chance -= SpawnWeights[i];
            if (Chance <= 0)
            {
                Position = i;
                print("Spawn at point " + Position);
                break;
            }

        }
        Instantiate(Thief, Entry_Locations[Position].position, Quaternion.identity);
    }
}
