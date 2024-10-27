using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasText : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI abilityLeft, maxAbility, Timertext;


    [Header("MedalCheck")] 
    [SerializeField] float BronzeTime;
    [SerializeField] GameObject BronzeImage;

    [SerializeField] float SilverTime;
    [SerializeField] GameObject SilverImage;

    [SerializeField] float GoldenTime;
    [SerializeField] GameObject GoldenImage;

    [SerializeField] GameObject PooMedal;

    [SerializeField] bool SecretCheck;
    [SerializeField] GameObject SecretMedal;

    [Header("Face Check")]
    [SerializeField] GameObject LowFace;
    [SerializeField] GameObject MidFace;
    [SerializeField] GameObject HighFace;
    [SerializeField] GameObject PooFace;

    [Header("Time")]
    [SerializeField] float RemainingTime;
    [SerializeField] float MaxTime;
    public TextMeshProUGUI FinalTime;
    [SerializeField] Image bar;

    PlayerScript ps;

    void Start()
    {
        ps = GetComponent<PlayerScript>();
        SetStats();  
        bar.fillAmount = 1;
        BronzeImage.SetActive(false);
        SilverImage.SetActive(false);
        GoldenImage.SetActive(false);
        PooMedal.SetActive(false);
        PooFace.SetActive(false);
        LowFace.SetActive(false);
        MidFace.SetActive(false);
        HighFace.SetActive(false);
        SecretMedal.SetActive(false);
    }

    private void Update()
    {
        PlayerCheck();
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
            PooMedal.SetActive(true);
        }


    }
    void SetStats()
    {
        abilityLeft.text = ps.abilityCount.ToString();
        maxAbility.text = ps.maxAbilityCount.ToString();
        Timertext.text = RemainingTime.ToString();
        FinalTime.text = Timertext.text;
    }

    public void MedalCheck()
    {
        if (RemainingTime > GoldenTime)
        {
            GoldenImage.SetActive(true);
        }
        
        if (RemainingTime > SilverTime)
        {
            SilverImage.SetActive(true);
        }
        
        if (RemainingTime > BronzeTime)
        {
            BronzeImage.SetActive(true);
        }

        if (ps.Secret == true)
        {
            SecretMedal.SetActive(true);
        }


    }

    public void PlayerCheck()
    {
        if (RemainingTime <= 0)
        {
            LowFace.SetActive(false);
            MidFace.SetActive(false);
            HighFace.SetActive(false);
            PooFace.SetActive(true);
        }
        else if (RemainingTime < MaxTime * 0.25f)
        {
            LowFace.SetActive(false);
            MidFace.SetActive(false);
            HighFace.SetActive(true);
            PooFace.SetActive(false);
        }
        else if (RemainingTime < MaxTime * 0.5f)
        {
            LowFace.SetActive(false);
            MidFace.SetActive(true);
            HighFace.SetActive(false);
            PooFace.SetActive(false);
        }
        else if (RemainingTime < MaxTime)
        {
            LowFace.SetActive(true);
            MidFace.SetActive(false);
            HighFace.SetActive(false);
            PooFace.SetActive(false);
        }

    }



}
