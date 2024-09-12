using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetter : MonoBehaviour
{
    [Header("Setting")]
    public float stageMaxAbilityCount;

    public PlayerScript ps;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            ps.maxAbilityCount = stageMaxAbilityCount;
            ps.abilityCount = ps.maxAbilityCount;
            Destroy(gameObject);
        }
    
}
}
