// Created by Daniil Makarenko

using UnityEngine;
using System.Collections;

public class DodgeballTutorialManager : MonoBehaviour
{
    // Singleton
    public static DodgeballTutorialManager Instance { get; private set; }
    
    // References
    [Header("References")]
    public DodgeballEnemyManager enemyManager;
    public DodgeballPlayerController playerController;
    public DodgeballFriendlyBallSpawner friendlyBallSpawner;
    public Transform player;
    public GameObject enemyBallPrefab;
    public GameObject friendlyBallPrefab;
    
    [Header("UI References")]
    public GameObject wasdSymbol; // WASD indicator sprite
    public GameObject clickSymbol; // Left click indicator sprite
    public GameObject wasdSymbolAlt; // Alternate WASD sprite (for flashing effect)
    
    [Header("Tutorial Settings")]
    public float tutorialBallSlowdownDistance = 3f; // Distance at which ball starts slowing
    public float tutorialBallMinSpeed = 1f; // Minimum speed when close to player
    public float tutorialBallStopDistance = 1.5f; // Distance at which ball stops
    public float reducedSlowdownDistance = 1.5f; // Slowdown distance after player moves
    public float reducedStopDistance = 0.8f; // Stop distance after player moves
    public float wasdSymbolDelay = 0.5f; // Delay before WASD appears
    public float wasdFlashInterval = 0.3f; // How fast WASD flashes
    public Vector3 wasdSymbolOffset = new Vector3(2f, 0f, 0f); // Offset from player
    public Vector3 clickSymbolOffset = new Vector3(0f, 1.5f, 0f); // Offset above ball
    
    // Tutorial state
    private bool tutorialCompleted = false;
    private bool isInTutorial = false;
    private bool enemyBallThrown = false;
    private bool playerMoved = false;
    private bool friendlyBallThrown = false;
    private GameObject tutorialEnemyBall;
    private GameObject tutorialFriendlyBall;
    private bool wasdSymbolActive = false;
    private float lastWasdFlashTime;
    private bool wasdVisible = true;
    
    void Awake()
    {
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
    // Try to find references if not set
    if (enemyManager == null)
        enemyManager = FindFirstObjectByType<DodgeballEnemyManager>();
    
    if (playerController == null)
        playerController = FindFirstObjectByType<DodgeballPlayerController>();
    
    if (friendlyBallSpawner == null)
        friendlyBallSpawner = FindFirstObjectByType<DodgeballFriendlyBallSpawner>();
    
    if (player == null && playerController != null)
        player = playerController.transform;
    
    // Hide UI elements by disabling their renderers, not the GameObjects
    HideRenderers(wasdSymbol);
    HideRenderers(wasdSymbolAlt);
    HideRenderers(clickSymbol);
    
    // Check if tutorial should run
    // Tutorial flag persists across score resets but not scene reloads
    if (!tutorialCompleted)
    {
        StartTutorial();
    }
}

// Helper method to hide all renderers in a GameObject
void HideRenderers(GameObject obj)
{
    if (obj == null)
        return;
    
    Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
    foreach (Renderer renderer in renderers)
    {
        renderer.enabled = false;
    }
}

// Helper method to show all renderers in a GameObject
void ShowRenderers(GameObject obj)
{
    if (obj == null)
        return;
    
    Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
    foreach (Renderer renderer in renderers)
    {
        renderer.enabled = true;
    }
}
    
    void Update()
    {
        if (!isInTutorial)
            return;
        
        // Update WASD symbol flashing
        if (wasdSymbolActive && wasdSymbol != null)
        {
            UpdateWASDFlashing();
        }
        
        // Check if player moved
        if (enemyBallThrown && !playerMoved)
        {
            CheckPlayerMovement();
        }
    }
    
    void StartTutorial()
    {
        isInTutorial = true;
        
        // Disable normal gameplay systems
        if (enemyManager != null)
            enemyManager.enabled = false;
        
        if (friendlyBallSpawner != null)
            friendlyBallSpawner.enabled = false;
        
        // Start tutorial sequence
        StartCoroutine(TutorialSequence());
    }
    
