using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bringupseting : MonoBehaviour

{
    public GameObject setting;
    public bool isdettingactive;
    // Start is called before the first frame update
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(isdettingactive==false)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }
    public void Pause()
    {
        setting.SetActive(true);
        isdettingactive = true;
        this.GetComponent<MouseLook>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Pause");
       // Cursor.visible = true;
    }
    public void Resume()
    {
        setting.SetActive(false);
        isdettingactive = false;
        this.GetComponent<MouseLook>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        Debug.Log("UnPause");




    }


    
}
