// Created by Daniil Makarenko

using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // Singleton for easy access from anywhere
    public static ScoreManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI[] lifeTexts;

    [Header("Score Settings")]
    public int currentScore = 0; // Current score
    public int maxLives = 3; // Maximum allowed missed birdies
    private int currentLives = 3; // Lives remaining

    // These parameters control how much harder the game gets with increasing score
    [Header("Difficulty Scaling")]
    public float baseFlightSpeed = 8f; // Starting flight speed
    public float speedIncreasePerHit = 0.2f; // How much increases per score
    public float maxFlightSpeed = 20f; // Maximum speed

    public float baseGroundedWindow = 0.5f; // Starting grounded hit window (seconds)
    public float windowDecreasePerHit = 0.02f; // How much decreases per hit
    public float minGroundedWindow = 0.1f; // Never shorter than this

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
        currentLives = maxLives;
        UpdateScoreDisplay();
        UpdateLivesDisplay();
    }

    // Call this on successfully hit
    public void AddScore()
    {
        currentScore++;
        UpdateScoreDisplay();

        // Trigger any score-based events here like when I implement audio and art changes
        OnScoreChanged();
    }

    // Call this on miss
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
        if (currentScore > 10)
        {
            var gm = GameManager.Instance;

            gm.completedMinigames.Add("badminton");

            if (!gm.highScores.ContainsKey("badminton") || currentScore > gm.highScores["badminton"])
            {
                gm.highScores["badminton"] = currentScore;
            }

            SaveSystem.Save(gm.currentSaveSlot);
        }
        currentScore = 0;
        currentLives = maxLives;
        UpdateScoreDisplay();
        UpdateLivesDisplay();
        OnScoreChanged();
    }

    // Reset score
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
        OnScoreChanged();
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

    // Calculate current flight speed based on score
    public float GetCurrentFlightSpeed()
    {
        float speed = baseFlightSpeed + (currentScore * speedIncreasePerHit);
        return Mathf.Min(speed, maxFlightSpeed); // Limit to max speed
    }

    // Calculate current grounded hit window based on score
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