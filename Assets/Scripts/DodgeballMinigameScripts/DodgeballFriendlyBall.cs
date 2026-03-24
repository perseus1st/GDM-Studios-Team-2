// Created by Daniil Makarenko

using UnityEngine;

public class DodgeballFriendlyBall : MonoBehaviour
{
    // References
    [Header("References")]
    public DodgeballPlayerController playerController; // Reference to player
    public Transform player; // Reference to player position
    public DodgeballEnemyManager enemyManager; // Reference to enemy manager for positions
    public GameObject glowEffect; // Glow sprite for when to collect
    // public AudioSource landSound; // Sound when ball lands
    // public AudioSource collectSound; // Sound when collecting ball
    // public AudioSource throwSound; // Sound when throwing ball
    
    // Ball flight settings
    [Header("Flight Settings")]
    public float minArcHeight = 1f; // Minimum arc height (close to back wall)
    public float maxArcHeight = 3f; // Maximum arc height (far from back wall)
    public float flightSpeed = 8f; // How fast ball travels
    public float thrownBallSpeed = 15f; // How fast ball travels when thrown at enemy
    
    // Landing settings
    [Header("Landing Settings")]
    public float maxDistanceFromSpawn = 8f; // How far ball can land from spawn point
    public float backWallZ = -9f; // Z position of back wall
    
    // Collection settings
    [Header("Collection Settings")]
    public float collectRange = 2f; // How close player needs to be to collect
    public float groundedWindow = 3f; // How long ball stays on ground before disappearing
    public float flashDuration = 1f; // How long ball flashes before disappearing
    public float flashInterval = 0.1f; // How fast ball flashes
    
    // Throw settings
    [Header("Throw Settings")]
    public float throwDelay = 0.5f; // Delay between collect and throw (animation stand-in)
    public float enemyHitRadius = 0.5f; // How close to enemy counts as hit
    
    // Ball states
    private Vector3 startPosition; // Where flight starts
    private Vector3 targetPosition; // Where ball will land
    private float journeyLength; // Total flight distance
    private float journeyProgress; // How far along flight (0 to 1)
    private bool isFlying; // Is ball in flight
    private bool isGrounded; // Is ball on ground waiting for pickup
    private float landedTime; // Time when ball landed
    private bool canBeCollected; // Can player collect the ball
    private Vector3 originalScale; // Original scale of the ball
    private bool isBeingThrown; // Is player in process of throwing this ball
    private float throwStartTime; // When throw sequence started
    private bool isVisible = true; // Current visibility for flashing
    private float lastFlashTime; // Last time flash toggled
    private float currentArcHeight; // Arc height for this specific ball
    private bool isFlyingToEnemy; // Is ball flying toward enemy after being thrown
    private Vector3 enemyTarget; // Which enemy we're targeting
    private Vector3 flyDirection; // Direction ball is flying
    private float currentSpeed; // Current flight speed
    private bool isTutorialBall = false; // For tutorial
    
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
        
        if (enemyManager == null)
        {
            enemyManager = FindFirstObjectByType<DodgeballEnemyManager>();
        }
        
        // Store original scale
        originalScale = transform.localScale;
        
        // Glow starts disabled
        if (glowEffect != null)
            glowEffect.SetActive(false);
        
