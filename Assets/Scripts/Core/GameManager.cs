using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  //allows to reference game manager from anywhere 
    public GameState State; 
    public static event Action<GameState> OnGameStateChanged; 

  void Awake()
    {
        Instance = this; 
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
}

public enum GameState
{
    Options,
    Paused, 
    Cutscene, 
    Room,
    Minigame 
}
