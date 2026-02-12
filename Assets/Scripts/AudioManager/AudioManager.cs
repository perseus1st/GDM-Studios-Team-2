using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager INSTANCE;
    public Sound[] musicSounds;
    public Sound[] sfxSounds;
    public string theme;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (theme != null)
        {
            PlayMusic(theme);
        }
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x=> x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");   
        }

        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x=> x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");   
        }

        else
        {
            musicSource.PlayOneShot(s.clip);
        }
    }

    public void PauseMusic()
    {
        if (musicSource.clip != null)
        {
            musicSource.Pause();
        }
    }

    public void UnpauseMusic()
    {
        if (musicSource.clip != null)
        {
            musicSource.UnPause();
        }
    }
}