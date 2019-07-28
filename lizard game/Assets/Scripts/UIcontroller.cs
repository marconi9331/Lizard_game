using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIcontroller : MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("First level");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void RollCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}