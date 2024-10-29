using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    public Slider slider;
    public float mousesentivity = 100f;
    
    float xRotation = 0f;
    float yRotation = 0f;

    public Transform orientation;
    public Transform camHolder;

    void Start()
    {
        mousesentivity = PlayerPrefs.GetFloat("currentSentivity", 100);
        slider.value = mousesentivity / 10;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        PlayerPrefs.SetFloat("currentSentivity", mousesentivity);

        // แก้ไขชื่อแกนจาก "MouaseX" และ "MouaseY" เป็น "Mouse X" และ "Mouse Y"
        float mouseX = Input.GetAxis("Mouse X") * mousesentivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mousesentivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void AdjustSpeed(float newSpeed)
    {
        mousesentivity = newSpeed * 10;
    }
}