    IEnumerator TutorialSequence()
    {
        // Wait a moment for everything to initialize
        yield return new WaitForSeconds(1f);
        
        // Step 1: Throw tutorial enemy ball
        ThrowTutorialEnemyBall();
        
        // Wait before showing WASD
        yield return new WaitForSeconds(wasdSymbolDelay);
        
        // Show WASD symbol
        ShowWASDSymbol();
        
        // Wait for player to move
        yield return new WaitUntil(() => playerMoved);
        
        // Hide WASD symbol
        HideWASDSymbol();
        
        // Wait for enemy ball to pass z = -8
        yield return new WaitUntil(() => tutorialEnemyBall == null || tutorialEnemyBall.transform.position.z < -8f);
        
        // Step 2: Spawn tutorial friendly ball
        SpawnTutorialFriendlyBall();
        
        // Wait for player to throw the ball
        yield return new WaitUntil(() => friendlyBallThrown);
        
        // Tutorial complete!
        CompleteTutorial();
    }
    
    void ThrowTutorialEnemyBall()
{
    if (enemyBallPrefab == null || enemyManager == null || enemyManager.enemyPositions == null)
        return;
    
    // Get middle enemy position (index 3 for 7 enemies)
    int middleIndex = enemyManager.enemyPositions.Length / 2;
    Transform middleEnemy = enemyManager.enemyPositions[middleIndex];
    
    if (middleEnemy == null)
        return;
    
    // Create ball at middle enemy position
    tutorialEnemyBall = Instantiate(enemyBallPrefab, middleEnemy.position, enemyBallPrefab.transform.rotation);
    
    // Add tutorial ball behavior
    TutorialEnemyBallBehavior behavior = tutorialEnemyBall.AddComponent<TutorialEnemyBallBehavior>();
    behavior.Initialize(
        middleEnemy.position,
        player.position,
        8f, // Normal speed
        tutorialBallSlowdownDistance,
        tutorialBallMinSpeed,
        tutorialBallStopDistance,
        reducedSlowdownDistance,
        reducedStopDistance,
        playerController
    );
    
    enemyBallThrown = true;
}
    
    void ShowWASDSymbol()
{
    if (player == null)
        return;
    
    wasdSymbolActive = true;
    
    ShowRenderers(wasdSymbol);
    HideRenderers(wasdSymbolAlt);
    
    lastWasdFlashTime = Time.time;
    wasdVisible = true;
}
    
    void HideWASDSymbol()
{
    wasdSymbolActive = false;
    
    HideRenderers(wasdSymbol);
    HideRenderers(wasdSymbolAlt);
}
    
    void UpdateWASDFlashing()
{
    // Position both WASD symbols relative to player
    if (wasdSymbol != null)
        wasdSymbol.transform.position = player.position + wasdSymbolOffset;
    
    if (wasdSymbolAlt != null)
        wasdSymbolAlt.transform.position = player.position + wasdSymbolOffset;
    
    // Alternate between the two symbols
    if (Time.time - lastWasdFlashTime >= wasdFlashInterval)
    {
        wasdVisible = !wasdVisible;
        lastWasdFlashTime = Time.time;
        
        // Toggle between the two symbols
        if (wasdVisible)
        {
            ShowRenderers(wasdSymbol);
            HideRenderers(wasdSymbolAlt);
        }
        else
        {
            HideRenderers(wasdSymbol);
            ShowRenderers(wasdSymbolAlt);
        }
    }
}
    
    void CheckPlayerMovement()
    {
        // Check if player has given any movement input through the player controller
        if (playerController != null && playerController.Rigidbody != null)
        {
            // Check if player's velocity has changed (indicating movement input)
            if (playerController.Rigidbody.linearVelocity.magnitude > 0.1f)
            {
                playerMoved = true;
            }
        }
    }
    
