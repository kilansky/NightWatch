using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelManager : MonoBehaviour
{
    public GameObject continueWarning;
    public Button SecondLevel;
    public Image blackOverlay;
    public float fadeToBlackTime = 2f;

    [HideInInspector] public int difficulty;
    [HideInInspector] public int lastPlayedScene;
    [HideInInspector] public bool LevelOneCompletion;

    private int selectedLevel;

    private void Start()
    {
        selectedLevel = 1;
        LoadDifficultySelection();
        if (!SecondLevel)
        {
            if (LevelOneCompletion)
            {
                SecondLevel.interactable = true;
            }
        }
    }

    public void LoadLastLevel()
    {
        if (lastPlayedScene < 1)
        {
            continueWarning.SetActive(true);
        }
        else
        {
            LoadLevel(lastPlayedScene);
        }     
    }

    public void selectLevel(int level)
    {
        selectedLevel = level;
        print("Select Level " + selectedLevel);
    }

    public void loadSelectedLevel()
    {
        LoadLevel(selectedLevel);
    }

    public void LoadLevel(int level)
    {
        StartCoroutine(FadeToBlack(level));
    }

    public void QuitGame(int curr)
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            lastPlayedScene = SceneManager.GetActiveScene().buildIndex + curr;
            SaveSystemScript.SaveGameInfo(this);
            print("Last Played Scene");
        }
        print("Scene build " + SceneManager.GetActiveScene().buildIndex);
        
        Application.Quit();
    }

    public void SaveDifficultySelection(int diff)
    {
        difficulty = diff;
        if(SceneManager.GetActiveScene().buildIndex > 0)
        {
            lastPlayedScene = SceneManager.GetActiveScene().buildIndex;
        }
        SaveSystemScript.SaveGameInfo(this);
    }

    public void LoadDifficultySelection()
    {
        GameData data = SaveSystemScript.LoadGameInfo();
        if (data != null)
        {
            difficulty = data.difficultySelection;
            lastPlayedScene = data.lastScene;
            print("Last Level = " + lastPlayedScene);
            if (difficulty == 0)
            {
                print("Difficulty selected is easy");
            }
            else if (difficulty == 1)
            {
                print("Difficulty selected is normal");
            }
            else if (difficulty == 2)
            {
                print("Difficulty selected is hard");
            }
        }
        else
        {
            SaveDifficultySelection(0);
        }
    }

    private IEnumerator FadeToBlack(int levelToLoad)
    {
        blackOverlay.color = new Color(0, 0, 0, 0);
        float timeElaped = 0;

        while (timeElaped < fadeToBlackTime)
        {
            float alpha = Mathf.Lerp(0, 1, timeElaped/fadeToBlackTime);
            blackOverlay.color = new Color(0, 0, 0, alpha);
            timeElaped += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }

        blackOverlay.color = new Color(0, 0, 0, 1);

        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(levelToLoad);
    }
}
