using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    public Sound[] musicSounds, ambientSounds, sfxSounds;
    public AudioSource musicSource, ambientSource, sfxSource;

    private void Start()
    {
        PlayMusic("theme");
        PlayAmbient("wind");
    }

    public void PlayMusic(string name)
    {
        Sound sound = Array.Find(musicSounds, x => x.name == name);

        if (sound == null)
        {
            Debug.LogWarning("Music Not Found");
        }
        else
        {
            musicSource.clip = sound.clip;
            musicSource.Play();
        }
    }

    public void PlayAmbient(string name)
    {
        Sound sound = Array.Find(ambientSounds, x => x.name == name);

        if (sound == null)
        {
            Debug.LogWarning("Ambient Sound Not Found");
        }
        else
        {
            ambientSource.clip = sound.clip;
            ambientSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound sound = Array.Find(sfxSounds, x => x.name == name);

        if (sound == null)
        {
            Debug.LogWarning("Sound Not Found");
        }
        else
        {
            musicSource.PlayOneShot(sound.clip);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

}
