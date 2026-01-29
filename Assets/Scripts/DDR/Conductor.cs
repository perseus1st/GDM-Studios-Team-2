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
    public int songPositionInBeats;

    //How many seconds have passed since the song started
    public float dspSongTime;

    //an AudioSource attached to this GameObject that will play the music.
    public int index; 
    public Chart chart; 

    private float[] laneX = new float[] {-6f, -2f, 2f, 6f}; 
    private float laneY = 6f; 

    public GameObject notePrefab; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        songBpm = 134f; 
        secPerBeat = 60f / songBpm; 
        index = 0; 
    }

    public void OnButtonClicked()
    {
        dspSongTime = (float)AudioSettings.dspTime; 
        GetComponent<AudioSource>().Play(); 
    }

    // Update is called once per frame
    void Update()
    {
        songPosition = (float)(AudioSettings.dspTime - dspSongTime); 
        songPositionInBeats = (int)(songPosition / secPerBeat); 

        if (index >= chart.notes.Count)
        {
            return;
        }

       Note next = chart.notes[index];

        if (songPositionInBeats == next.beat)
        {
            SpawnNote(next);
            index++;
        }
    }

    void SpawnNote(Note note)
    {
        int laneIndex = note.row; 
        Vector3 spawnPos = new Vector3(laneX[laneIndex], laneY, 0f);
        GameObject obj = Instantiate(notePrefab, spawnPos, Quaternion.identity);
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sprite = note.sprite;
    }

  void OnApplicationQuit()
  {
    songPosition = 0f;
    songPositionInBeats = 0;
    dspSongTime = 0f;
  }
}