        // Don't reset states here - they're set by Initialize()
        // Initialize() is called before Start() by the spawner
    }
    
    // Called by spawner to initialize the ball's flight
    public void Initialize(Vector3 spawnPosition)
    {
        startPosition = spawnPosition;
        
        // Pick random landing spot on player's side of court
        // Adjust court boundaries as needed
        float randomX = Random.Range(-9f, 9f);
        float randomZ = Random.Range(-9f, -1f);
        
        // Clamp distance from spawn
        Vector3 randomTarget = new Vector3(randomX, 0.5f, randomZ);
        Vector3 directionFromSpawn = randomTarget - startPosition;
        if (directionFromSpawn.magnitude > maxDistanceFromSpawn)
        {
            directionFromSpawn = directionFromSpawn.normalized * maxDistanceFromSpawn;
            randomTarget = startPosition + directionFromSpawn;
        }
        
        targetPosition = randomTarget;
        targetPosition.y = 0.5f;
        
        // Calculate arc height based on distance from back wall
        // Closer to back wall = lower arc, farther = higher arc
        float distanceFromBackWall = Mathf.Abs(targetPosition.z - backWallZ);
        float maxPossibleDistance = Mathf.Abs(-1f - backWallZ); // Distance from back wall to net (-1 to -9)
        float arcT = Mathf.Clamp01(distanceFromBackWall / maxPossibleDistance);
        currentArcHeight = Mathf.Lerp(minArcHeight, maxArcHeight, arcT);
        
        // Calculate flight distance
        journeyLength = Vector3.Distance(startPosition, targetPosition);
        
        // Set initial speed
        currentSpeed = flightSpeed;
        
        // Reset flight and start
        journeyProgress = 0f;
        isFlying = true;
        isGrounded = false;
        canBeCollected = false;
        isFlyingToEnemy = false;
        
        Debug.Log($"FriendlyBall initialized - Flying: {isFlying}, Start: {startPosition}, Target: {targetPosition}, Journey: {journeyLength}, Arc Height: {currentArcHeight}");
    }
    
    void Update()
    {
        if (isFlying)
        {
            UpdateFlight();
        }
        else if (isGrounded && !isBeingThrown)
        {
            UpdateGroundedState();
        }
        else if (isBeingThrown)
        {
            UpdateThrowSequence();
        }
        else if (isFlyingToEnemy)
        {
            UpdateEnemyFlight();
        }
    }
    
    // Update ball position during flight
    void UpdateFlight()
    {
        // Avoid division by zero
        if (journeyLength <= 0.001f)
        {
            LandBall();
            return;
        }
        
        // Increase progress
        journeyProgress += (currentSpeed / journeyLength) * Time.deltaTime;
        
        // Calculate position
        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, journeyProgress);
        
        // Calculate arc height using the specific arc height for this ball
        float arcProgress = 4f * journeyProgress * (1f - journeyProgress);
        float currentArcHeightValue = arcProgress * currentArcHeight;
        
        // Scale sprite to simulate arc
        float scaleMultiplier = 1f + currentArcHeightValue;
        transform.localScale = originalScale * scaleMultiplier;
        
        // Apply position
        transform.position = currentPos;
        
        // Check if flight is over
        if (journeyProgress >= 1f)
        {
            LandBall();
        }
    }
    
    // Ball has landed on ground
    void LandBall()
    {
        // Flight over
        isFlying = false;
        journeyProgress = 0f;
        
        // Reset scale and position
        transform.localScale = originalScale;
        transform.position = targetPosition;
        
        // Play land sound
        // if (landSound != null)
        //     landSound.Play();
        DodgeballAudioManager.INSTANCE.PlaySFX("HitFloor");
        
        // Ball is now grounded
        isGrounded = true;
        canBeCollected = true;
        landedTime = Time.time;
    }
    
    // Update grounded state - check for collection or timeout
    // Update grounded state - check for collection or timeout
void UpdateGroundedState()
{
    float timeSinceLanded = Time.time - landedTime;
    
    // Tutorial ball never times out
    if (isTutorialBall)
    {
        // Check if player is close enough to collect
        if (canBeCollected && player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            bool inRange = distance <= collectRange;
            
            // Show/hide glow based on range
            if (glowEffect != null)
                glowEffect.SetActive(inRange);
            
            // Show click symbol if in range and tutorial is active
            if (inRange && DodgeballTutorialManager.Instance != null)
            {
                DodgeballTutorialManager.Instance.ShowClickSymbol(transform.position);
            }
            else if (DodgeballTutorialManager.Instance != null)
            {
                DodgeballTutorialManager.Instance.HideClickSymbol();
            }
            
            // Check if player pressed interact
            if (inRange && playerController != null && playerController.GetInteractPressed())
            {

                // Hide click symbol immediately when player clicks
                if (DodgeballTutorialManager.Instance != null)
                {
                    DodgeballTutorialManager.Instance.HideClickSymbol();
                }

                CollectBall();
                playerController.ConsumeInteract();
            }
        }
        return; // Don't run normal timeout logic
    }
    
    // Normal (non-tutorial) ball behavior
    // Check if player is close enough to collect
    if (canBeCollected && player != null)
    {
        float distance = Vector3.Distance(player.position, transform.position);
        bool inRange = distance <= collectRange;
        
        // Show/hide glow based on range (only if not flashing)
        if (timeSinceLanded <= groundedWindow)
        {
            if (glowEffect != null)
                glowEffect.SetActive(inRange);
        }
        
        // Check if player pressed interact
        if (inRange && playerController != null && playerController.GetInteractPressed())
        {
            CollectBall();
            playerController.ConsumeInteract();
        }
    }
    
    // Start flashing after grounded window
    if (timeSinceLanded > groundedWindow)
    {
        UpdateFlash();
    }
    
    // Destroy after flash duration ends
    if (timeSinceLanded > (groundedWindow + flashDuration))
    {
        Destroy(gameObject);
    }
}
    
    // Flash ball before it disappears
