using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void PlayCutScene()
    {
        SceneManager.LoadScene("Cutscene");
    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player has Quit The Game");
    }
}
