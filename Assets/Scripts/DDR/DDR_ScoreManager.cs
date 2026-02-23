// Created by Daniil Makarenko

using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DDR_ScoreManager : MonoBehaviour
{
    // Singleton for easy access from anywhere
    public static DDR_ScoreManager Instance { get; private set; }

    public Conductor gameConductor; 

    public GameObject hitZone; 

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public GameObject restartPanel;
    public TextMeshProUGUI scoreMsg;
    public TextMeshProUGUI[] lifeTexts;

    [Header("Score Settings")]
    public int currentScore = 0; // Current score
    public int maxLives = 3; // Maximum allowed missed notes
    private int currentLives = 3; // Lives remaining

    public float timeMsgVisible; 
    private float timeWhenDisappear; 

    public int HIGHTIER = 3; 
    public int MIDTIER = 2; 
    public int LOWTIER = 1; 


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
        scoreMsg.gameObject.SetActive(false); 
    }

    void Update()
    {
        if (scoreMsg != null)
        {
            if (Time.time >= timeWhenDisappear)
            {
                scoreMsg.gameObject.SetActive(false); 
            }
        }
    }

    // Call this on successfully hit
    public void AddScore(string category)
    {
        switch (category)
        {
            case "Perfect!" :
                currentScore+=HIGHTIER;
                break;
            case "Great!" : 
                currentScore+=MIDTIER;
                break; 
            case "Okay" : 
                currentScore+=LOWTIER;
                break;
        }
        Debug.Log(category); 
        scoreMsg.text = category; 
        scoreMsg.gameObject.SetActive(true); 
        timeWhenDisappear = Time.time + timeMsgVisible; 
        UpdateScoreDisplay();
        // Trigger any score-based events here like when I implement audio and art changes
        OnScoreChanged();
    }

    // Call this on miss
    public void LoseLife()
    {
        currentLives--;
        UpdateLivesDisplay();
        scoreMsg.text= "Oops!"; 
        scoreMsg.gameObject.SetActive(true); 
        timeWhenDisappear = Time.time + timeMsgVisible; 

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

            gm.MarkMinigameCompleted("DDR");

            if (!gm.highScores.ContainsKey("DDR") || currentScore > gm.highScores["DDR"])
            {
                gm.highScores["DDR"] = currentScore;
            }

            SaveSystem.Save(gm.currentSaveSlot);
        }
        currentScore = 0;
        currentLives = maxLives;
        UpdateScoreDisplay();
        UpdateLivesDisplay();
        OnScoreChanged();

        gameConductor.ResetGame(); 

        hitZone.SetActive(false); 

        restartPanel.SetActive(true); 
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