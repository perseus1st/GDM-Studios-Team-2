using System.Collections;
using System.Collections.Generic;
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
    private int index; 
    public Chart chart; 

    private float[] laneX = new float[] {-6f, -2f, 2f, 6f}; 
    private float laneY = 6f; 

    public GameObject notePrefab; 

    public Queue<NoteMover> AactiveNotes; 
    public Queue<NoteMover> WactiveNotes; 
    public Queue<NoteMover> SactiveNotes; 
    public Queue<NoteMover> DactiveNotes; 



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        songBpm = 134f; 
        secPerBeat = 60f / songBpm; 
        index = 0; 
        AactiveNotes = new Queue<NoteMover>(); 
        WactiveNotes = new Queue<NoteMover>(); 
        SactiveNotes = new Queue<NoteMover>(); 
        DactiveNotes = new Queue<NoteMover>(); 
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

        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     if (AactiveNotes.Count != 0)
        //     {
        //         NoteMover pressedNote = AactiveNotes.Dequeue();
        //         Destroy(pressedNote);
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     if (WactiveNotes.Count != 0)
        //     {
        //         NoteMover pressedNote = WactiveNotes.Dequeue();
        //         Destroy(pressedNote);
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     if (SactiveNotes.Count != 0)
        //     {
        //         NoteMover pressedNote = SactiveNotes.Dequeue();
        //         Destroy(pressedNote);
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     if (DactiveNotes.Count != 0)
        //     {
        //         NoteMover pressedNote = DactiveNotes.Dequeue();
        //         Destroy(pressedNote);
        //     }
        // }
    }

    void SpawnNote(Note note)
    {
        int laneIndex = note.row; 
        Vector3 spawnPos = new Vector3(laneX[laneIndex], laneY, 0f);
        GameObject obj = Instantiate(notePrefab, spawnPos, Quaternion.identity);
        NoteMover inst = obj.GetComponent<NoteMover>(); 
        inst.GetComponent<NoteMover>().conductor = this;

        inst.active = true; 
        inst.lane = laneIndex; 
        inst.beat = note.beat; 

        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sprite = note.sprite;

        // enqueue instance in corresponding lane 
        switch (inst.lane)
        {
            case 0 :
                AactiveNotes.Enqueue(inst);
                break;
            case 1 : 
                WactiveNotes.Enqueue(inst);
                break; 
            case 2 : 
                SactiveNotes.Enqueue(inst);
                break;
            case 3 : 
                DactiveNotes.Enqueue(inst);
                break;
            default:
                break; 
        }
    }

  void OnApplicationQuit()
  {
    songPosition = 0f;
    songPositionInBeats = 0;
    dspSongTime = 0f;
  }
}
