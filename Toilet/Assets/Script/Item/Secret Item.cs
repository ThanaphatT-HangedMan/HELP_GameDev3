using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SecretItem : MonoBehaviour
{
    public PlayerScript ps;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            ps.Secret = true;
            Destroy(gameObject);
        }
    }

}