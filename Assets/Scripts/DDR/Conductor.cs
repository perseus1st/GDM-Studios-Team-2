using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class Conductor : MonoBehaviour
{
    AudioSource music; 
    PlayerInput playerInput; 
    public DDR_ScoreManager scoreManager; 
    public static float songBpm = 140f; 

    //The offset to the first beat of the song in seconds
    public static float firstBeatOffset; //SET THIS 
    //The number of seconds for each song beat
    public static float secPerBeat =  60f / songBpm;

    //Current song position, in seconds
    public float songPosition;

    //Current song position, in beats
    public int songPositionInBeats;

    //How many seconds have passed since the song started
    public float dspSongTime;

    // index of the next note to be spawned, according to chart 
    private int index; 
    public Chart chart; 

    // x positions of the lanes AWSD 
    private float[] laneX = new float[] {-6f, -2f, 2f, 6f}; 
    private float laneY = 6f; 
    public float PerfectTiming = 0.1f; 
    public float GreatTiming = 0.15f; 
    public float OkayTiming = 0.25f; 
    

    public GameObject notePrefab; 

    public Queue<GameObject> AactiveNotes; 
    public Queue<GameObject> WactiveNotes; 
    public Queue<GameObject> SactiveNotes; 
    public Queue<GameObject> DactiveNotes; 

    [SerializeField] PauseManager pauseManager;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start!"); 
        playerInput = GetComponent<PlayerInput>(); 

        AactiveNotes = new Queue<GameObject>(); 
        WactiveNotes = new Queue<GameObject>(); 
        SactiveNotes = new Queue<GameObject>(); 
        DactiveNotes = new Queue<GameObject>(); 

        this.enabled = false; 
    }

    // called by scoreManager when three lives are spent 
    public void StopGame()
    {
        GetComponent<AudioSource>().Stop();
        GameObject[] activeNotes = GameObject.FindGameObjectsWithTag("Note");

        foreach (GameObject note in activeNotes)
        {
            Destroy(note);
        } 

        AactiveNotes.Clear(); 
        WactiveNotes.Clear(); 
        SactiveNotes.Clear(); 
        DactiveNotes.Clear(); 

        this.enabled = false;  
    }

    // Called when minigame starts 
    public void OnButtonClicked()
    {
        music = GetComponent<AudioSource>(); 
        // reset list of notes to be spawned from beginning 
        index = 0;

        // reset all song position markers 
        songPosition = 0f;
        songPositionInBeats = 0;
        music.time = 0f; 

        this.enabled = true; 
        
        playerInput.SwitchCurrentActionMap("DDR_Minigame"); 

        dspSongTime = (float)AudioSettings.dspTime; 
        music.Play(); 

    }

    // Update is called once per frame
    void Update()
    {
        songPosition = (float)(AudioSettings.dspTime - dspSongTime); 
        songPositionInBeats = (int)(songPosition / secPerBeat); 

        // Note spawning logic, according to song position in beats 
        if (index < chart.notes.Count)
        {
            Note next = chart.notes[index];

            if (songPositionInBeats == next.beat)
            {
                SpawnNote(next, songPosition);
                index++;
            }
        }
    }

    // Called when the player pressed a key 
    private void calculatePts(float targetTime, float pressedTime)
    {
        float diff = Math.Abs(pressedTime - targetTime); 
        if (diff <= PerfectTiming)
        {
            scoreManager.AddScore("Perfect!");
        }
        else if (diff <= GreatTiming)
        {
            scoreManager.AddScore("Great!");
        } 
        else if (diff <= OkayTiming)
        {
            scoreManager.AddScore("Okay");
        } else
        {
            scoreManager.LoseLife(); 
        }
    }

    // void OnInteract(InputValue value)
    // {
    //     Debug.Log("Click pressed!"); 
    // }

    public void PastHitzone()
    {
        scoreManager.LoseLife(); 
    }

    void OnUp(InputValue value)
    {
        Debug.Log("up!"); 
        float pressedTime = songPosition; 
        if (WactiveNotes.Count != 0)
            {
                GameObject pressedNote = WactiveNotes.Dequeue();
                calculatePts(pressedNote.GetComponent<NoteMover>().targetTime, pressedTime); 
                Destroy(pressedNote);
            }
        else
        {
            scoreManager.LoseLife(); 
        }
    }

    void OnRight(InputValue value)
    {
        Debug.Log("right!"); 
        float pressedTime = songPosition;
         if (DactiveNotes.Count != 0)
            {
                GameObject pressedNote = DactiveNotes.Dequeue();
                calculatePts(pressedNote.GetComponent<NoteMover>().targetTime, pressedTime); 
                Destroy(pressedNote);
            }
        else
        {
            scoreManager.LoseLife(); 
        }
    }

    void OnDown(InputValue value)
    {
        Debug.Log("down!"); 
        float pressedTime = songPosition;
         if (SactiveNotes.Count != 0)
            {
                GameObject pressedNote = SactiveNotes.Dequeue();
                calculatePts(pressedNote.GetComponent<NoteMover>().targetTime, pressedTime); 
                Destroy(pressedNote);
            }
        else
        {
            scoreManager.LoseLife(); 
        }
    }

    void OnLeft(InputValue value)
    {
        Debug.Log("left!"); 
        float pressedTime = songPosition;
         if (AactiveNotes.Count != 0)
            {
                GameObject pressedNote = AactiveNotes.Dequeue();
                calculatePts(pressedNote.GetComponent<NoteMover>().targetTime, pressedTime); 
                Destroy(pressedNote);
            }
        else
        {
            scoreManager.LoseLife(); 
        }
    }

    void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            pauseManager.Pause();
        }
    }

    void SpawnNote(Note note, float currTime)
    {
        int laneIndex = note.row; 
        Vector3 spawnPos = new Vector3(laneX[laneIndex], laneY, 0f);
        GameObject obj = Instantiate(notePrefab, spawnPos, Quaternion.identity);
        NoteMover inst = obj.GetComponent<NoteMover>(); 
        inst.GetComponent<NoteMover>().conductor = this;

        inst.targetTime = currTime + 12f / NoteMover.getSpeed(); 
        inst.lane = laneIndex; 
        inst.beat = note.beat; 

        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sprite = note.sprite;

        // enqueue instance in corresponding lane 
        switch (inst.lane)
        {
            case 0 :
                AactiveNotes.Enqueue(obj);
                break;
            case 1 : 
                WactiveNotes.Enqueue(obj);
                break; 
            case 2 : 
                SactiveNotes.Enqueue(obj);
                break;
            case 3 : 
                DactiveNotes.Enqueue(obj);
                break;
            default:
                break; 
        }
    }
}
