using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonPattern<GameManager>
{
    public GameObject sceneViewMask; //allows viewing of hidden objects (like thieves) in the editor or setup phase
    public Light[] directionalLights;
    public float planningPhaseBrightness = 0.75f;
    public float nightPhaseBrightness = 0.25f;
    public float timeToChangeBrightness = 2f;

    [HideInInspector] public int currentLevel = 1;
    [HideInInspector] public bool nightWatchPhase = false;

    private GameObject planningCanvas;
    private GameObject nightCanvas;
    private int currLevel;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        PlayerInputs.Instance.canPause = true;
        sceneViewMask.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;

        planningCanvas = HUDController.Instance.gameObject;
        nightCanvas = NightHUDController.Instance.gameObject;

        currentLevel = SceneManager.GetActiveScene().buildIndex;
        HUDController.Instance.SetAvailableStartingButtons(currentLevel);

        //Adjust brightness value of lights based on the number of lights
        int lightCount = 0;
        foreach (Light light in directionalLights)
            lightCount++;

        planningPhaseBrightness /= lightCount;
        nightPhaseBrightness /= lightCount;

        BeginPlanningPhase();
    }

    //Call on Level Start, allow placement of security measures
    public void BeginPlanningPhase()
    {
        AudioManager.Instance.PlayDayTrack();

        foreach (Light light in directionalLights)
            light.intensity = planningPhaseBrightness;

        planningCanvas.SetActive(true);
        nightCanvas.SetActive(false);
    }

    //Call when player presses button, hides the placement UI and begins spawning thieves
    public void BeginNightPhase()
    {
        AudioManager.Instance.PlayNightTrack();

        nightWatchPhase = true;
        StartCoroutine(ChangeTimeOfDay(planningPhaseBrightness, nightPhaseBrightness));

        planningCanvas.SetActive(false);
        SecuritySelection.Instance.CloseSelection();
        SecurityPlacement.Instance.ExitPlacementMode();

        ThiefSpawnSystem.Instance.BeginSpawnCycle();
        nightCanvas.SetActive(true);
        NightHUDController.Instance.StartNight();

    }

    public IEnumerator ChangeTimeOfDay(float startBrightness, float endBrightness)
    {
        float timeElapsed = 0f;
        while(timeElapsed < timeToChangeBrightness)
        {
            float newIntensity = Mathf.Lerp(startBrightness, endBrightness, timeElapsed / timeToChangeBrightness);

            foreach (Light light in directionalLights)
                light.intensity = newIntensity;

            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        foreach (Light light in directionalLights)
            light.intensity = endBrightness;
    }

    //Call once all thieves have been spawned and the last thief is caught or escapes
    public void EndLevel()
    {
        Time.timeScale = 0;
        NightHUDController.Instance.SetGameEndStats();
    }
}
