using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FinishPoint : MonoBehaviour
{
    [Header ("Reference")]
    public GameObject gameOverUI;
    public PlayerCamera pc;
    public PlayerScript ps;
    public CanvasText ct;

    [SerializeField] TextMeshProUGUI finalText;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            pc.GetComponent<PlayerCamera>().enabled = false;
            ps.GetComponent<PlayerScript>().enabled = false;
            ct.GetComponent<CanvasText>().enabled = false;
            GameOver();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void GameOver()
    {
        finalText = ct.FinalTime;
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