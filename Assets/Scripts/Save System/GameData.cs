using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int difficultySelection;
    public int lastScene;
    public bool LevelOne;


    public GameData (LevelManager gameInfo)
    {
        difficultySelection = gameInfo.difficulty;
        lastScene = gameInfo.lastPlayedScene;
        LevelOne = gameInfo.LevelOneCompletion;
    }
}
