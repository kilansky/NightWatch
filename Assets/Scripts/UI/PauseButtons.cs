using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons : MonoBehaviour
{
    public GameObject manager;
    //Closes the pause screen
    public void Continue()
    {
        PlayerInputs.Instance.ContinueButtonPressed();
    }

    //Loads the next level in the build index
    public void LoadNextLevel()
    {
        int currLevelIndex = SceneManager.GetActiveScene().buildIndex;
        manager.GetComponent<MoneyManager>().AddMoney(300);
        PlayerPrefs.SetInt("Money", manager.GetComponent<MoneyManager>().money);
        print("Money saved in playerPrefs " + PlayerPrefs.GetInt("Money"));
        SceneManager.LoadScene(currLevelIndex + 1);
    }

    //Reloads the current scene
    public void RestartButton()
    {
        Time.timeScale = 1;
        Scene currScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currScene.buildIndex);
    }

    //Loads the google form web page
    public void GoogleForm()
    {
        Application.OpenURL("https://forms.gle/yTTLFrK9X1HjCLXh9");
    }

    //Quits the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
