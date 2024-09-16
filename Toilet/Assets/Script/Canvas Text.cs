using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI abilityLeft, maxAbility;
    PlayerScript ps;
    void Start()
    {
        ps = GetComponent<PlayerScript>();
        SetStats();  
    }

    private void Update()
    {
        SetStats();
    }

    void SetStats()
    {
        abilityLeft.text = ps.abilityCount.ToString();
        maxAbility.text = ps.maxAbilityCount.ToString();
    }
}
