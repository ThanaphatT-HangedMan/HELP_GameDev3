using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FinishPoint : MonoBehaviour
{
    public GameObject gameOverUI;
    public PlayerCamera pc;
    public PlayerScript ps;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            pc.GetComponent<PlayerCamera>().enabled = false;
            ps.GetComponent<PlayerScript>().enabled = false;
            GameOver();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void GameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void SelectLevel(string String)
    {
        SceneController.instance.LoadScene(String);
    }

    public void NextLevel()
    {
        SceneController.instance.NextLevel();
    }

    public void RestartLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }



}
