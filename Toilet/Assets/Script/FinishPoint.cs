using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FinishPoint : MonoBehaviour
{
    [SerializeField] private bool goNextLevel;
    [SerializeField] private string levelName;
    //public GameObject gameOverUI;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            if (goNextLevel)
           {
                SceneController.instance.NextLevel();
            }
            else
            {
                SceneController.instance.LoadScene(levelName);
            }
        }
    }
    
   // private void Gameover()
   // {
   //     gameOverUI.SetActive(true);
   // }

}