using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f; // ความเร็วการหมุนกล้อง
    public Transform playerBody; // ตัวละครที่จะหมุนตามกล้อง

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // ล็อกเคอร์เซอร์ไว้กลางหน้าจอ
    }

    void Update()
    {
        // รับค่าการเคลื่อนไหวของเมาส์ในแนวแกน X (ซ้าย-ขวา)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // หมุนตัวละครเฉพาะแกน Y (แนวนอน)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}