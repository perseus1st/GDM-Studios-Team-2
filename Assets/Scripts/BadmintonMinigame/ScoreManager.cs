// Created by Daniil Makarenko

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    // Singleton for easy access from anywhere
    public static ScoreManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI[] lifeTexts;
    [SerializeField] private Image scoreFillImage;
    [SerializeField] private TextMeshProUGUI scoreTextWhite;  // The white base text
    [SerializeField] private TextMeshProUGUI scoreTextPink;   // The pink filled text

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

    [Header("Scene Completion")]
    public int scoreToComplete = 15; // Score needed to complete minigame
    public string minigameID = "badminton"; // Unique ID for this minigame
    public string sceneToLoad = "Sister_Room"; // Scene to load on completion
    public AudioSource whistleSound; // Sound when minigame is completed
    public float completionDelay = 2f; // Pause before scene transition
    private bool minigameCompleted = false; // Has minigame been completed this session

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
        scoreTextWhite.text = currentScore.ToString();
        scoreTextPink.text = currentScore.ToString();

        // Update the fill amount
        if (scoreFillImage != null)
        {
            float progress = (float)currentScore / scoreToComplete;
            scoreFillImage.fillAmount = progress;
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
                    lifeTexts[i].text = "\u2665";
                    lifeTexts[i].color = new Color(196f/255f, 22f/255f, 26f/255f); 
                }
                else
                {
                    lifeTexts[i].text = "X"; 
                    lifeTexts[i].color = Color.white; 
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
        if (!minigameCompleted && currentScore >= scoreToComplete)
        {
            CompleteMinigame();
        }
    }
	
    // Called when player reaches completion score
    void CompleteMinigame()
{
    minigameCompleted = true;
    Debug.Log($"Minigame completed at score {currentScore}!");

    var gm = GameManager.Instance;

    if (gm != null)
    {
        gm.MarkMinigameCompleted(minigameID);
        if (!gm.highScores.ContainsKey(minigameID) || currentScore > gm.highScores[minigameID])
        {
            gm.highScores[minigameID] = currentScore;
        }
        SaveSystem.Save(gm.currentSaveSlot);
    }
    else
        Debug.LogWarning("GameManager not found! Cannot mark minigame as completed.");

    StartCoroutine(CompletionSequence());
}

IEnumerator CompletionSequence()
{
    // Stop player 
    BadmintonPlayerController playerController = FindFirstObjectByType<BadmintonPlayerController>();
    if (playerController != null)
    {
        playerController.enabled = false;
        playerController.Rigidbody.linearVelocity = Vector3.zero;
    }

    // Stop Birdie
    BirdieController birdie = FindFirstObjectByType<BirdieController>();
if (birdie != null)
    birdie.enabled = false;
    Renderer birdieRenderer = birdie.GetComponentInChildren<Renderer>();
if (birdieRenderer != null)
    birdieRenderer.enabled = false;

    // Play whistle
    if (whistleSound != null)
        whistleSound.Play();

    // Wait
    yield return new WaitForSeconds(completionDelay);

    // Transition
    SceneController sceneController = FindAnyObjectByType<SceneController>();
    if (sceneController != null)
        sceneController.StartAnimation(sceneToLoad);
    else
    {
        Debug.LogWarning("SceneController not found! Loading scene directly without animation.");
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
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