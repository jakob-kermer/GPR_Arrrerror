using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider sfxVolume;

    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadMusicVolume();
        }
        else
        {
            SetMusicVolume();
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadSFXVolume();
        }
        else
        {
            SetSFXVolume();
        }
    }

    public void SetMusicVolume()
    {
        float musicvolume = musicVolume.value;
        myMixer.SetFloat("Music", Mathf.Log10(musicvolume) * 20);
        PlayerPrefs.SetFloat("musicVolume", musicvolume);
    }

    public void SetSFXVolume()
    {
        float sfxvolume = sfxVolume.value;
        myMixer.SetFloat("SFX", Mathf.Log10(sfxvolume) * 20);
    }

    private void LoadMusicVolume()
    {
        musicVolume.value = PlayerPrefs.GetFloat("musicVolume");

        SetMusicVolume();
    }

    private void LoadSFXVolume()
    {
        sfxVolume.value = PlayerPrefs.GetFloat("sfxVolume");
    }
}