using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int difficultySelection;
    public int lastScene;
    public int money;
    public bool LevelOne;
    public bool inProgress;


    public GameData (LevelManager gameInfo)
    {
        difficultySelection = gameInfo.difficulty;
        lastScene = gameInfo.lastPlayedScene;
        LevelOne = gameInfo.LevelOneCompletion;
        money = gameInfo.money;
        inProgress = gameInfo.inProgress;
    }
}
