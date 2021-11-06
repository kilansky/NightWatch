using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons : MonoBehaviour
{
    //Closes the pause screen
    public void Continue()
    {
        PlayerInputs.Instance.ContinueButtonPressed();
    }

    //Loads the next level in the build index
    public void LoadNextLevel()
    {
        int currLevelIndex = SceneManager.GetActiveScene().buildIndex;
        NightHUDController.Instance.AddMoneyForCurrentSecurity();

        if (MoneyManager.Instance.Money < 500)
            MoneyManager.Instance.ResetMoney();

        PlayerPrefs.SetInt("Money", MoneyManager.Instance.Money);
        //print("Money saved in playerPrefs " + PlayerPrefs.GetInt("Money"));
        SceneManager.LoadScene(currLevelIndex + 1);
    }

    //Reloads the current scene
    public void RestartButton()
    {
        Time.timeScale = 1;

        NightHUDController.Instance.AddMoneyForCurrentSecurity();
        PlayerPrefs.SetInt("Money", MoneyManager.Instance.Money);

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
