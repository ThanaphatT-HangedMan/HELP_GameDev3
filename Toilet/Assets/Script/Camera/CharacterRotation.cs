using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    public Transform cameraTransform; // อ้างอิงกล้อง (Cinemachine)

    void Update()
    {
        if (cameraTransform != null)
        {
            // ให้โมเดลหันหน้าไปในทิศทางที่กล้องมอง
            transform.rotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
        }
    }
}
