// Created by Daniil Makarenko

using UnityEngine;

public class BirdieController : MonoBehaviour
{
    // References
    [Header("References")]
    public Transform player; // Reference player position
    public BadmintonPlayerController playerController; // Reference player controller for hits
    public Transform opponent; // Reference sister's position
    public GameObject glowEffect; // Glow sprite for when to hit
    public AudioSource hitSound; // Hit birdie sound
    public GameObject targetIndicator; // Shows where birdie will land

    // Birdie movement
    [Header("Movement Settings")]
    public float arcHeight = 2f; // height of arc

    // Hit detection
    [Header("Hit Settings")]
    public float hitRange = 2f; // How close player has to be to hit birdie in units
    public float hitWindowStart = 0.9f; // What percentage into flight the player can hit birdie

    [Header("Tutorial Settings")]
    public bool isTutorial = true; // Need tutorial to happen
    public GameObject tutorialMoveIcon; // WASD image
    public Sprite tutorialMoveFrame1; // WASD image no glow
    public Sprite tutorialMoveFrame2; // WASD image with glow
    public GameObject tutorialClickIcon; // Interact image
    public float tutorialIconFlashSpeed = 0.5f; // How fast flashes in seconds
    public Vector3 tutorialTargetOffset = new Vector3(2f, 0.5f, -5f); // Where placed relative to player
    public Vector3 tutorialMoveIconOffset = new Vector3(0, 0.5f, 2.5f); // WASD icon position relative to target reticle
    public Vector3 tutorialClickIconOffset = new Vector3(1f, 0.5f, 2.5f); // Interact icon position relative to target reticle

    private bool tutorialCompleted = false; // Is tutorial over
    private float tutorialFlashTimer = 0f; // Tracks how long ago icons flashed in tutorial
    private bool tutorialShowingFrame1 = true;

    // Controls for how the shot types work
    [Header("Shot Type Settings")]
    public float normalShotMinDistanceAtLowScore = 1f; // Easy minimum distance from player
    public float normalShotMinDistanceAtHighScore = 4f; // Hard minimum distance from player
    public float normalShotMaxDistanceAtLowScore = 3f; // Easy maximum distance from player 
    public float normalShotMaxDistanceAtHighScore = 8f; // Hard maximum distance from player
    public float normalShotExtraTimeAtLowScore = 2f;  // Extra time for players to reach the normal shot
    public float normalShotExtraTimeAtHighScore = 0.3f;
    public float dropShotNetDistance = 2f;  // Distance from net
    public float dropShotArcMultiplier = 2f; // How much higher the drop shot arc is
    public float dropShotExtraTimeAtLowScore = 2f;  // Gives extra time for player to reach drop shot
    public float dropShotExtraTimeAtHighScore = 0.3f;
    public float longShotBackDistance = 2f; // Distance from back wall
    public float longShotArcMultiplier = 0.5f; // How much smaller the long shot arc is
    public float longShotExtraTimeAtLowScore = 2f; // Gives player extra time to hit long shot
    public float longShotExtraTimeAtHighScore = 0.3f;   
    public int scoreForFullDistribution = 30;   // Maximum difficulty at this score


    // Birdie states
    private Vector3 startPosition; // Where flight starts
    private Vector3 targetPosition; // Where birdie will end
    private float journeyLength; // total flight distance
    private float journeyProgress; // How far along flight (0 to 1)
    private bool isFlying; // Is birdie in flight?
    private bool isOnPlayerSide; // Which side of net is the birdie on
    private bool canBeHit; // Is the player allowd to hit the birdie right now
    private Vector3 originalScale; // Original scale of the birdie
    private float landedTime; // Time when flight over 
    private bool isGrounded; // Is flight done but birdie not hit yet
    private float currentFlightSpeed; // How fast birdie flying for this specific shot
    private float currentArcHeight; // How high birdie flying for this specific shot
    private int consecutiveHits = 0; // For tracking how much extra time to give the player after they miss



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store original scale
        originalScale = transform.localScale;

        // Glow starts disabled
        if (glowEffect != null)
            glowEffect.SetActive(false);

        // Hide target indicator
        if (targetIndicator != null)
            targetIndicator.SetActive(false);

        // Hide tutorial icons
        if (tutorialMoveIcon != null)
            tutorialMoveIcon.SetActive(false);
        if (tutorialClickIcon != null)
            tutorialClickIcon.SetActive(false);

