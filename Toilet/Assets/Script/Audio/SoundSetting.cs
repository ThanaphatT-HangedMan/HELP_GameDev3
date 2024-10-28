using Palmmedia.ReportGenerator.Core.Reporting.Builders.Rendering;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu (menuName = "Audio/SoundSettingPreset", fileName = "SoundSettingPreset")]
public class SoundSetting : ScriptableObject
{
    public AudioMixer audioMixer;

    [Header("MasterVolume")]
    public string MasterVolumeName = "Master Volume";
    [Range(-80, 20)]
    public float MasterVolume;

    [Header("MusicVolume")]
    public string MusicVolumeName = "Music Volume";
    [Range(-80, 20)]
    public float MusicVolume;

    [Header("SFXVolume")]
    public string SFXVolumeName = "SFX Volume";
    [Range(-80, 20)]
    public float SFXVolume;

    [Header("Game SFX Volume")]
    public string GameSFXVolumeName = "Game SFX Volume";
    [Range(-80, 20)]
    public float GameSFXVolume;

    [Header("Fart Volume")]
    public string FartVolumeName = "Fart Volume";
    [Range(-80, 20)]
    public float FartVolume;

    [Header("Other SFX Volume")]
    public string OtherSFXVolumeName = "Other SFX Volume";
    [Range(-80, 20)]
    public float OtherSFXVolume;

    [Header("UI Volume")]
    public string UIVolumeName = "UI Volume";
    [Range(-80, 20)]
    public float UIVolume;

}
