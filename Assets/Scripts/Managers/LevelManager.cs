using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelManager : MonoBehaviour
{
    public int difficulty;
    public int lastPlayedScene;
    public bool LevelOneCompletion;
    public GameObject continueWarning;
    public Button SecondLevel;
    private int selectedLevel;
    public List<GameObject> securityObjects = new List<GameObject>();
    public List<Transform> securityPlacements = new List<Transform>();


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
            SceneManager.LoadScene(lastPlayedScene);
        }
        
    }
    public void selectLevel(int level)
    {
        selectedLevel = level;
        print("Select Level " + selectedLevel);
    }
    public void loadSelectedLevel()
    {
        SceneManager.LoadScene(selectedLevel);
    }
    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
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
}
