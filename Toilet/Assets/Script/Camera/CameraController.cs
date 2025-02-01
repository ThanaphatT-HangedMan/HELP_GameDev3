using UnityEngine;

using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera thirdPersonCam;
    public float rotationSpeed = 2f;

    private float mouseX, mouseY;

    void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -30f, 60f);

        thirdPersonCam.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }
}
