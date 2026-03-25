using UnityEngine;
using System;
using System.Collections;

public class DDRAudioManager : MonoBehaviour
{
    public static DDRAudioManager INSTANCE;

    [Header("Sounds")]
    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    [Header("Sources")]
    public AudioSource musicSourceA;
    public AudioSource musicSourceB;
    public AudioSource sfxSource;

    AudioSource activeMusicSource;
    AudioSource inactiveMusicSource;

    [Header("Fade Settings")]
    public float fadeOutTime = 3f;      
    public float fadeInTime = 1.5f; 

    private void Awake()
    {
        // Singleton pattern
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Safety check
        if (musicSourceA == null || musicSourceB == null || sfxSource == null)
        {
            Debug.LogError("AudioSources are not assigned in DDRAudioManager!");
        }

        activeMusicSource = musicSourceA;
        inactiveMusicSource = musicSourceB;

        sfxSource.ignoreListenerPause = true;
    }

    // Play music by name
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("Music not found: " + name);
            return;
        }

        bool alreadyPlaying =
            (activeMusicSource.clip == s.clip && activeMusicSource.isPlaying) ||
            (inactiveMusicSource.clip == s.clip && inactiveMusicSource.isPlaying);

        if (alreadyPlaying)
            return;

        StartCoroutine(CrossfadeMusic(s.clip));
    }

    IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        Sound s = Array.Find(musicSounds, x => x.clip == newClip);
        if (s == null)
        {
            Debug.LogWarning("Clip not found in musicSounds: " + newClip.name);
            yield break;
        }

        inactiveMusicSource.clip = newClip;
        inactiveMusicSource.volume = 0;
        inactiveMusicSource.Play();

        float targetVolume = s.volume;

        float elapsed = 0f;
        float maxTime = Mathf.Max(fadeOutTime, fadeInTime);

        while (elapsed < maxTime)
        {
            elapsed += Time.deltaTime;

            // Fade-out uses fadeOutTime
            activeMusicSource.volume = Mathf.Lerp(activeMusicSource.volume, 0f, Mathf.Clamp01(elapsed / fadeOutTime));
            // Fade-in uses fadeInTime
            inactiveMusicSource.volume = Mathf.Lerp(0f, targetVolume, Mathf.Clamp01(elapsed / fadeInTime));

            yield return null;
        }

        activeMusicSource.Stop();

        // Swap sources
        AudioSource temp = activeMusicSource;
        activeMusicSource = inactiveMusicSource;
        inactiveMusicSource = temp;
    }


    // Play SFX by name
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("SFX not found: " + name);
            return;
        }

        sfxSource.PlayOneShot(s.clip, s.volume);
        // Debug.Log("Playing SFX: " + name);
    }

    // Stop the music
    public void StopMusic()
    {
        StartCoroutine(FadeOutMusic(4f));
    }

    IEnumerator FadeOutMusic(float customFadeTime = -1f)
    {
        float duration = (customFadeTime > 0) ? customFadeTime : fadeOutTime;

        float startVolume = activeMusicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            activeMusicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        activeMusicSource.Stop();
        activeMusicSource.volume = startVolume; // reset volume in case it plays again later
    }

    public void PauseMusic()
    {
        if (activeMusicSource && activeMusicSource.isActiveAndEnabled)
            activeMusicSource.Pause();

        if (inactiveMusicSource && inactiveMusicSource.isActiveAndEnabled)
            inactiveMusicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (activeMusicSource)
            activeMusicSource.UnPause();

        if (inactiveMusicSource)
            inactiveMusicSource.UnPause();
    }
}