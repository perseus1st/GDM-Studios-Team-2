// Created by Daniil Makarenko

using UnityEngine;
using System.Collections;

public class DodgeballEnemyManager : MonoBehaviour
{
    // References
    [Header("References")]
    public Transform[] enemyPositions; // Array of 7 enemy positions
    public GameObject enemyBallPrefab; // Prefab for the ball to throw
    public Transform player; // Reference to player
    public DodgeballPlayerController playerController; // Reference to player controller
    // public AudioSource throwSound; // Sound when enemy throws
    private bool[] enemyStunned; // Tracks which enemies are currently stunned

    // Animation
    [Header("Animation")]
    public Animator[] enemyAnimators; // Animator for each enemy (match order with enemyPositions)
    public float throwAnimationDelay = 0.3f; // Time from animation start to ball spawn
    
    // Throw timing
    [Header("Throw Settings")]
    public float baseThrowInterval = 3f; // Base time between throws
    private float nextThrowTime; // When next throw will happen
    public float throwIntervalVariation = 0.3f; // Adds random variation to throw time

    // Ball settings
    [Header("Ball Settings")]
    public float ballLifetime = 6f; // How long before ball is destroyed off-screen
    public float hitRadius = 0.5f; // How close to player counts as hit
    
    // Difficulty scaling
    [Header("Difficulty Scaling")]
    public int scoreForMaxDifficulty = 30; // Score at which difficulty is maximized
    public int scoreForMultipleThrowers = 10; // Score threshold to enable multiple throwers
    public float maxPredictionDistance = 2f; // Maximum distance to predict ahead of player
    
    // Alternating pattern settings
    [Header("Alternating Pattern Settings")]
    public float alternatingPatternBaseDelay = 0.5f; // Base time between alternating throws
    public float alternatingPatternMinDelay = 0.1f; // Minimum time between alternating throws at high score
    
    // Crossing pattern settings
    [Header("Crossing Pattern Settings")]
    public float crossingAngleDegrees = 15f; // Angle in degrees toward center (adjustable in inspector)
    public float crossingPatternBaseDelay = 0.5f; // Base time between crossing throws
    public float crossingPatternMinDelay = 0.1f; // Minimum time between crossing throws at high score
    
    // Tracking state
    private bool hasHadMultipleThrowers = false; // Has multiple throwers been triggered yet
    private bool isExecutingPattern = false; // Is currently executing a multi-throw pattern
    
    void Start()
    {
        // Try to find player if not set
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<DodgeballPlayerController>();
        }
        
        if (player == null && playerController != null)
        {
            player = playerController.transform;
        }
       
        enemyStunned = new bool[enemyPositions.Length]; // Initiate the array
        
        // Schedule first throw
        nextThrowTime = Time.time + baseThrowInterval;
        
