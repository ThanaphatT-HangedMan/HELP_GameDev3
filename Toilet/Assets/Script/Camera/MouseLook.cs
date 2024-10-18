using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    public Slider slider;
    public float mousesentivity = 100f;
    public Transform playerbody;
    float xRotation = 0f;

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

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerbody.Rotate(Vector3.up * mouseX);
    }

    public void AdjustSpeed(float newSpeed)
    {
        mousesentivity = newSpeed * 10;
    }
}
