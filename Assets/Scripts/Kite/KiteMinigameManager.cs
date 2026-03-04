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
    
    // These parameters control how much harder the game gets with increasing score
    [Header("Difficulty Scaling")]
    public float speedScale = 0.5f; // Starting speed
    public bool IsRunning { get; private set; } = true;    

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
        // Try to find minigame manager if not set
        if (enemyManager == null)
        {
            enemyManager = FindFirstObjectByType<ObstacleSpawner>();
        }   
        
        currentLives = maxLives;
        UpdateScoreDisplay();
        UpdateLivesDisplay();
    }
    
    // Call this on successfully collecting wind charge
    public void AddScore()
    {
        currentScore++;
        UpdateScoreDisplay();
        
        // Trigger any score-based events here like when I implement audio and art changes
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
        // // Save highscore if score is above 5
        // if (currentScore > 5)
        // {
        //     var gm = GameManager.Instance;

        //     gm.completedMinigames.Add("Kite");

        //     if (!gm.highScores.ContainsKey("Kite") || currentScore > gm.highScores["Kite"])
        //     {
        //         gm.highScores["Kite"] = currentScore;
        //     }

        //     SaveSystem.Save(gm.currentSaveSlot);
        // }

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
    
    // Reset score
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
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

    // Called whenever score changes - add more functionality here later
    void OnScoreChanged()
    {
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
