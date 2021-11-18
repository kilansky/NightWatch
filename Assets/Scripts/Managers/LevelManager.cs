using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public int difficulty;
    public int lastPlayedScene;
    public GameObject continueWarning;


    private void Start()
    {
        LoadDifficultySelection();
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
    public void SaveCurrentInformation(int curr)
    {
        
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
