using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : SingletonPattern<LevelManager>
{
    public GameObject continueWarning;
    public Button SecondLevel;
    public Image blackOverlay;
    public float fadeToBlackTime = 2f;

    [HideInInspector] public int difficulty;
    [HideInInspector] public int money;
    [HideInInspector] public int lastPlayedScene;
    [HideInInspector] public bool LevelOneCompletion;
    [HideInInspector] public bool inProgress;

    private int selectedLevel;

    private void Start()
    {
        selectedLevel = 1;
        LoadGameData();
        if(SceneManager.GetActiveScene().buildIndex > 1)
        {
            SaveGameData(difficulty);
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

    public void AddMoneyForCurrentSecurity()
    {
        foreach (GuardController guard in FindObjectsOfType<GuardController>())
            MoneyManager.Instance.AddMoney(500);

        foreach (HackedSecurityScript security in FindObjectsOfType<HackedSecurityScript>())
            MoneyManager.Instance.AddMoney(security.GetComponent<SecurityMeasure>().cost);
    }

    public void LoadNextLevel()
    {
        AddMoneyForCurrentSecurity();

        if (MoneyManager.Instance.money < 500)
            MoneyManager.Instance.ResetMoney();
        SaveGameData(difficulty);
        int currLevelIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(FadeToBlack(currLevelIndex + 1));
    }

    public void LoadMenu()
    {
        AddMoneyForCurrentSecurity();
        lastPlayedScene = SceneManager.GetActiveScene().buildIndex;
        if (MoneyManager.Instance.money < 500)
            MoneyManager.Instance.ResetMoney();
        SaveGameData(difficulty);
        int currLevelIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(FadeToBlack(0));
    }

    public void QuitGame(int curr)
    {
        AddMoneyForCurrentSecurity();
        if(SceneManager.GetActiveScene().buildIndex != 3)
        {
            lastPlayedScene = SceneManager.GetActiveScene().buildIndex + 1;
            print("New last level is " + lastPlayedScene);
        }
        
        if (MoneyManager.Instance.money < 500)
            MoneyManager.Instance.ResetMoney();
        SaveGameData(difficulty);
        int currLevelIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(FadeToBlack(0));
    }

    public void SaveGameData(int diff)
    {
        difficulty = diff;
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            money = MoneyManager.Instance.money;
        }
        SaveSystemScript.SaveGameInfo(this);
    }

    public void LoadGameData()
    {
        GameData data = SaveSystemScript.LoadGameInfo();
        if (data != null)
        {
            difficulty = data.difficultySelection;
            lastPlayedScene = data.lastScene;
            if (SceneManager.GetActiveScene().buildIndex > 0)
            {
                if (SceneManager.GetActiveScene().buildIndex > 1)
                {
                    MoneyManager.Instance.AddMoney(data.money);
                }
                else
                {
                    MoneyManager.Instance.money = MoneyManager.Instance.startingMoney;
                }
            }

            print("Player has " + MoneyManager.Instance.money + " dollars");
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
            SaveGameData(0);
        }
    }

    private IEnumerator FadeToBlack(int levelToLoad)
    {
        if(SceneManager.GetActiveScene().buildIndex  > 0)
            blackOverlay = NightHUDController.Instance.blackOverlay;

        blackOverlay.gameObject.SetActive(true);

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
