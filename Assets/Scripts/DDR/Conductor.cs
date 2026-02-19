using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class Conductor : MonoBehaviour
{
    PlayerInput playerInput; 
    public DDR_ScoreManager scoreManager; 
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

    public Queue<GameObject> AactiveNotes; 
    public Queue<GameObject> WactiveNotes; 
    public Queue<GameObject> SactiveNotes; 
    public Queue<GameObject> DactiveNotes; 

    [Header("Pause Manager")]
    [SerializeField] PauseManager pauseManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Start!"); 
        playerInput = GetComponent<PlayerInput>(); 

        songBpm = 140f; 
        secPerBeat = 60f / songBpm; 
        index = 0; 
        AactiveNotes = new Queue<GameObject>(); 
        WactiveNotes = new Queue<GameObject>(); 
        SactiveNotes = new Queue<GameObject>(); 
        DactiveNotes = new Queue<GameObject>(); 
    }

    public void OnButtonClicked()
    {
        playerInput.SwitchCurrentActionMap("DDR_Minigame"); 
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
            SpawnNote(next, songPosition);
            index++;
        }
    }

    private void calculatePts(float targetTime)
    {
        float diff = Math.Abs(songPosition - targetTime); 
        if (diff <= 0.1)
        {
            scoreManager.AddScore("Perfect!");
        }
        else if (diff <= 0.15)
        {
            scoreManager.AddScore("Great!");
        } 
        else if (diff <= 0.25)
        {
            scoreManager.AddScore("Okay");
        } else
        {
            scoreManager.LoseLife(); 
        }
    }

    void OnInteract(InputValue value)
    {
        Debug.Log("Click pressed!"); 
    }

    void OnUp(InputValue value)
    {
        Debug.Log("up!"); 
        if (WactiveNotes.Count != 0)
            {
                GameObject pressedNote = WactiveNotes.Dequeue();
                calculatePts(pressedNote.GetComponent<NoteMover>().targetTime); 
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
         if (DactiveNotes.Count != 0)
            {
                GameObject pressedNote = DactiveNotes.Dequeue();
                calculatePts(pressedNote.GetComponent<NoteMover>().targetTime); 
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
         if (SactiveNotes.Count != 0)
            {
                GameObject pressedNote = SactiveNotes.Dequeue();
                calculatePts(pressedNote.GetComponent<NoteMover>().targetTime); 
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
         if (AactiveNotes.Count != 0)
            {
                GameObject pressedNote = AactiveNotes.Dequeue();
                calculatePts(pressedNote.GetComponent<NoteMover>().targetTime); 
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
            Debug.Log("pause");
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

        inst.targetTime = currTime + 12f / inst.getSpeed(); 
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

  void OnApplicationQuit()
  {
    songPosition = 0f;
    songPositionInBeats = 0;
    dspSongTime = 0f;
  }
}
