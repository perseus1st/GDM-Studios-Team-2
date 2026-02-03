using System.Dynamic;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Runtime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //Temporary, this function will be moved to the individual minigame scripts once they have ending logic
    public void OnMiniGameFinished(string minigameID)
    {
        var gm = GameManager.Instance;
        int score = ScoreManager.Instance.currentScore;

        gm.completedMinigames.Add(minigameID);

        if (!gm.highScores.ContainsKey(minigameID) || score > gm.highScores[minigameID])
        {
            gm.highScores[minigameID] = score;
        }

        SaveSystem.Save(gm.currentSaveSlot);
    }

}