        // Reset multiple throwers state
        hasHadMultipleThrowers = false;
    }
    
    void Update()
    {
        // Don't schedule new throws while executing a pattern
        if (isExecutingPattern)
            return;
    
        // Check if it's time to throw
        if (Time.time >= nextThrowTime)
        {
            // Get current score
            int currentScore = DodgeballScoreManager.Instance != null 
                ? DodgeballScoreManager.Instance.GetScore() 
                : 0;
        
            // Calculate difficulty factor (0 at score 0, 1 at max score)
            float t = Mathf.Clamp01((float)currentScore / scoreForMaxDifficulty);
        
            // Define probabilities based on score
            float singleShotChance = Mathf.Lerp(0.3f, 0.2f, t);
            float alternatingChance = Mathf.Lerp(0.7f, 0.45f, t); // Stays constant
            float crossingChance = Mathf.Lerp(0.0f, 0.25f, t);
        
            // Roll for shot type
            float roll = Random.Range(0f, 1f);
        
            DodgeballAudioManager.INSTANCE.PlaySFX("Throw");
            if (roll < singleShotChance)
            {
                // Single shot (tracking throw)
                ThrowBalls();
            
                // Schedule next throw
                ScheduleNextThrow();
            }
            else if (roll < singleShotChance + alternatingChance)
            {
                // Alternating pattern
                StartCoroutine(ExecuteAlternatingPattern(currentScore));
            }
            else
            {
                // Crossing pattern
                StartCoroutine(ExecuteCrossingPattern(currentScore));
            }
        }
    }

    // Helper method to schedule next throw
    void ScheduleNextThrow()
    {
        float currentInterval = DodgeballScoreManager.Instance != null 
            ? DodgeballScoreManager.Instance.GetCurrentThrowFrequency() 
            : baseThrowInterval;

        // Add random variation 
        float minVariation = currentInterval - throwIntervalVariation;
        float maxVariation = currentInterval + throwIntervalVariation;
        float randomizedInterval = Random.Range(minVariation, maxVariation);
    
        nextThrowTime = Time.time + randomizedInterval;
    }
    
    // Execute crossing pattern based on score
    IEnumerator ExecuteCrossingPattern(int currentScore)
    {
        isExecutingPattern = true;
        
        // Determine number of volleys based on score
        int volleyCount;
        if (currentScore < 10)
        {
            volleyCount = 1; // Single volley
        }
        else if (currentScore < 20)
        {
            volleyCount = 2; // Two volleys
        }
        else
        {
            volleyCount = 3; // Three volleys
        }
        
        // Calculate delay between volleys based on score
        float t = Mathf.Clamp01((float)currentScore / scoreForMaxDifficulty);
        float volleyDelay = Mathf.Lerp(crossingPatternBaseDelay, crossingPatternMinDelay, t);
        
        // Execute volleys (always odd-numbered enemies only)
        for (int i = 0; i < volleyCount; i++)
        {
            ThrowCrossingVolley();
            
            // Wait before next volley (unless it's the last one)
            if (i < volleyCount - 1)
            {
                yield return new WaitForSeconds(volleyDelay);
            }
        }
        
        // Pattern complete - schedule next throw
        float currentInterval = DodgeballScoreManager.Instance != null 
            ? DodgeballScoreManager.Instance.GetCurrentThrowFrequency() 
            : baseThrowInterval;

        // Add random variation 
        float minVariation = currentInterval - throwIntervalVariation;
        float maxVariation = currentInterval + throwIntervalVariation;
        float randomizedInterval = Random.Range(minVariation, maxVariation);
        
        nextThrowTime = Time.time + randomizedInterval;
        
        isExecutingPattern = false;
    }
    
    // Execute alternating pattern based on score
    IEnumerator ExecuteAlternatingPattern(int currentScore)
    {
        isExecutingPattern = true;
        
        // Determine number of volleys based on score
        int volleyCount;
        if (currentScore < 10)
        {
            volleyCount = 1; // Single volley
        }
        else if (currentScore < 20)
        {
            volleyCount = 2; // Two volleys
        }
        else
        {
            volleyCount = 3; // Three volleys
        }
        
        // Calculate delay between volleys based on score
        float t = Mathf.Clamp01((float)currentScore / scoreForMaxDifficulty);
        float volleyDelay = Mathf.Lerp(alternatingPatternBaseDelay, alternatingPatternMinDelay, t);
        
        // Randomly choose to start with even or odd
        bool throwEven = Random.Range(0, 2) == 0;
        
        // Execute volleys
        for (int i = 0; i < volleyCount; i++)
        {
            ThrowAlternatingVolley(throwEven);
            
            // Alternate between even and odd
            throwEven = !throwEven;
            
            // Wait before next volley (unless it's the last one)
            if (i < volleyCount - 1)
            {
                yield return new WaitForSeconds(volleyDelay);
            }
        }
        
        // Pattern complete - schedule next throw
        float currentInterval = DodgeballScoreManager.Instance != null 
            ? DodgeballScoreManager.Instance.GetCurrentThrowFrequency() 
            : baseThrowInterval;

        // Add random variation 
        float minVariation = currentInterval - throwIntervalVariation;
        float maxVariation = currentInterval + throwIntervalVariation;
        float randomizedInterval = Random.Range(minVariation, maxVariation);
        
        nextThrowTime = Time.time + randomizedInterval;
        
        isExecutingPattern = false;
    }
    
    // Throw from even-numbered enemies at crossing angles (indices 0, 2, 4, 6)
    void ThrowCrossingVolley()
    {
        if (enemyPositions == null || enemyPositions.Length == 0 || enemyBallPrefab == null)
            return;
    
        // Throw from even-numbered enemies: indices 0,6
        for (int i = 0; i < enemyPositions.Length; i++)
        {
            // Even indices: 0, 6 (for 7 enemies at indices 0-6)
            // bool isEven = (i % 2 == 0);
            bool isSix = (i % 6 == 0);
        
            if (isSix && enemyPositions[i] != null)
            {
                ThrowAtAngleTowardCenter(enemyPositions[i]);
            }
        }
    }
    
    // Throw from either even or odd numbered enemies straight down
    void ThrowAlternatingVolley(bool throwEven)
    {
        if (enemyPositions == null || enemyPositions.Length == 0 || enemyBallPrefab == null)
            return;
        
        // Throw from every other enemy
        for (int i = 0; i < enemyPositions.Length; i++)
        {
            // Check if this index matches our even/odd requirement
            bool isEven = (i % 2 == 0);
            
            if (isEven == throwEven && enemyPositions[i] != null)
            {
                ThrowStraightDown(enemyPositions[i]);
            }
        }
    }
    
    void ThrowAtAngleTowardCenter(Transform throwingEnemy)
{
    int index = System.Array.IndexOf(enemyPositions, throwingEnemy);
    if (index >= 0 && enemyStunned != null && index < enemyStunned.Length && enemyStunned[index])
        return;

    StartCoroutine(AnimatedAngleTowardCenter(throwingEnemy, index));
}

