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

    //Reloads the current scene
    public void RestartButton()
    {
        Scene currScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currScene.buildIndex);
    }

    //Loads the main menu scene
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    //Quits the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
