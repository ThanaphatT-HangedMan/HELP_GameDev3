using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasText : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI Timertext;

    [Header("Ability")]
    public float abilityLeft;
    public float maxAbility;
    [SerializeField] GameObject ability1;
    [SerializeField] GameObject ability2;
    [SerializeField] GameObject ability3;


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


        abilityLeft = ps.abilityCount;
        maxAbility = ps.maxAbilityCount;
    }

    private void Update()
    {
        abilityLeft = ps.abilityCount;
        maxAbility = ps.maxAbilityCount;

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
            FindObjectOfType<AudioManager>().Play("TimeOver");
            Timertext.color = Color.red;
            bar.fillAmount = 0;
            PooMedal.SetActive(true);
        }


    }
    void SetStats()
    {
        ps.abilityCount = abilityLeft;
        ps.maxAbilityCount = maxAbility;
        Timertext.text = RemainingTime.ToString();
        FinalTime.text = Timertext.text;

        if (abilityLeft == 3)
        {
            ability1.SetActive(true);
            ability2.SetActive(true);
            ability3.SetActive(true);
        }
        else if (abilityLeft == 2)
        {
            ability1.SetActive(false);
            ability2.SetActive(true);
            ability3.SetActive(true);
        }
        else if (abilityLeft == 1)
        {
            ability1.SetActive(false);
            ability2.SetActive(false);
            ability3.SetActive(true);
        }
        else
        {
            ability1.SetActive(false);
            ability2.SetActive(false);
            ability3.SetActive(false);
        }
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