IEnumerator AnimatedAngleTowardCenter(Transform throwingEnemy, int enemyIndex)
{
    if (enemyIndex >= 0 && enemyAnimators != null && enemyIndex < enemyAnimators.Length && enemyAnimators[enemyIndex] != null)
        enemyAnimators[enemyIndex].SetTrigger("Throw");

    yield return new WaitForSeconds(throwAnimationDelay);

    // if (throwSound != null)
    //     throwSound.Play();
    // DodgeballAudioManager.INSTANCE.PlaySFX("Throw");

    GameObject ball = Instantiate(enemyBallPrefab, throwingEnemy.position + new Vector3(-0.7f, 0f, 0f), enemyBallPrefab.transform.rotation);

    float ballSpeed = DodgeballScoreManager.Instance != null
        ? DodgeballScoreManager.Instance.GetCurrentBallSpeed() : 8f;

    float enemyX = throwingEnemy.position.x;
    float angleDirection = enemyX < 0 ? 1f : -1f;
    float angleRad = crossingAngleDegrees * Mathf.Deg2Rad;
    float xOffset = Mathf.Tan(angleRad) * 20f * angleDirection;
    Vector3 targetPosition = throwingEnemy.position + new Vector3(xOffset, 0f, -20f);

    EnemyBallBehavior ballBehavior = ball.AddComponent<EnemyBallBehavior>();
    ballBehavior.Initialize(throwingEnemy.position, targetPosition, ballSpeed, hitRadius, playerController);
    Destroy(ball, ballLifetime);
}
    
    void ThrowStraightDown(Transform throwingEnemy)
{
    int index = System.Array.IndexOf(enemyPositions, throwingEnemy);
    if (index >= 0 && enemyStunned != null && index < enemyStunned.Length && enemyStunned[index])
        return;

    StartCoroutine(AnimatedStraightDown(throwingEnemy, index));
}