void UpdateFlash()
{
    if (Time.time - lastFlashTime >= flashInterval)
    {
        isVisible = !isVisible;
        lastFlashTime = Time.time;
        
        // Toggle visibility of ball
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = isVisible;
        }
        
        // Only flash glow if it was already active (player is in range)
        if (glowEffect != null)
        {
            // Check if player is in range
            bool inRange = false;
            if (player != null)
            {
                float distance = Vector3.Distance(player.position, transform.position);
                inRange = distance <= collectRange;
            }
            
            // Only show glow if visible AND player in range
            glowEffect.SetActive(isVisible && inRange);
        }
    }
}
    
    // Player collected the ball
    void CollectBall()
    {
        // Play collect sound
        // if (collectSound != null)
        //     collectSound.Play();
        
        // Disable glow
        if (glowEffect != null)
            glowEffect.SetActive(false);

        // Ensure ball is visible (in case it was collected while flashing)
        Renderer renderer = GetComponentInChildren<Renderer>();
        renderer.enabled = true;
        isVisible = true;
        
        // Ball is no longer grounded
        isGrounded = false;
        canBeCollected = false;
        
        // Start throw sequence
        isBeingThrown = true;
        throwStartTime = Time.time;

        if (playerController.GetComponentInChildren<Animator>() != null)
            playerController.GetComponentInChildren<Animator>().SetTrigger("Throw");
        
        // Lock player movement
        if (playerController != null)
            playerController.isThrowing = true;
        
    }
    
    // Update throw sequence - wait for delay then throw
    void UpdateThrowSequence()
    {
        float throwProgress = Time.time - throwStartTime;
        
        // Check if player was hit during throw
        if (playerController != null && playerController.IsInvincible())
        {
            // Player was hit - cancel throw and destroy ball
            // Unlock player movement
            if (playerController != null)
                playerController.isThrowing = false;
            
            Destroy(gameObject);
            return;
        }
        
        // Check if throw delay complete
        if (throwProgress >= throwDelay)
        {
            ThrowAtEnemy();
        }
    }
    
    // Throw ball at nearest enemy
    void ThrowAtEnemy()
{
    // Play throw sound
    // if (throwSound != null)
    //     throwSound.Play();
    DodgeballAudioManager.INSTANCE.PlaySFX("Throw");
    
    // Unlock player movement
    if (playerController != null)
        playerController.isThrowing = false;
    
    // Notify tutorial if this is tutorial ball
    if (isTutorialBall && DodgeballTutorialManager.Instance != null)
    {
        DodgeballTutorialManager.Instance.OnTutorialBallThrown();
    }
    
    // Find nearest enemy
    Transform nearestEnemy = FindNearestEnemy();
    
    if (nearestEnemy == null)
    {
        // No enemies found, just destroy ball
        Destroy(gameObject);
        return;
    }
    
    // Set up flight to enemy
    enemyTarget = nearestEnemy.position;
    flyDirection = (enemyTarget - transform.position).normalized;
    flyDirection.y = 0f; // Keep horizontal
    flyDirection.Normalize();
    
    // Start flying to enemy
    isFlyingToEnemy = true;
    isBeingThrown = false;
    currentSpeed = thrownBallSpeed;
}
    
    // Find the nearest enemy to throw at
    Transform FindNearestEnemy()
{
    if (enemyManager == null || enemyManager.enemyPositions == null)
        return null;

    Transform nearest = null;
    float nearestDistance = Mathf.Infinity;

    for (int i = 0; i < enemyManager.enemyPositions.Length; i++)
    {
        Transform enemy = enemyManager.enemyPositions[i];
        if (enemy != null && !enemyManager.IsEnemyStunned(i))
        {
            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = enemy;
            }
        }
    }

    return nearest;
}
    
    // Update ball flying toward enemy
    void UpdateEnemyFlight()
    {
        // Move ball in straight line toward enemy
        transform.position += flyDirection * currentSpeed * Time.deltaTime;
        
        // Check if hit any enemy
        if (CheckEnemyHit())
        {
            // Hit enemy - add score and destroy ball
            if (DodgeballScoreManager.Instance != null)
                DodgeballScoreManager.Instance.AddScore();
            
            Destroy(gameObject);
            return;
        }
        
        // Destroy if ball goes too far off screen
        if (transform.position.z > 15f || Mathf.Abs(transform.position.x) > 15f)
        {
            Destroy(gameObject);
        }
    }
    
    bool CheckEnemyHit()
{
    if (enemyManager == null || enemyManager.enemyPositions == null)
        return false;

    foreach (Transform enemy in enemyManager.enemyPositions)
    {
        if (enemy != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance <= enemyHitRadius)
            {
                enemyManager.StunEnemy(enemy, 0.7f);
                return true;
            }
        }
    }

    return false;
}
    
    // Called when player is hit - cancel throw if in progress
    public void CancelThrow()
    {
        if (isBeingThrown)
        {
            // Unlock player movement
            if (playerController != null)
                playerController.isThrowing = false;
            
            Destroy(gameObject);
        }
    }

    // Tutorial throw
    // Make this ball a tutorial ball (doesn't time out, shows click symbol)
    public void MakeTutorialBall()
    {
         isTutorialBall = true;
    } 
    
    // Visual debug in editor
    void OnDrawGizmosSelected()
    {
        // Draw collect range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectRange);
        
        // Draw max distance from spawn if we have a start position
        if (isFlying || isGrounded)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(startPosition, maxDistanceFromSpawn);
        }
        
        // Draw enemy hit radius
        if (isFlyingToEnemy)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyHitRadius);
            Gizmos.DrawLine(transform.position, transform.position + flyDirection * 2f);
        }
    }
}