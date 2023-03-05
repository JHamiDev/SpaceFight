using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static bool isEndlessMode;
    public static int selectedLevel;

    void Start()
    {
        PlayerMovement.playerDeathSoundHasPlayed = false;
    }

    public void PlayGame()
    {
        isEndlessMode = true;
        selectedLevel = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadLevel(int level)
    {
        isEndlessMode = false;
        selectedLevel = level;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