IEnumerator AnimatedStraightDown(Transform throwingEnemy, int enemyIndex)
{
    if (enemyIndex >= 0 && enemyAnimators != null && enemyIndex < enemyAnimators.Length && enemyAnimators[enemyIndex] != null)
        enemyAnimators[enemyIndex].SetTrigger("Throw");

    yield return new WaitForSeconds(throwAnimationDelay);

    // if (throwSound != null)
    //     throwSound.Play();
    // DodgeballAudioManager.INSTANCE.PlaySFX("Throw");

    GameObject ball = Instantiate(enemyBallPrefab, throwingEnemy.position + new Vector3(-0.7f, 0f, 0f), enemyBallPrefab.transform.rotation);

    float ballSpeed = DodgeballScoreManager.Instance != null
        ? DodgeballScoreManager.Instance.GetCurrentBallSpeed() : 8f;

    Vector3 targetPosition = throwingEnemy.position + new Vector3(0f, 0f, -20f);

    EnemyBallBehavior ballBehavior = ball.AddComponent<EnemyBallBehavior>();
    ballBehavior.Initialize(throwingEnemy.position, targetPosition, ballSpeed, hitRadius, playerController);
    Destroy(ball, ballLifetime);
}
    
    // Determine how many enemies throw and execute throws
    void ThrowBalls()
    {
        // Validate we have enemies and a ball prefab
        if (enemyPositions == null || enemyPositions.Length == 0 || enemyBallPrefab == null)
            return;
        
        // Get current score
        int currentScore = DodgeballScoreManager.Instance != null 
            ? DodgeballScoreManager.Instance.GetScore() 
            : 0;
        
        // Determine number of simultaneous throwers
        int numberOfThrowers = 1; // Default to single thrower
        
        if (currentScore >= scoreForMultipleThrowers)
        {
            // Calculate chance for multiple throwers based on score
            float t = Mathf.Clamp01((float)(currentScore - scoreForMultipleThrowers) / (scoreForMaxDifficulty - scoreForMultipleThrowers));
            
            // Roll for number of throwers (1, 2, or 3)
            float roll = Random.Range(0f, 1f);
            
            // At low scores past threshold: mostly 1, sometimes 2, rarely 3
            // At max difficulty: mostly 2-3, rarely 1
            if (roll < Mathf.Lerp(1f, 0.99f, t)) // Chance for 1 thrower decreases
            {
                numberOfThrowers = 1;
            }
            else if (roll < Mathf.Lerp(0.95f, 0.5f, t)) // Chance for 2 throwers increases then plateaus
            {
                numberOfThrowers = 2;
            }
            else // Chance for 3 throwers increases
            {
                // Only allow 3 throwers if we've already had multiple throwers once
                numberOfThrowers = hasHadMultipleThrowers ? 3 : 2;
            }
            
            // If we selected multiple throwers, mark that we've had it
            if (numberOfThrowers > 1)
            {
                hasHadMultipleThrowers = true;
            }
        }
        
        // Throw from random enemies
        for (int i = 0; i < numberOfThrowers; i++)
        {
            // Pick random enemy (allow duplicates for simplicity, unlikely with 7 enemies)
            int randomIndex = Random.Range(0, enemyPositions.Length);
            Transform throwingEnemy = enemyPositions[randomIndex];
            
            if (throwingEnemy != null)
            {
                ThrowSingleShot(throwingEnemy);
            }
        }
    }
    
    // Starts the throw animation, then spawns ball after delay
void ThrowSingleShot(Transform throwingEnemy)
{
    int index = System.Array.IndexOf(enemyPositions, throwingEnemy);
    if (index >= 0 && enemyStunned != null && index < enemyStunned.Length && enemyStunned[index])
        return;

    StartCoroutine(AnimatedSingleShot(throwingEnemy, index));
}

IEnumerator AnimatedSingleShot(Transform throwingEnemy, int enemyIndex)
{
    // Trigger throw animation
    if (enemyIndex >= 0 && enemyAnimators != null && enemyIndex < enemyAnimators.Length && enemyAnimators[enemyIndex] != null)
        enemyAnimators[enemyIndex].SetTrigger("Throw");

    // Wait before spawning ball
    yield return new WaitForSeconds(throwAnimationDelay);

    // Create ball at enemy position
    GameObject ball = Instantiate(enemyBallPrefab, throwingEnemy.position + new Vector3(-0.7f, 0f, 0f), enemyBallPrefab.transform.rotation);

    int currentScore = DodgeballScoreManager.Instance != null
        ? DodgeballScoreManager.Instance.GetScore() : 0;

    float predictionFactor = Mathf.Clamp01((float)currentScore / scoreForMaxDifficulty);
    Vector3 playerPosition = player != null ? player.position : Vector3.zero;
    Vector3 playerVelocity = playerController != null ? playerController.GetMovementDirection() : Vector3.zero;

    float ballSpeed = DodgeballScoreManager.Instance != null
        ? DodgeballScoreManager.Instance.GetCurrentBallSpeed() : 8f;

    float distanceToPlayer = Vector3.Distance(throwingEnemy.position, playerPosition);
    float timeToReach = distanceToPlayer / ballSpeed;

    Vector3 fullPredictionOffset = playerVelocity * playerController.maxSpeed * timeToReach;
    if (fullPredictionOffset.magnitude > maxPredictionDistance)
        fullPredictionOffset = fullPredictionOffset.normalized * maxPredictionDistance;

    Vector3 predictionOffset = fullPredictionOffset * predictionFactor;
    Vector3 targetPosition = playerPosition + predictionOffset;

    EnemyBallBehavior ballBehavior = ball.AddComponent<EnemyBallBehavior>();
    ballBehavior.Initialize(throwingEnemy.position, targetPosition, ballSpeed, hitRadius, playerController);
    Destroy(ball, ballLifetime);
}
    
    // Reset multiple throwers state (called by score manager)
    public void ResetMultipleThrowers()
    {
        hasHadMultipleThrowers = false;
    }
    
    // Visual debug in editor
    void OnDrawGizmosSelected()
    {
        // Draw enemy positions
        if (enemyPositions != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform enemy in enemyPositions)
            {
                if (enemy != null)
                {
                    Gizmos.DrawWireSphere(enemy.position, 0.5f);
                }
            }
        }
    }

    public void StunEnemy(Transform enemy, float duration)
{
    int index = System.Array.IndexOf(enemyPositions, enemy);
    if (index >= 0 && index < enemyStunned.Length)
        StartCoroutine(StunCoroutine(index, enemy, duration));
}

