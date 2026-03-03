using UnityEngine;

public class KiteMinigameManager : MonoBehaviour
{
    public bool IsRunning { get; private set; } = true;
    public int Score { get; private set; }

    public void WindGustCollected()
    {
        Score++;
    }

    public void GameOver() {
        IsRunning = false;
        Debug.Log("Minigame Over");
    }
}