    void SpawnTutorialFriendlyBall()
{
    if (friendlyBallPrefab == null || player == null || friendlyBallSpawner == null)
        return;
    
    // Use the same spawn logic as the normal spawner
    float randomX = Random.Range(friendlyBallSpawner.spawnLineMinX, friendlyBallSpawner.spawnLineMaxX);
    Vector3 spawnPosition = new Vector3(randomX, friendlyBallSpawner.spawnHeight, friendlyBallSpawner.spawnLineZ);
    
    // Create ball at spawn position
    tutorialFriendlyBall = Instantiate(friendlyBallPrefab, spawnPosition, friendlyBallPrefab.transform.rotation);
    
    // Get the friendly ball component and set it up
    DodgeballFriendlyBall ballScript = tutorialFriendlyBall.GetComponent<DodgeballFriendlyBall>();
    if (ballScript != null)
    {
        ballScript.playerController = playerController;
        ballScript.player = player;
        ballScript.enemyManager = enemyManager;
        
        // Make it a tutorial ball (doesn't time out)
        ballScript.MakeTutorialBall();
        
        // Initialize with normal flight
        ballScript.Initialize(spawnPosition);
    }
}
    
    public void ShowClickSymbol(Vector3 ballPosition)
{
    if (clickSymbol != null && !friendlyBallThrown)
    {
        ShowRenderers(clickSymbol);
        clickSymbol.transform.position = ballPosition + clickSymbolOffset;
    }
}
    
    public void HideClickSymbol()
{
    if (clickSymbol != null)
    {
        HideRenderers(clickSymbol);
    }
}
    
    public void OnTutorialBallThrown()
    {
        friendlyBallThrown = true;
        HideClickSymbol();
    }
    
    void CompleteTutorial()
    {
        tutorialCompleted = true;
        isInTutorial = false;
        
        // Re-enable normal gameplay systems
        if (enemyManager != null)
            enemyManager.enabled = true;
        
        if (friendlyBallSpawner != null)
            friendlyBallSpawner.enabled = true;
    }
    
    public bool IsTutorialActive()
    {
        return isInTutorial;
    }
    
    public bool IsTutorialBall(GameObject ball)
    {
        return ball == tutorialFriendlyBall;
    }
    
// Inner class for tutorial enemy ball behavior
private class TutorialEnemyBallBehavior : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 direction;
    private float normalSpeed;
    private float slowdownDistance;
    private float minSpeed;
    private float stopDistance;
    private float reducedSlowdownDistance;
    private float reducedStopDistance;
    private DodgeballPlayerController playerController;
    private bool isInitialized = false;
    private bool playerHasMoved = false;
    
    public void Initialize(Vector3 start, Vector3 target, float speed, float slowDist, float minSpd, float stopDist, float redSlowDist, float redStopDist, DodgeballPlayerController player)
    {
        startPosition = start;
        targetPosition = target;
        normalSpeed = speed;
        slowdownDistance = slowDist;
        minSpeed = minSpd;
        stopDistance = stopDist;
        reducedSlowdownDistance = redSlowDist;
        reducedStopDistance = redStopDist;
        playerController = player;
        
        // Calculate direction
        direction = (targetPosition - startPosition).normalized;
        direction.y = 0f;
        direction.Normalize();
        
        isInitialized = true;
    }
    
    void Update()
    {
        if (!isInitialized || playerController == null)
            return;
        
        // Check if player has moved (reduces slowdown range)
        if (!playerHasMoved && playerController.Rigidbody != null)
        {
            if (playerController.Rigidbody.linearVelocity.magnitude > 0.1f)
            {
                playerHasMoved = true;
            }
        }
        
        // Use reduced distances after player moves
        float currentSlowdownDistance = playerHasMoved ? reducedSlowdownDistance : slowdownDistance;
        float currentStopDistance = playerHasMoved ? reducedStopDistance : stopDistance;
        
        // Calculate distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, playerController.transform.position);
        
        // Determine speed based on distance
        float currentSpeed;
        if (distanceToPlayer <= currentStopDistance)
        {
            currentSpeed = 0f; // Stop completely
        }
        else if (distanceToPlayer <= currentSlowdownDistance)
        {
            // Lerp between min speed and normal speed
            float t = (distanceToPlayer - currentStopDistance) / (currentSlowdownDistance - currentStopDistance);
            currentSpeed = Mathf.Lerp(minSpeed, normalSpeed, t);
        }
        else
        {
            currentSpeed = normalSpeed;
        }
        
        // Move ball
        transform.position += direction * currentSpeed * Time.deltaTime;
        
        // Destroy if goes too far
        if (transform.position.z < -15f)
        {
            Destroy(gameObject);
        }
    }
}
}