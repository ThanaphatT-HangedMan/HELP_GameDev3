using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] protected SoundSetting soundSetting;

    public Slider slider_Master_Volume;
    public Slider slider_Music_Volume;
    public Slider slider_SFX_Volume;
    public Slider slider_GameSFX_Volume;
    public Slider slider_Fart_Volume;
    public Slider slider_OtherSFX_Volume;
    public Slider slider_UI_Volume;

    void Start()
    {
        InitialiseVolume();
        Debug.Log(soundSetting.MasterVolumeName);
        
    }

    private void InitialiseVolume()
    {
        SetMasterVolume(soundSetting.MasterVolume);
    }

    public void SetMasterVolume(float vol)
    {
        soundSetting.audioMixer.SetFloat(soundSetting.MasterVolumeName, vol);
        soundSetting.MasterVolume = vol;
        slider_Master_Volume.value = soundSetting.MasterVolume;
    }

}
