using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonPattern<GameManager>
{
    public GameObject sceneViewMask; //allows viewing of hidden objects (like thieves) in the editor or setup phase
    public GameObject planningCanvas;
    public GameObject nightCanvas;

    [HideInInspector] public int currentLevel = 1;
    [HideInInspector] public bool nightWatchPhase = false;

    // Start is called before the first frame update
    void Start()
    {
        sceneViewMask.SetActive(false);

        HUDController.Instance.SetAvailableStartingButtons(currentLevel);
        BeginPlanningPhase();
    }

    //Call on Level Start, allow placement of security measures
    public void BeginPlanningPhase()
    {
        planningCanvas.SetActive(true);
        nightCanvas.SetActive(false);
    }

    //Call when player presses button, hides the placement UI and begins spawning thieves
    public void BeginNightPhase()
    {
        nightWatchPhase = true;
        planningCanvas.SetActive(false);
        SecuritySelection.Instance.CloseSelection();
        SecurityPlacement.Instance.ExitPlacementMode();
        ThiefSpawnSystem.Instance.BeginSpawnCycle();
        nightCanvas.SetActive(true);
    }

    //Call once all thieves have been spawned and the last thief is caught or escapes
    public void EndLevel()
    {
        Time.timeScale = 0;
        NightHUDController.Instance.SetGameEndStats();
    }
}
