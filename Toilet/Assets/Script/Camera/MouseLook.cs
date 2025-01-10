using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    public Slider slider;
    public float mousesentivity = 100f;

    void Start()
    {
        mousesentivity = PlayerPrefs.GetFloat("currentSentivity", 100);
        slider.value = mousesentivity / 10;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void AdjustSpeed(float newSpeed)
    {
        mousesentivity = newSpeed * 10;
        PlayerPrefs.SetFloat("currentSentivity", mousesentivity);
    }
}
