using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform player;       // ตัวละครหลัก
    public Transform orientation;  // ช่วยหมุนตัวละคร
    public Transform cam;          // กล้องหลัก

    [Header("Settings")]
    public float rotationSpeed = 5f;      // ความเร็วในการหมุน
    public Vector2 pitchClamp = new Vector2(-30f, 60f);  // จำกัดการเงยและก้ม
    public float distanceFromPlayer = 3f; // ระยะห่างกล้องจากตัวละคร

    private float xRotation;  // หมุนแกน X (ขึ้น-ลง)
    private float yRotation;  // หมุนแกน Y (ซ้าย-ขวา)

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // ล็อคเมาส์ตรงกลาง
        Cursor.visible = false;  // ซ่อนเมาส์
    }

    private void Update()
    {
        HandleCameraRotation();
    }

    void HandleCameraRotation()
    {
        // รับ Input เมาส์
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // หมุนแกน Y (ซ้าย-ขวา)
        yRotation += mouseX;

        // หมุนแกน X (ขึ้น-ลง) และ Clamp มุมกล้อง
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, pitchClamp.x, pitchClamp.y);

        // หมุน Orientation (ควบคุมทิศทางตัวละคร)
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        // หมุน CamHolder ในแกน X สำหรับมุมกล้อง
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

        // ตั้งตำแหน่งกล้องให้อยู่หลังตัวละคร
        Vector3 offset = new Vector3(0, 1.5f, -distanceFromPlayer);
        cam.position = player.position + transform.rotation * offset;

        // ให้กล้องมองไปที่ตัวละคร
        cam.LookAt(player.position + Vector3.up * 1.5f);
    }
}
