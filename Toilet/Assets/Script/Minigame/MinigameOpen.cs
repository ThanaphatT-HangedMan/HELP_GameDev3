using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameOpen : MonoBehaviour
{
    [Header("Open key")]
    [SerializeField] Transform progressBar;
    public KeyCode pauseKey = KeyCode.Tab;
    public GameObject Fishing;
    public bool isdettingactive;

    private float progress = 0f; // ตัวแปรเก็บ progress
    private Vector3 minScale = new Vector3(0, 1, 1); // ขนาดเริ่มต้น
    private Vector3 maxScale = new Vector3(1, 1, 1); // ขนาดสูงสุด

    void Update()
    {
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

        // รีเซ็ต Progress Bar
        ResetProgressBar();

        Debug.Log("Pause - Minigame Opened & Progress Reset");
    }

    public void Resume()
    {
        Fishing.SetActive(false);
        isdettingactive = false;
        this.GetComponent<MouseLook>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;

        // ปรับขนาด Progress Bar เป็น 0.2 ในแกน X
        if (progressBar != null)
        {
            StartCoroutine(SetProgressBarSize(new Vector3(0.2f, progressBar.localScale.y, progressBar.localScale.z)));
        }

        Debug.Log("UnPause - Minigame Closed & Progress Adjusted");
    }

    void ResetProgressBar()
    {
        if (progressBar != null)
        {
            progress = 0f; // รีเซ็ตค่า progress เป็น 0
            progressBar.localScale = minScale; // ตั้งขนาด progressBar ใหม่
        }
    }

    IEnumerator SetProgressBarSize(Vector3 targetSize)
    {
        float duration = 0.2f;
        Vector3 startSize = progressBar.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            progressBar.localScale = Vector3.Lerp(startSize, targetSize, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        progressBar.localScale = targetSize;
    }
}

