using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform player;     
    public Transform orientation;  
    public Transform cam;          

    [Header("Settings")]
    public float rotationSpeed = 5f;      
    public Vector2 pitchClamp = new Vector2(-30f, 60f);  
    public float distanceFromPlayer = 3f; 

    private float xRotation;  
    private float yRotation; 

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  
        Cursor.visible = false;  
    }

    private void Update()
    {
        HandleCameraRotation();
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

       
        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, pitchClamp.x, pitchClamp.y);

        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);

        Vector3 offset = new Vector3(0, 1.5f, -distanceFromPlayer);
        cam.position = player.position + transform.rotation * offset;

        cam.LookAt(player.position + Vector3.up * 1.5f);
    }
}
