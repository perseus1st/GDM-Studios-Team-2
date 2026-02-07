using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<string> completedMinigames;
    public List<HighScoreEntry> highScores;

    public SaveData()
    {
        completedMinigames = new List<string>();
        highScores = new List<HighScoreEntry>();
    }
}

[System.Serializable]
public class HighScoreEntry
{
    public string minigameID;
    public int score;
}