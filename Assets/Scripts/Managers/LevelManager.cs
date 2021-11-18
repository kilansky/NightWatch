using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int difficulty;
    public bool NoSaveData;

    private void Start()
    {
        if (NoSaveData)
        {
            NoSaveData = false;
            SaveDifficultySelection(0);
        }
        else
        {
            LoadDifficultySelection();
        }
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
    public void SaveDifficultySelection(int diff)
    {
        difficulty = diff;
        SaveSystemScript.SaveGameInfo(this);
    }
    public void LoadDifficultySelection()
    {
        GameData data = SaveSystemScript.LoadGameInfo();
        difficulty = data.difficultySelection;
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
}
