// Created by Daniil Makarenko

using UnityEngine;
using TMPro;

public class DodgeballScoreManager : MonoBehaviour
{
    // Singleton for easy access from anywhere
    public static DodgeballScoreManager Instance { get; private set; }
    
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI[] lifeTexts;
    
    [Header("References")]
    public DodgeballEnemyManager enemyManager; // Reference to enemy manager for resets
    
    [Header("Score Settings")]
    public int currentScore = 0; // Current score
    public int maxLives = 3; // Maximum allowed hits
    private int currentLives = 3; // Lives remaining
    
    // These parameters control how much harder the game gets with increasing score
    [Header("Difficulty Scaling")]
    public float baseBallSpeed = 8f; // Starting ball speed
    public float maxBallSpeed = 20f; // Maximum ball speed at max difficulty
    public float speedIncreasePerHit = 0.4f; // How much speed increases per score point
    
    public float baseThrowFrequency = 3f; // Starting time between throws (seconds)
    public float minThrowFrequency = 0.5f; // Minimum time between throws at max difficulty
    public float frequencyDecreasePerHit = 0.08f; // How much time decreases per score point
    
    public float baseGroundedWindow = 3f; // Starting time ball stays on ground (seconds)
    public float minGroundedWindow = 1f; // Minimum time at max difficulty
    public float windowDecreasePerHit = 0.05f; // How much decreases per hit
    
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
        // Try to find enemy manager if not set
        if (enemyManager == null)
        {
            enemyManager = FindFirstObjectByType<DodgeballEnemyManager>();
        }
        
        currentLives = maxLives;
        UpdateScoreDisplay();
        UpdateLivesDisplay();
    }
    
    // Call this on successfully throwing ball back
    public void AddScore()
    {
        currentScore++;
        UpdateScoreDisplay();
        
        // Trigger any score-based events here like when I implement audio and art changes
        OnScoreChanged();
    }
    
    // Call this on getting hit
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
        
        // Reset enemy manager state
        if (enemyManager != null)
        {
            enemyManager.ResetMultipleThrowers();
        }
    }
    
    // Reset score
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
        OnScoreChanged();
        
        // Reset enemy manager state
        if (enemyManager != null)
        {
            enemyManager.ResetMultipleThrowers();
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
    
    // Calculate current ball speed based on score
    public float GetCurrentBallSpeed()
    {
        float speed = baseBallSpeed + (currentScore * speedIncreasePerHit);
        return Mathf.Min(speed, maxBallSpeed); // Limit to max speed
    }
    
    // Calculate current throw frequency based on score
    public float GetCurrentThrowFrequency()
    {
        float frequency = baseThrowFrequency - (currentScore * frequencyDecreasePerHit);
        return Mathf.Max(frequency, minThrowFrequency); // Limit to minimum time
    }
    
    // Calculate current grounded window based on score
    public float GetCurrentGroundedWindow()
    {
        float window = baseGroundedWindow - (currentScore * windowDecreasePerHit);
        return Mathf.Max(window, minGroundedWindow); // Limit to minimum time
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