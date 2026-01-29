using UnityEngine;

public class KiteMinigameManager : MonoBehaviour
{
    public bool IsRunning { get; private set; } = true;

    public void GameOver() {
        IsRunning = false;
        Debug.Log("Minigame Over");
    }
}
