using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI abilityLeft, maxAbility, Timertext;
    [SerializeField] float RemainingTime;
    [SerializeField] float MaxTime;
    PlayerScript ps;
    public TextMeshProUGUI FinalTime;
    [SerializeField] Image bar;

    void Start()
    {
        ps = GetComponent<PlayerScript>();
        SetStats();  
        bar.fillAmount = 1;
    }

    private void Update()
    {
        SetStats();
        if (RemainingTime > 0)
        {
            RemainingTime  -= Time.deltaTime;
            float fillAmount = RemainingTime / MaxTime;
            bar.fillAmount = Mathf.Clamp(fillAmount, 0, 1);
        }
        else if (RemainingTime < 0)
        {
            RemainingTime = 0;
            Timertext.color = Color.red;
            bar.fillAmount = 0;
        }


    }

    void SetStats()
    {
        abilityLeft.text = ps.abilityCount.ToString();
        maxAbility.text = ps.maxAbilityCount.ToString();
        Timertext.text = RemainingTime.ToString();
        FinalTime.text = Timertext.text;
    }
}
