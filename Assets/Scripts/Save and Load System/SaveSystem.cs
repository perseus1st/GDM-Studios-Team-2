using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveSystem
{
    private static string GetPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"save_slot_{slot}.json");
    }

    public static void Save(int slot)
    {
        SaveData data = new SaveData();
        var gm = GameManager.Instance;
        data.completedMinigames.AddRange(gm.completedMinigames);
        foreach (var entry in gm.highScores)
        {
            data.highScores.Add(new HighScoreEntry {
                minigameID = entry.Key,
                score = entry.Value
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(slot), json);
    }

    public static void Load(int slot)
    {
        string path = GetPath(slot);

        if (!File.Exists(path))
        {
            Debug.Log("Error: save path doesn't exist");
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        var gm = GameManager.Instance;

        gm.completedMinigames = new HashSet<string>(data.completedMinigames);
        gm.highScores.Clear();
        foreach (var entry in data.highScores)
        {
            gm.highScores[entry.minigameID] = entry.score;
        }
        gm.currentSaveSlot = slot;
    }

    public static bool SaveExists(int slot)
    {
        return File.Exists(GetPath(slot));
    }

    public static void Delete(int slot)
    {
        string path = GetPath(slot);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Deleted save slot {slot}");
        }
    }
}