        // Birdie starts on opponent side
        isOnPlayerSide = false;
        canBeHit = false;
        isFlying = false;

        // Start first serve after a delay just for testing
        Invoke("ServeToPlayer", 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        // Checks for birdie side (z<0 = player's side)
        isOnPlayerSide = transform.position.z < 0f;

        // Reset hit window
        bool inHitWindow = false;
        
            // Regular landing behaviour
            if (isFlying && isOnPlayerSide)
            {
                // Can hit during last part of flight
                inHitWindow = journeyProgress >= hitWindowStart;
            }
            else if (isGrounded && isOnPlayerSide)
            {
                // Gives infinite time to hit birdie during tutorial
                if (isTutorial && !tutorialCompleted)
                {
                    inHitWindow = true;  // Always allow hitting during tutorial
                    UpdateTutorialIcons();  // Show/update tutorial icons
                }
                else
                {
    
                    // Get grounded window from ScoreManager with fallback
                    float currentGroundedWindow = ScoreManager.Instance != null ? ScoreManager.Instance.GetCurrentGroundedWindow() : 0.5f; 
    
                    // can hit for short time after landing
                    float timeSinceLanded = Time.time - landedTime;

		    // Pause timer if player is in hit animation
                    bool playerInHitAnimation = playerController != null && playerController.IsInHitAnimation();
                    if (playerInHitAnimation)
                    {
                        // Freeze the timer by updating landedTime
                        landedTime = Time.time;
                        timeSinceLanded = 0f;
                    }

                    inHitWindow = timeSinceLanded <= currentGroundedWindow;
                
                    // lose life and re-serve if on ground for too long
                    if (timeSinceLanded > currentGroundedWindow)
                    {
                        if (ScoreManager.Instance != null)
                            ScoreManager.Instance.LoseLife();
                            consecutiveHits = 0; // Reset consecutive hits tracker
                            ResetAndServe();
                    }
                }
             }

	// Update tutorial icons throughout the entire tutorial
        if (isTutorial && isFlying && !tutorialCompleted)
        {
            UpdateTutorialIcons();
        }

        // Check if player close enough to hit and within hit window
        if (inHitWindow && player != null)
        {
            // calculate player-birdie distance
            float distance = Vector3.Distance(player.position, transform.position);

            // Is player close enough
            canBeHit = distance <= hitRange;

            // Show/hide glow if can hit
            if (glowEffect != null)
                glowEffect.SetActive(canBeHit);
        }
        else
        {
            // Not allowed to hit
            canBeHit = false;
            if (glowEffect != null)
                glowEffect.SetActive(false);
        }

        // Check if player clicked
        if (playerController != null && playerController.GetHitPressed())
        {
            // Check if in allowed to hit
            if (canBeHit)
            {
                HitBirdieToOpponent();

                // consume the hit
                playerController.ConsumeHit();
            }
        }

        if (isFlying)
        {
            UpdateFlight();
        }
    }

    // When player hits birdie
    void HitBirdieToOpponent()
    {
        // play hit sound
        if (hitSound != null)
            hitSound.Play();

        // Finish tutorial
        if (isTutorial && !tutorialCompleted)
        {
            tutorialCompleted = true;
            if (tutorialMoveIcon != null)
                tutorialMoveIcon.SetActive(false);
            if (tutorialClickIcon != null)
                tutorialClickIcon.SetActive(false);
        }

        // Increment score
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore();

        // Increment consecutive hits tracker
        consecutiveHits++;

        // Remove target indicator
        if (targetIndicator != null)
        targetIndicator.SetActive(false);

	// Reset rotation before flight
        transform.rotation = Quaternion.identity;

        // Get opponent position
        startPosition = transform.position;

        // Aim for opponent with middle of opponent play area as backup
        if (opponent != null)
            targetPosition = opponent.position + new Vector3(-1.65f, 0f, -0.1f);
        else
            targetPosition = new Vector3(0f, 0.5f, 5f);

        // Calcualte distance for flight
        journeyLength = Vector3.Distance(startPosition, targetPosition);

        // Use base values for the player shot with base speed as backup
        currentFlightSpeed = ScoreManager.Instance != null 
            ? ScoreManager.Instance.GetCurrentFlightSpeed() 
            : 8f;
        currentArcHeight = arcHeight;

        // Reset flight and start new flight
        journeyProgress = 0f;
        isFlying = true;
        isOnPlayerSide = false;
        isGrounded = false;
        canBeHit = false;

        // immediately disable glow
        if (glowEffect != null)
            glowEffect.SetActive(false);
    }

