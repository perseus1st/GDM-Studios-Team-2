using UnityEngine;

public class Conductor : MonoBehaviour
{
    public float songBpm; //SET THIS 

    //The offset to the first beat of the song in seconds
    public float firstBeatOffset; //SET THIS 
    //The number of seconds for each song beat
    public float secPerBeat;

    //Current song position, in seconds
    public float songPosition;

    //Current song position, in beats
    public float songPositionInBeats;

    //How many seconds have passed since the song started
    public float dspSongTime;

    //an AudioSource attached to this GameObject that will play the music.
    public AudioSource musicSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        songBpm = 134f; 
        secPerBeat = 60f / songBpm; 
        dspSongTime = (float)AudioSettings.dspTime; 
        GetComponent<AudioSource>().Play(); 
    }

    // Update is called once per frame
    void Update()
    {
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset); 
        songPositionInBeats = songPosition / secPerBeat; 
    }
}
