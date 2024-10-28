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

    public void SetMusicVolume(float vol)
    {
        soundSetting.audioMixer.SetFloat(soundSetting.MusicVolumeName, vol);
        soundSetting.MusicVolume = vol;
        slider_Master_Volume.value = soundSetting.MusicVolume;
    }

    public void SetSFXVolume(float vol)
    {
        soundSetting.audioMixer.SetFloat(soundSetting.SFXVolumeName, vol);
        soundSetting.SFXVolume = vol;
        slider_Master_Volume.value = soundSetting.SFXVolume;
    }

    public void SetGameSFXVolume(float vol)
    {
        soundSetting.audioMixer.SetFloat(soundSetting.GameSFXVolumeName, vol);
        soundSetting.GameSFXVolume = vol;
        slider_Master_Volume.value = soundSetting.GameSFXVolume;
    }

    public void SetFartVolume(float vol)
    {
        soundSetting.audioMixer.SetFloat(soundSetting.FartVolumeName, vol);
        soundSetting.FartVolume = vol;
        slider_Master_Volume.value = soundSetting.FartVolume;
    }

    public void SetOtherSFXVolume(float vol)
    {
        soundSetting.audioMixer.SetFloat(soundSetting.OtherSFXVolumeName, vol);
        soundSetting.OtherSFXVolume = vol;
        slider_Master_Volume.value = soundSetting.OtherSFXVolume;
    }

    public void SetUIVolume(float vol)
    {
        soundSetting.audioMixer.SetFloat(soundSetting.UIVolumeName, vol);
        soundSetting.UIVolume = vol;
        slider_Master_Volume.value = soundSetting.UIVolume;
    }
}
