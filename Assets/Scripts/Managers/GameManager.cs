using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonPattern<GameManager>
{
    public GameObject sceneViewMask; //allows viewing of hidden objects (like thieves) in the editor or setup phase
    public GameObject planningCanvas;
    //public GameObject nightCanvas;

    [HideInInspector] public bool nightWatchPhase = false;

    // Start is called before the first frame update
    void Start()
    {
        sceneViewMask.SetActive(false);
        BeginPlanningPhase();
    }

    //Call on Level Start, allow placement of security measures
    public void BeginPlanningPhase()
    {
        planningCanvas.SetActive(true);
        //nightCanvas.SetActive(false);
    }

    //Call when player presses button, hides the placement UI and begins spawning thieves
    public void BeginNightPhase()
    {
        nightWatchPhase = true;
        planningCanvas.SetActive(false);
        ThiefSpawnSystem.Instance.BeginSpawnCycle();
        //nightCanvas.SetActive(true);
    }
}
