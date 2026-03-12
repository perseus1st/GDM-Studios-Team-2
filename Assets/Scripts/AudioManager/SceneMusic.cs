using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [Header("Music Settings")]
    public string musicName;     // Name of music to play from AudioManager
    public bool stopMusic = false; // If true, stop any music

    private void Start()
    {
        Debug.Log("SceneMusic Start called");

        if (AudioManager.INSTANCE == null)
        {
            Debug.LogWarning("AudioManager instance not found!");
            return;
        }

        if (stopMusic)
        {
            AudioManager.INSTANCE.StopMusic();
            Debug.Log("SceneMusic requested stop");
        }
        else if (!string.IsNullOrEmpty(musicName))
        {
            AudioManager.INSTANCE.PlayMusic(musicName);
        }

        else
        {
            // No musicName means stop the current music
            AudioManager.INSTANCE.StopMusic();
            Debug.Log("SceneMusic stopped music because no musicName is set.");
        }
    }
}