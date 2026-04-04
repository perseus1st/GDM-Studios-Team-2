using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  //allows to reference game manager from anywhere 
    public GameState State; 
    public static event Action<GameState> OnGameStateChanged;

    public HashSet<string> completedMinigames = new HashSet<string>(); 
    public Dictionary<string, int> highScores = new();
    public HashSet<string> seenDialogues = new HashSet<string>(); // Added by Daniil 04-04-2026

    public int currentSaveSlot = -1;

  void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this; 
        DontDestroyOnLoad(gameObject); 
    }
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
    {
        UpdateGameState(GameState.Options); 

        Debug.Log("Hello world"); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState; 

        //TODO write specific logic for each state 
        switch (newState)
        {
            case GameState.Options:
                break;
            case GameState.Paused: 
                break; 
            case GameState.Cutscene:
                break; 
            case GameState.Minigame: 
                break; 
            case GameState.Room:
                break; 
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null); 
        }

        OnGameStateChanged?.Invoke(newState); 
    }

    public void MarkMinigameCompleted(string minigameId)
    {
        completedMinigames.Add(minigameId);
    }


    public bool IsMinigameCompleted(string minigameId)
    {
        return completedMinigames.Contains(minigameId);
    }

    public int getNumMinigameCompleted()
    {
        return completedMinigames.Count;
    }
}

public enum GameState
{
    Options,
    Paused, 
    Cutscene, 
    Room,
    Minigame 
}
