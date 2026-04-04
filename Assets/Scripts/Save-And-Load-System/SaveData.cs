using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<string> completedMinigames;
    public List<HighScoreEntry> highScores;
    public List<string> seenDialogues; // Added by Daniil 04-04-2026

    public SaveData()
    {
        completedMinigames = new List<string>();
        highScores = new List<HighScoreEntry>();
        seenDialogues = new List<string>(); // Added by Daniil 04-04-2026
    }
}

[System.Serializable]
public class HighScoreEntry
{
    public string minigameID;
    public int score;
}