IEnumerator StunCoroutine(int index, Transform enemy, float duration)
{
    // Move enemy forward by 0.5 in Z
    enemy.position += new Vector3(0f, 0f, 0.5f);

    // Stun — block throwing
    enemyStunned[index] = true;

    // Flash the enemy renderer
    Renderer rend = enemy.GetComponentInChildren<SpriteRenderer>();
    float elapsed = 0f;
    float flashInterval = 0.1f;
    while (elapsed < duration)
    {
        if (rend != null) rend.enabled = !rend.enabled;
        yield return new WaitForSeconds(flashInterval);
        elapsed += flashInterval;
    }

    // Restore visibility and unstun
    if (rend != null) rend.enabled = true;
    enemyStunned[index] = false;

    // Move enemy back
    enemy.position -= new Vector3(0f, 0f, 0.5f);
}

public bool IsEnemyStunned(int index)
{
    if (enemyStunned == null || index < 0 || index >= enemyStunned.Length)
        return false;
    return enemyStunned[index];
}
    
    // Inner class for ball behavior - keeps everything in one script
    private class EnemyBallBehavior : MonoBehaviour
    {
        // Movement
        private Vector3 startPosition; // Where ball was thrown from
        private Vector3 targetPosition; // Where ball is aimed at
        private Vector3 direction; // Direction ball travels
        private float speed; // How fast ball travels
        private bool isInitialized = false; // Has ball been set up
        
        // Hit detection
        private float hitRadius; // How close to player counts as hit
        private DodgeballPlayerController playerController; // Reference to player
        private bool hasHit = false; // Has ball already hit player
        
        // Called by enemy manager to set up the ball
        public void Initialize(Vector3 start, Vector3 target, float ballSpeed, float radius, DodgeballPlayerController player)
        {
            startPosition = start;
            targetPosition = target;
            speed = ballSpeed;
            hitRadius = radius;
            playerController = player;
            
            // Calculate direction from start to target
            direction = (targetPosition - startPosition).normalized;
            
            // Make sure ball travels horizontally (no vertical component)
            direction.y = 0f;
            direction.Normalize();
            
            isInitialized = true;
        }
        
        void Update()
        {
            // Don't move until initialized
            if (!isInitialized)
                return;
            
            // Move ball in straight line
            transform.position += direction * speed * Time.deltaTime;
            
            // Check for collision with player
            CheckPlayerCollision();
        }
        
        // Check if ball hit the player
        void CheckPlayerCollision()
        {
            // Don't check if already hit
            if (hasHit || playerController == null)
                return;
            
            // Check if player is invincible
            if (playerController.IsInvincible())
                return;
            
            // Calculate distance to player
            float distance = Vector3.Distance(transform.position, playerController.transform.position);
            
            // Check if close enough to hit
            if (distance <= hitRadius)
            {
                // Hit the player
                playerController.GetHit();
                hasHit = true;
                
                // Destroy ball immediately after hit
                Destroy(gameObject);
            }
        }
        
        // Visual debug in editor
        void OnDrawGizmosSelected()
        {
            // Draw hit radius
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, hitRadius);
            
            // Draw direction if initialized
            if (isInitialized)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, transform.position + direction * 2f);
            }
        }
    }
}