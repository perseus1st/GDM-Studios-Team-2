// Created by Daniil Makarenko

using UnityEngine;

public class DodgeballEnemyManager : MonoBehaviour
{
    // References
    [Header("References")]
    public Transform[] enemyPositions; // Array of 7 enemy positions
    public GameObject enemyBallPrefab; // Prefab for the ball to throw
    public Transform player; // Reference to player
    public DodgeballPlayerController playerController; // Reference to player controller
    public AudioSource throwSound; // Sound when enemy throws
    
    // Throw timing
    [Header("Throw Settings")]
    public float baseThrowInterval = 3f; // Base time between throws
    private float nextThrowTime; // When next throw will happen
    
    // Ball settings
    [Header("Ball Settings")]
    public float ballLifetime = 10f; // How long before ball is destroyed off-screen
    public float hitRadius = 0.5f; // How close to player counts as hit
    
    // Difficulty scaling
    [Header("Difficulty Scaling")]
    public int scoreForMaxDifficulty = 30; // Score at which difficulty is maximized
    public int scoreForMultipleThrowers = 10; // Score threshold to enable multiple throwers
    public float maxPredictionDistance = 3f; // Maximum distance to predict ahead of player
    
    // Tracking state
    private bool hasHadMultipleThrowers = false; // Has multiple throwers been triggered yet
    
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
        
        // Schedule first throw
        nextThrowTime = Time.time + baseThrowInterval;
        
        // Reset multiple throwers state
        hasHadMultipleThrowers = false;
    }
    
    void Update()
    {
        // Check if it's time to throw
        if (Time.time >= nextThrowTime)
        {
            ThrowBalls();
            
            // Schedule next throw using score-based frequency
            float currentInterval = DodgeballScoreManager.Instance != null 
                ? DodgeballScoreManager.Instance.GetCurrentThrowFrequency() 
                : baseThrowInterval;
            
            nextThrowTime = Time.time + currentInterval;
        }
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
            if (roll < Mathf.Lerp(0.7f, 0.1f, t)) // Chance for 1 thrower decreases
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
    
    // Single throw with prediction based on score
    void ThrowSingleShot(Transform throwingEnemy)
    {
        // Play throw sound
        if (throwSound != null)
            throwSound.Play();
        
        // Create ball at enemy position
        GameObject ball = Instantiate(enemyBallPrefab, throwingEnemy.position, enemyBallPrefab.transform.rotation);
        
        // Get current score for difficulty calculations
        int currentScore = DodgeballScoreManager.Instance != null 
            ? DodgeballScoreManager.Instance.GetScore() 
            : 0;
        
        // Calculate prediction amount based on score
        // At score 0: predictionFactor = 0 (no prediction, aim at current position)
        // At max score: predictionFactor = 1 (full prediction up to max distance)
        float predictionFactor = Mathf.Clamp01((float)currentScore / scoreForMaxDifficulty);
        
        // Get player's current position and velocity
        Vector3 playerPosition = player != null ? player.position : Vector3.zero;
        Vector3 playerVelocity = playerController != null ? playerController.GetMovementDirection() : Vector3.zero;
        
        // Get current ball speed from score manager
        float ballSpeed = DodgeballScoreManager.Instance != null 
            ? DodgeballScoreManager.Instance.GetCurrentBallSpeed() 
            : 8f;
        
        // Calculate time for ball to reach player's current position
        float distanceToPlayer = Vector3.Distance(throwingEnemy.position, playerPosition);
        float timeToReach = distanceToPlayer / ballSpeed;
        
        // Calculate predicted position based on player velocity and prediction factor
        Vector3 fullPredictionOffset = playerVelocity * playerController.maxSpeed * timeToReach;
        
        // Clamp prediction to maximum distance
        if (fullPredictionOffset.magnitude > maxPredictionDistance)
        {
            fullPredictionOffset = fullPredictionOffset.normalized * maxPredictionDistance;
        }
        
        // Apply prediction factor to get final offset
        Vector3 predictionOffset = fullPredictionOffset * predictionFactor;
        Vector3 targetPosition = playerPosition + predictionOffset;
        
        // Add ball behavior component
        EnemyBallBehavior ballBehavior = ball.AddComponent<EnemyBallBehavior>();
        ballBehavior.Initialize(throwingEnemy.position, targetPosition, ballSpeed, hitRadius, playerController);
        
        // Destroy ball after lifetime to prevent buildup
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