    // Opponent hits birdie
    void ServeToPlayer()
    {
        // play hit sound
        if (hitSound != null)
            hitSound.Play();

	// Move opponent to random position
        if (opponent != null)
        {
            OpponentController opponentController = opponent.GetComponent<OpponentController>();
            if (opponentController != null)
            {
                opponentController.PlayHitAnimation();
                opponentController.MoveToRandomPosition();
            }
        }
      
        // Set flight start
        startPosition = transform.position;

	// Reset rotation before flight
        transform.rotation = Quaternion.identity;

        // Do the tutorial
        if (isTutorial && !tutorialCompleted)
        {
            ServeTutorialShot();
        }
        else
        {
            // Calculate probabilities of each shot type based on score
            int currentScore = ScoreManager.Instance != null ? ScoreManager.Instance.GetScore() : 0;
            float normalChance = GetNormalShotChance(currentScore);
            float dropChance = GetDropShotChance(currentScore);
    
            // Select shot type randomly
            float roll = Random.Range(0f, 1f);
    
            if (roll < normalChance)
            {
                ServeNormalShot(currentScore);
            }
            else if (roll < normalChance + dropChance)
            {
                ServeDropShot(currentScore);
            }
            else
            {
                ServeLongShot(currentScore);
            }
        }

        // Calculate flight distance
        journeyLength = Vector3.Distance(startPosition, targetPosition);

        // Reset flight and start new flight
        journeyProgress = 0f;
        isFlying = true;
        isOnPlayerSide = false;
        isGrounded = false;

        // Show where the birdie will land
        if (targetIndicator != null)
        {
            targetIndicator.transform.position = targetPosition;
            targetIndicator.SetActive(true);
        }

        // Show tutorial icons immediately during tutorial
        if (isTutorial && !tutorialCompleted)
        {
            if (tutorialMoveIcon != null)
            {
                tutorialMoveIcon.transform.position = targetPosition + tutorialMoveIconOffset;
                tutorialMoveIcon.SetActive(true);
            }
            if (tutorialClickIcon != null)
            {
                tutorialClickIcon.transform.position = targetPosition + tutorialClickIconOffset;
                tutorialClickIcon.SetActive(false);
            }
        }
    }

    // Normal shot goes to a random location near player
    void ServeNormalShot(int score)
    {
        // Calculate distance away from player based on score
        float t = Mathf.Clamp01((float)score / scoreForFullDistribution);
        float maxDistance = Mathf.Lerp(normalShotMaxDistanceAtLowScore, normalShotMaxDistanceAtHighScore, t);
        float minDistance = Mathf.Lerp(normalShotMinDistanceAtLowScore, normalShotMinDistanceAtHighScore, t);
        
        // Pick random location
        float randomDistance = Random.Range(minDistance, maxDistance);
        float randomAngle = Random.Range(0f, 360f);
        
        // Calculate offset that will be applied to player position
        Vector3 offset = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance,
            0f,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance
        );
        
        // Apply offset and clamp to be inside play area
        // Will need to adjust manually if court size changes
        Vector3 targetPos = player.position + offset;
        targetPos.x = Mathf.Clamp(targetPos.x, -8f, 8f);
        targetPos.z = Mathf.Clamp(targetPos.z, -7.5f, -2f);
        targetPos.y = 0.5f;
        
        // Calculate speed and height based on extra time
        targetPosition = targetPos;
        float extraTime = Mathf.Lerp(dropShotExtraTimeAtLowScore, dropShotExtraTimeAtHighScore, t);
        extraTime += GetMissRecoveryBonus();
        float distance = Vector3.Distance(player.position, targetPosition);
        float playerMaxSpeed = playerController != null ? playerController.maxSpeed : 8f;
        float timeNeeded = distance / playerMaxSpeed;
        float totalTime = timeNeeded + extraTime;

