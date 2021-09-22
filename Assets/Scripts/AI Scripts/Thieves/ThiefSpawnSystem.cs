using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefSpawnSystem : SingletonPattern<ThiefSpawnSystem>
{
    //The minimum amount of time it takes for a thief to spawn
    public float BaseSpawnTimer;
    //The longest possible time that the BaseSpawnTimer can be increased by
    public float Timer_Mod;
    //The Weight that each possible spawnpoint has
    public int[] SpawnWeights;
    //The transform values for each spawn point
    public Transform[] Entry_Locations;
    //The thief prefab
    public GameObject ThiefPrefab;

    public int numThievesToSpawn;

    public int ItemsLeft; //Number of target items left before game over

    public List<GameObject> TargetObjects = new List<GameObject>(); //List of Target items


    //Selected Spawnpoint
    private int Position;
    //Random Number Generated
    private int Chance;
    //The combination of all weighted spawnpoints that serve as the highest possible value for the random number generator
    private int TotalChance;
    //The time left until the thief spawns
    private float Timer;

    private int thievesSpawned;

    private int TargetItemAssigned;

    // Start is called before the first frame update
    private void Start()
    {
        thievesSpawned = 0;

        //Generates the TotalChance variable
        for (var i = 0; i < SpawnWeights.Length; i++)
        {
            TotalChance += SpawnWeights[i];
        }

        ItemsLeft = TargetObjects.Count; //Records how many items there are in the map

        //Sets up the first timer
        Timer = BaseSpawnTimer + Random.Range(0, Timer_Mod);
    }

    private void Update()
    {
        //NOTE: This is a placeholder meant to test a basic gameover state.
        if (ItemsLeft <= 0)
        {
            print("GAME OVER");
        }
    }


    public void BeginSpawnCycle()
    {
        //Start Timer to spawn the first thief
        StartCoroutine(SpawnTimer());
    }

    //Count Down To Next Thief Spawn function
    private IEnumerator SpawnTimer()
    {
        while (Timer > 0)
        {
            Timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (TargetObjects.Count >= 1) //Checks to see if there are any target items left before spawning in a new thief
        {
            SpawnSequence();
        }
        
        Timer = BaseSpawnTimer + Random.Range(0, Timer_Mod);
        thievesSpawned++;
        
        //If the total number of thieves to spawn has not been reached, restart this coroutine
        if(thievesSpawned < numThievesToSpawn)
            StartCoroutine(SpawnTimer());
    }

    //Thief Spawn function
    private void SpawnSequence()
    {       
        Chance = Random.Range(1, TotalChance);
        //print("Number Generated = " + Chance);
        for (var i = 0; i < SpawnWeights.Length; i++)
        {
            Chance -= SpawnWeights[i];
            if (Chance <= 0)
            {
                Position = i;
                //print("Spawn at point " + Position);
                break;
            }
        }
        TargetItemAssigned = Random.Range(0, TargetObjects.Count - 1); //Creates a randomly generated number used to assign the thief its target object

        GameObject obj = Instantiate(ThiefPrefab, Entry_Locations[Position].position, Quaternion.identity) as GameObject; //Spawns thief
        
        obj.GetComponent<ThiefPathfinding>().Target = TargetObjects[TargetItemAssigned]; //Assigns thief a target object to chase
        
        //Saves Entry Point location
        obj.GetComponent<ThiefPathfinding>().SpawnPoint = Entry_Locations[Position];

        TargetObjects.Remove(TargetObjects[TargetItemAssigned]); //Removes the assigned target object from the list
    }
}
