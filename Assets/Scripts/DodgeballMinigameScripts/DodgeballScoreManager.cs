// Created by Daniil Makarenko

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DodgeballScoreManager : MonoBehaviour
{
    // Singleton for easy access from anywhere
    public static DodgeballScoreManager Instance { get; private set; }
    
    [Header("UI References")]
    public TextMeshProUGUI[] lifeTexts;
    [SerializeField] private Image scoreFillImage;
    [SerializeField] private TextMeshProUGUI scoreTextWhite;  // The white base text
    [SerializeField] private TextMeshProUGUI scoreTextPink;   // The pink filled text
    
    [Header("References")]
    public DodgeballEnemyManager enemyManager; // Reference to enemy manager for resets
    public AudioSource whistleSound; // Sound when minigame is completed
    public float completionDelay = 2f; // Pause before scene transition
    
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

    [Header("Scene Completion")]
    public int scoreToComplete = 15; // Score needed to complete minigame
    public string minigameID = "dodgeball"; // Unique ID for this minigame
    public string sceneToLoad = "Sister_Room"; // Scene to load on completion
    private bool minigameCompleted = false; // Has minigame been completed this session

    [Header("Music")]
    public AudioSource musicSource; // Single audio source for playback
    public AudioClip track1;
    public AudioClip track2;
    public AudioClip track3;
    public float crossfadeDuration = 1f; // Duration of crossfade back to track 1 on reset
    
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


        if (musicSource != null && track1 != null)
        {
            musicSource.clip = track1;
            musicSource.loop = false; // Loop handled by Update
            musicSource.Play();
        }
    }

private AudioClip nextClip; // Queued clip to play after current track finishes

void Update()
{
    if (musicSource == null || minigameCompleted) return;

    // Queue the correct next track based on current score
    float threshold1 = scoreToComplete / 3f;
    float threshold2 = scoreToComplete * 2f / 3f;

    if (currentScore < threshold1)
        nextClip = track1;
    else if (currentScore < threshold2)
        nextClip = track2;
    else
        nextClip = track3;

    // Wait for current track to finish before switching
    if (!musicSource.isPlaying)
    {
        musicSource.clip = nextClip;
        musicSource.loop = false; // All tracks are handled manually
        musicSource.Play();
    }
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
            DodgeballAudioManager.INSTANCE.PlaySFX("Reset");
            musicSource.Stop();
            nextClip = track1;
            ResetGame();
        } else
        {
            DodgeballAudioManager.INSTANCE.PlaySFX("Mistake");
        }
    }
    
    // Reset score and lives
    void ResetGame()
    {
        // Save highscore if score is above 5
        // if (currentScore > 5)
        // {
        //     var gm = GameManager.Instance;

        //     gm.completedMinigames.Add("dodgeball");

        //     if (!gm.highScores.ContainsKey("dodgeball") || currentScore > gm.highScores["dodgeball"])
        //     {
        //         gm.highScores["dodgeball"] = currentScore;
        //     }

        //     SaveSystem.Save(gm.currentSaveSlot);
        // }

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

IEnumerator CrossfadeToTrack1()
{
    if (musicSource == null || track1 == null) yield break;

    // Fade out
    float startVolume = musicSource.volume;
    float elapsed = 0f;
    while (elapsed < crossfadeDuration / 2f)
    {
        musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (crossfadeDuration / 2f));
        elapsed += Time.deltaTime;
        yield return null;
    }

    // Switch to track 1
    musicSource.volume = 0f;
    musicSource.clip = track1;
    musicSource.loop = true;
    musicSource.Play();

    // Fade in
    elapsed = 0f;
    while (elapsed < crossfadeDuration / 2f)
    {
        musicSource.volume = Mathf.Lerp(0f, startVolume, elapsed / (crossfadeDuration / 2f));
        elapsed += Time.deltaTime;
        yield return null;
    }

    musicSource.volume = startVolume;
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
    
    // Public getters
    public int GetScore()
    {
        return currentScore;
    }

    // Called when player reaches completion score
    void CompleteMinigame()
{
    minigameCompleted = true;
    Debug.Log($"Minigame completed at score {currentScore}!");

    var gm = GameManager.Instance;

    DodgeballAudioManager.INSTANCE.PlaySFX("Whistle");

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
    // Stop enemies
    if (enemyManager != null)
        enemyManager.enabled = false;

    // Stop music
    if (musicSource != null)
        musicSource.Stop();

// Stop Friendly balls from spawning
DodgeballFriendlyBallSpawner friendlyBallSpawner = FindFirstObjectByType<DodgeballFriendlyBallSpawner>();
if (friendlyBallSpawner != null)
    friendlyBallSpawner.enabled = false;

    // Stop player
    DodgeballPlayerController playerController = FindFirstObjectByType<DodgeballPlayerController>();
    if (playerController != null)
    {
        playerController.isInvincible = true; // character cant get damaged by moving balls
        playerController.GetComponentInChildren<Animator>().SetBool("IsMoving", false); // stop character animation
        playerController.enabled = false; 
        playerController.Rigidbody.linearVelocity = Vector3.zero; // stop moving character
    }

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

    
    public int GetLives()
    {
        return currentLives;
    }
}