        // Set speed, distance, and height
        float shotDistance = Vector3.Distance(startPosition, targetPosition);
        currentFlightSpeed = shotDistance / totalTime;
        currentArcHeight = arcHeight;
    }

    // Tutorial shot type
    void ServeTutorialShot()
    {
        // Hit birdie to the right of the player
        if (player != null)
            targetPosition = player.position + tutorialTargetOffset;
        else
            targetPosition = new Vector3(2f, 0.5f, -5f); // Backup offset
    
        // Set speed and height
        currentFlightSpeed = 6f;
        currentArcHeight = arcHeight;
    }

    // Drop shot goes near net with a bit of extra time
    void ServeDropShot(int score)
    {
        // Pick location near net
        // Will need to adjust manually if court size changes
        float randomX = Random.Range(-8f, 8f);
        float netZ = -0.8f - Random.Range(0f, dropShotNetDistance);
        netZ = Mathf.Clamp(netZ, -9f, -1f);

	// Force the shot to be a legal position
	randomX = Mathf.Clamp(randomX, -8f, 8f);
        netZ = Mathf.Clamp(netZ, -7.5f, -2f);
        
        targetPosition = new Vector3(randomX, 0.5f, netZ);
        
        // Calculate extra time
        float t = Mathf.Clamp01((float)score / scoreForFullDistribution);
        float extraTime = Mathf.Lerp(dropShotExtraTimeAtLowScore, dropShotExtraTimeAtHighScore, t);
        extraTime += GetMissRecoveryBonus();
        
        // Calculate time, distance, speed
        float distance = Vector3.Distance(player.position, targetPosition);
        float playerMaxSpeed = playerController != null ? playerController.maxSpeed : 8f;
        float timeNeeded = distance / playerMaxSpeed;
        float totalTime = timeNeeded + extraTime;
        
        // Set distance, speed, arc height
        float shotDistance = Vector3.Distance(startPosition, targetPosition);
        currentFlightSpeed = shotDistance / totalTime;
        currentArcHeight = arcHeight * dropShotArcMultiplier;
    }

    // Long shot goes to back of court with extra time
    void ServeLongShot(int score)
    {
        // Pick location near back
        // Will need to adjust manually if court size changes
        float randomX = Random.Range(-8f, 8f);
        float backZ = -9f + Random.Range(0f, longShotBackDistance);
        
        // Force the shot to be a legal position
	randomX = Mathf.Clamp(randomX, -8f, 8f);
        backZ = Mathf.Clamp(backZ, -7.5f, -2f);

        targetPosition = new Vector3(randomX, 0.5f, backZ);
        
        // Calculate extra time
        float t = Mathf.Clamp01((float)score / scoreForFullDistribution);
        float extraTime = Mathf.Lerp(longShotExtraTimeAtLowScore, longShotExtraTimeAtHighScore, t);
        extraTime += GetMissRecoveryBonus();
        
        // Calculate time, distance, speed
        float distance = Vector3.Distance(player.position, targetPosition);
        float playerMaxSpeed = playerController != null ? playerController.maxSpeed : 8f;
        float timeNeeded = distance / playerMaxSpeed;
        float totalTime = timeNeeded + extraTime;
        
        // Set distance, speed, arc height
        float shotDistance = Vector3.Distance(startPosition, targetPosition);
        currentFlightSpeed = shotDistance / totalTime;
        currentArcHeight = arcHeight * longShotArcMultiplier;
    }

    // Calculate normal shot probability. Starts at 100% and drops to 20%
    float GetNormalShotChance(int score)
    {
        float t = Mathf.Clamp01((float)score / scoreForFullDistribution);
        return Mathf.Lerp(1f, 0.2f, t);
    }

    // Calculate drop shot probability. Maxes out at 40%
    float GetDropShotChance(int score)
    {
        float t = Mathf.Clamp01((float)score / scoreForFullDistribution);
        return Mathf.Lerp(0f, 0.4f, t);
    }

    // Birdie position during flight
    void UpdateFlight()
    {
        // Avoid small mismatch with final position
        if (journeyLength <= 0.001f)
        {
            transform.position = targetPosition;
            isFlying = false;
            journeyProgress = 1f;
            transform.localScale = originalScale;
            
            if (transform.position.z < 0)
            {
                isGrounded = true;
                landedTime = Time.time;
            }
            return;
        }

        // Increase progress
        journeyProgress += (currentFlightSpeed / journeyLength) * Time.deltaTime;

        // Calculate position
        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, journeyProgress);

        // Calculate arc height
        float arcProgress = 4f * journeyProgress * (1f - journeyProgress);
        float currentArcHeightValue = arcProgress * currentArcHeight;

        // Scale sprite to simulate arc
        float scaleMultiplier = 1f + (currentArcHeightValue);
        transform.localScale = originalScale * scaleMultiplier;

	// Rotate birdie to face flight direction
        Vector3 flightDirection = targetPosition - startPosition;
        if (flightDirection.magnitude > 0.01f)
        {
            // Calculate angle in degrees (looking down from above, XZ plane)
            float angle = Mathf.Atan2(flightDirection.x, flightDirection.z) * Mathf.Rad2Deg;
        
            // Apply rotation (rotate around Y axis to face direction)
            transform.rotation = Quaternion.Euler(90f, angle, 0f);
        }

        // Apply position
        transform.position = currentPos;

        // Check if flight is over
        if (journeyProgress >= 1f)
        {
            // Fligh over
            isFlying = false;
            journeyProgress = 0f;

            // Reset scale and position
            transform.localScale = originalScale;
            transform.position = targetPosition;

            // Hide target indicator
            if (targetIndicator != null)
                targetIndicator.SetActive(false);

            // check which side landed on (-ve z = player side)
            if (transform.position.z < 0)
            {
                isGrounded = true;
                landedTime = Time.time;
            }
            else
            {
                isGrounded = false;
                Invoke("ServeToPlayer", 0f);
            }
        }

    }

    // Make tutorial icons appear and flash as necessary
    void UpdateTutorialIcons()
    {
        if (player == null) return;

        // Is player close to the birdie
        float distance = Vector3.Distance(player.position, transform.position);
        bool playerClose = distance <= hitRange;

        // Calculate icon positions with different offsets
        Vector3 moveIconPosition = targetPosition + tutorialMoveIconOffset;
        Vector3 clickIconPosition = targetPosition + tutorialClickIconOffset;
    
        // If player is close, then tell the player to click on the birdie
        if (playerClose)
        {
            if (tutorialMoveIcon != null)
                tutorialMoveIcon.SetActive(false);
            if (tutorialClickIcon != null)
            {
                tutorialClickIcon.transform.position = clickIconPosition;
                tutorialClickIcon.SetActive(true);
            }
        }
        // If player is far, tell the player to move to the birdie
        else
        {
           if (tutorialClickIcon != null)
                tutorialClickIcon.SetActive(false);
           if (tutorialMoveIcon != null)
           {
                tutorialMoveIcon.transform.position = moveIconPosition;
                tutorialMoveIcon.SetActive(true);
            
                // Flash the WASD icon
                tutorialFlashTimer += Time.deltaTime;
                if (tutorialFlashTimer >= tutorialIconFlashSpeed)
                {
                    tutorialFlashTimer = 0f;
                    tutorialShowingFrame1 = !tutorialShowingFrame1;
                    
                    SpriteRenderer iconSprite = tutorialMoveIcon.GetComponent<SpriteRenderer>();
                    if (iconSprite != null)
                    {
                        iconSprite.sprite = tutorialShowingFrame1 ? tutorialMoveFrame1 : tutorialMoveFrame2;
                    }
                }
            }
        }
    }

    // Reset birdie to opponent's position and serve to player when player fails to hit the birdie back
    void ResetAndServe()
    {

    // Move to opponent's position with middle of opponent field as backup
    if (opponent != null)
        transform.position = opponent.position + new Vector3(-1.65f, 0f, -0.1f);
    else
        transform.position = new Vector3(0f, 0.5f, 5f);
    
    // Reset scale
    transform.localScale = originalScale;

    // Reset grounded state
    isGrounded = false;
    isOnPlayerSide = false;
    
    // Serve to player after a pause
    Invoke("ServeToPlayer", 2f);
    }

    // Add extra time for the first two shots after a miss
    float GetMissRecoveryBonus()
    {
        if (consecutiveHits == 0)
            return 0.2f;
        else if (consecutiveHits == 1)
            return 0.1f;
        else
            return 0f;
    }


    // Visual debug in editor
    void OnDrawGizmosSelected()
    {
        // Draw hit range when birdie selected
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }

}
