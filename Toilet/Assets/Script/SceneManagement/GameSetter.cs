using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetter : MonoBehaviour
{
    [Header("Setting")]
    public float stageMaxAbilityCount;
    public bool defaultSetting;
    public float stageAbilityStart;

    public PlayerScript ps;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            ps.maxAbilityCount = stageMaxAbilityCount;

            if (defaultSetting == true)
            {

                ps.abilityCount = ps.maxAbilityCount;
            }
            else
            {
                ps.abilityCount = stageAbilityStart;
            }

            Destroy(gameObject);
        }
    
}
}
