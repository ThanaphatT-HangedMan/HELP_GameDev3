using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameOpen : MonoBehaviour
{
    [Header("Open key")]
    public KeyCode pauseKey = KeyCode.Tab; // ปุ่มที่สามารถกำหนดเองได้ใน Inspector

    public GameObject Fishing;
    public bool isdettingactive;

    void Update()
    {
        // ตรวจสอบว่าผู้เล่นกดปุ่มที่กำหนดไว้
        if (Input.GetKeyDown(pauseKey))
        {
            if (!isdettingactive)
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
        Fishing.SetActive(true);
        isdettingactive = true;
        this.GetComponent<MouseLook>().enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Pause");
    }

    public void Resume()
    {
        Fishing.SetActive(false);
        isdettingactive = false;
        this.GetComponent<MouseLook>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("UnPause");
    }
}
