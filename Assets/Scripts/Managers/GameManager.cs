using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonPattern<GameManager>
{
    public GameObject sceneViewMask; //allows viewing of hidden objects (like thieves) in the editor or setup phase
    public GameObject planningCanvas;
    public GameObject nightCanvas;
    public Light directionalLight;

    public float planningPhaseBrightness = 0.75f;
    public float nightPhaseBrightness = 0.25f;
    public float timeToChangeBrightness = 2f;

    [HideInInspector] public int currentLevel = 1;
    [HideInInspector] public bool nightWatchPhase = false;
    private int currLevel;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        PlayerInputs.Instance.canPause = true;
        sceneViewMask.SetActive(false);

        currentLevel = SceneManager.GetActiveScene().buildIndex + 1;
        HUDController.Instance.SetAvailableStartingButtons(currentLevel);
        BeginPlanningPhase();
    }

    //Call on Level Start, allow placement of security measures
    public void BeginPlanningPhase()
    {
        directionalLight.intensity = planningPhaseBrightness;
        planningCanvas.SetActive(true);
        nightCanvas.SetActive(false);
    }

    //Call when player presses button, hides the placement UI and begins spawning thieves
    public void BeginNightPhase()
    {
        nightWatchPhase = true;
        StartCoroutine(ChangeTimeOfDay(planningPhaseBrightness, nightPhaseBrightness));

        planningCanvas.SetActive(false);
        SecuritySelection.Instance.CloseSelection();
        SecurityPlacement.Instance.ExitPlacementMode();
        ThiefSpawnSystem.Instance.BeginSpawnCycle();
        nightCanvas.SetActive(true);
    }

    public IEnumerator ChangeTimeOfDay(float startBrightness, float endBrightness)
    {
        float timeElapsed = 0f;
        while(timeElapsed < timeToChangeBrightness)
        {
            directionalLight.intensity = Mathf.Lerp(startBrightness, endBrightness, timeElapsed / timeToChangeBrightness);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        directionalLight.intensity = endBrightness;
    }

    //Call once all thieves have been spawned and the last thief is caught or escapes
    public void EndLevel()
    {
        Time.timeScale = 0;
        NightHUDController.Instance.SetGameEndStats();
    }
}
