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

    private float lnTargetTime; 
    private float lnStartTime; 
    private NoteMover currln; 
    

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
    private void calculatePts(float targetTime, float timePressed)
    {
        float diff = Math.Abs(timePressed - targetTime); 
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

    public void PastHitzone()
    {
        scoreManager.LoseLife(); 
    }

    // void OnInteract(InputValue value)
    // {
    //     if (value.isPressed)
    //     {
    //         Debug.Log("pressed"); 
    //     }
    //     else
    //     {
    //         Debug.Log("released"); 
    //     }
    // }

    private void OnHold(float targetTime, float startTime)
    {
        //store info on time pressed for later score calculation 
        lnStartTime = startTime; 
        lnTargetTime = targetTime; 
    }

    private void OnRelease(int targetBeat, int endBeat)
    {
        if (targetBeat == endBeat)
        {
            Debug.Log("released correctly"); 
            calculatePts(lnTargetTime, lnStartTime); 
        } else
        {
            Debug.Log("Released wrong time"); 
            scoreManager.LoseLife(); 
        }
    }

    // void OnUp(InputValue value)
    // {
    //     float timePressed = songPosition; 
    //     int beatPressed = songPositionInBeats; 
 
    //     if (WactiveNotes.Count != 0)
    //         {
    //             GameObject pressedNote = WactiveNotes.Dequeue();
    //             NoteMover note = pressedNote.GetComponent<NoteMover>(); 

    //             if (note.isLongNote)
    //             {
    //                 currln = note; 

    //                 if (value.isPressed)
    //                 {
    //                     Debug.Log("long note start"); 
    //                     OnHold(note.targetTime, timePressed); 
    //                 } else
    //                 {
    //                     Debug.Log("long note end"); 
    //                     OnRelease(note.beat + note.lenghtInBeats, beatPressed); 
    //                     Destroy(pressedNote);
    //                 }
    //             } else
    //             {
    //                 calculatePts(note.targetTime, timePressed); 
    //                 Destroy(pressedNote);
    //             }
                
    //         }
    //     else
    //     {
    //         scoreManager.LoseLife(); 
    //     }
    // }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("pressed"); 
        }
        else if (context.canceled)
        {
            Debug.Log("released"); 
        }
    }

    public void OnUp(InputAction.CallbackContext context)
    {
        float timePressed = songPosition; 
        int beatPressed = songPositionInBeats; 
        
        if (context.started)
        {
            if (WactiveNotes.Count != 0)
            {
                GameObject nextInQueue = WactiveNotes.Peek();
                NoteMover pressedNote = nextInQueue.GetComponent<NoteMover>(); 

                // a regular note is pressed 
                if (!pressedNote.isLongNote)
                {
                    calculatePts(pressedNote.targetTime, timePressed); 
                    WactiveNotes.Dequeue(); 
                    Destroy(nextInQueue); 
                } else   // a long note is pressed 
                {
                    float diff = Math.Abs(timePressed - pressedNote.targetTime); 
                    if (diff > OkayTiming)
                    {
                        scoreManager.LoseLife(); 
                        WactiveNotes.Dequeue(); 
                        Destroy(nextInQueue);
                    } else
                    {
                        pressedNote.isHolding = true; 
                        OnHold(pressedNote.targetTime, timePressed); 
                    }
                }
            } else  //no corresponding note in game 
            {
                scoreManager.LoseLife(); 
            }
        
        }
        else if (context.performed)
        {
            
        }
        else if (context.canceled)
        {
            if (WactiveNotes.Count != 0)
            {
                GameObject nextInQueue = WactiveNotes.Peek();
                NoteMover pressedNote = nextInQueue.GetComponent<NoteMover>();

                if (pressedNote.isLongNote && pressedNote.isHolding)
                {
                    OnRelease(pressedNote.beat + pressedNote.lenghtInBeats, beatPressed); 
                    WactiveNotes.Dequeue(); 
                    Destroy(nextInQueue);
                }
            }
        }
        
    }

    public void OnRight(InputAction.CallbackContext context)
    {
    }
    public void OnLeft(InputAction.CallbackContext context)
    {
        
    }
    public void OnDown(InputAction.CallbackContext context)
    {
        
    }

    void OnRight(InputValue value)
    {
        float timePressed = songPosition; 
        //Debug.Log("right!"); 
         if (DactiveNotes.Count != 0)
            {
                GameObject pressedNote = DactiveNotes.Dequeue();
                calculatePts(pressedNote.GetComponent<NoteMover>().targetTime, timePressed); 
                Destroy(pressedNote);
            }
        else
        {
            scoreManager.LoseLife(); 
        }
    }

    void OnDown(InputValue value)
    {
        float timePressed = songPosition;
        Debug.Log("down!"); 
         if (SactiveNotes.Count != 0)
            {
                GameObject pressedNote = SactiveNotes.Dequeue();
                calculatePts(pressedNote.GetComponent<NoteMover>().targetTime, timePressed); 
                Destroy(pressedNote);
            }
        else
        {
            scoreManager.LoseLife(); 
        }
    }

    void OnLeft(InputValue value)
    {
        float timePressed = songPosition;
        Debug.Log("left!"); 
         if (AactiveNotes.Count != 0)
            {
                GameObject pressedNote = AactiveNotes.Dequeue();
                calculatePts(pressedNote.GetComponent<NoteMover>().targetTime, timePressed); 
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
        inst.isLongNote = note.isLongNote; 
        inst.lenghtInBeats = note.lengthInBeats; 


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
   
  }
}
