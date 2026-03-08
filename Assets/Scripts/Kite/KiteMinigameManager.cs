using UnityEngine;
using TMPro;

public class KiteMinigameManager : MonoBehaviour
{

    // Singleton for easy access from anywhere
    public static KiteMinigameManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI[] lifeTexts;

    [Header("References")]
    public ObstacleSpawner enemyManager; // Reference to minigame manager for resets

    [Header("Score Settings")]
    public int currentScore = 0; // Current score
    public int maxLives = 3; // Maximum allowed hits
    private int currentLives = 3; // Lives remaining
    public bool isRunning { get; private set; } = true; // To implement pausing (for tutorial and end of game)

    // These parameters control how much harder the game gets with increasing score
    [Header("Difficulty Scaling")]
    public float speedScale; // Starting speed
    public float levelOneSpeed = 0.875f; // Speed at level one
    public float levelTwoSpeed = 1.000f; // Speed at level two
    public float levelThreeSpeed = 1.125f; // Speed at level three
    public float levelFourSpeed = 1.250f; // Speed at level four
    public float levelFiveSpeed = 1.375f; // Speed at level five

    void Awake()
    {
        // Removes other score managers
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Debug.Log("MINIGAME MANAGER STARTING");
        // Try to find minigame manager if not set
        if (enemyManager == null)
        {
            enemyManager = FindFirstObjectByType<ObstacleSpawner>();
        }

        currentLives = maxLives; // Start with 3 lives
        UpdateScoreDisplay(); //
        UpdateLivesDisplay();

        // TODO:
        // show tutorial
    }

    // Call this upon successfully collecting wind charge
    public void AddScore()
    {
        currentScore++;
        UpdateScoreDisplay();
        OnScoreChanged();
    }

    // Call this on colliding with obstacle
    public void LoseLife()
    {
        currentLives--;
        UpdateLivesDisplay();

        // Check if all lives lost
        if (currentLives <= 0)
        {
            ResetGame();
        }
    }

    // Reset score and lives
    void ResetGame()
    {
        currentScore = 0;
        currentLives = maxLives;
        UpdateScoreDisplay();
        UpdateLivesDisplay();
        OnScoreChanged();

        // Reset minigame manager state
        if (enemyManager != null)
        {
            enemyManager.Reset();
        }
    }

    // Update the UI text
    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    // Update the lives UI
    void UpdateLivesDisplay()
    {
        if (lifeTexts == null || lifeTexts.Length != maxLives)
            return;

        // Update each life indicator
        for (int i = 0; i < lifeTexts.Length; i++)
        {
            if (lifeTexts[i] != null)
            {
                // If have life, show white circle. If lost life, show red x
                if (i < currentLives)
                {
                    lifeTexts[i].text = "O";
                    lifeTexts[i].color = Color.white;
                }
                else
                {
                    lifeTexts[i].text = "X";
                    lifeTexts[i].color = Color.red;
                }
            }
        }
    }

    // Called whenever score changes
    void OnScoreChanged()
    {
        if (currentScore == 0)
        {
            Debug.Log("SCORE RESET");
            Debug.Log("----------SPEED LEVEL 1----------");
            speedScale = levelOneSpeed;
            enemyManager.Reset();
            return;
        }
        else if (currentScore == 4)
        {
            Debug.Log("----------SPEED LEVEL 2----------");
            speedScale = levelTwoSpeed;
            enemyManager.SetSpeedScale(speedScale);
        }
        else if (currentScore == 8)
        {
            Debug.Log("----------SPEED LEVEL 3----------");
            speedScale = levelThreeSpeed;
            enemyManager.SetSpeedScale(speedScale);
        }
        else if (currentScore == 12)
        {
            Debug.Log("----------SPEED LEVEL 4----------");
            speedScale = levelFourSpeed;
            enemyManager.SetSpeedScale(speedScale);
        }
        else if (currentScore == 16)
        {
            Debug.Log("----------SPEED LEVEL 5----------");
            speedScale = levelFiveSpeed;
            enemyManager.SetSpeedScale(speedScale);
        }
        else if (currentScore == 20)
        {
            Debug.Log("YOU WIN");
            isRunning = false;
            //win?
        }
        // TODO: Add other effects based on score
    }

    // Public getters
    public int GetScore()
    {
        return currentScore;
    }

    public int GetLives()
    {
        return currentLives;
    }
}
