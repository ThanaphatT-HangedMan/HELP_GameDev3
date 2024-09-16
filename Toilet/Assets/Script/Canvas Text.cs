using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI abilityLeft, maxAbility, Timertext;
    [SerializeField] float RemainingTime;
    PlayerScript ps;
    void Start()
    {
        ps = GetComponent<PlayerScript>();
        SetStats();  
    }

    private void Update()
    {
        SetStats();
        if (RemainingTime > 0)
        {
            RemainingTime -= Time.deltaTime;
        }
        else if (RemainingTime < 0)
        {
            RemainingTime = 0;
            Timertext.color = Color.red;
        }
        Timertext.text = RemainingTime.ToString();

    }

    void SetStats()
    {
        abilityLeft.text = ps.abilityCount.ToString();
        maxAbility.text = ps.maxAbilityCount.ToString();
    }
}
