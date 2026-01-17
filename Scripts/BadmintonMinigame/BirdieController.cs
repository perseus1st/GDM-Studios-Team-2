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

    // Birdie movement
    [Header("Movement Settings")]
    public float arcHeight = 2f; // height of arc

    // Hit detection
    [Header("Hit Settings")]
    public float hitRange = 2f; // How close player has to be to hit birdie in units
    public float hitWindowStart = 0.9f; // What percentage into flight the player can hit birdie



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



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store original scale
        originalScale = transform.localScale;

        // Glow starts disabled
        if (glowEffect != null)
            glowEffect.SetActive(false);

        // Birdie starts on opponent side
        isOnPlayerSide = false;
        canBeHit = false;
        isFlying = false;

        // Start first serve after a delay just for testing
        Invoke("ServeToPlayer", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // Checks for birdie side (z<0 = player's side)
        isOnPlayerSide = transform.position.z < 0;

        // Reset hit window
        bool inHitWindow = false;
        
        if (isFlying && isOnPlayerSide)
        {
            // Can hit during last part of flight
            inHitWindow = journeyProgress >= hitWindowStart;
        }
        else if (isGrounded && isOnPlayerSide)
        {

            // Get grounded window from ScoreManager with fallback
            float currentGroundedWindow = ScoreManager.Instance != null ? ScoreManager.Instance.GetCurrentGroundedWindow() : 0.5f; 

            // can hit for short time after landing
            float timeSinceLanded = Time.time - landedTime;
            inHitWindow = timeSinceLanded <= currentGroundedWindow;
            
            // lose life and re-serve if on ground for too long
            if (timeSinceLanded > currentGroundedWindow)
            {
                if (ScoreManager.Instance != null)
                    ScoreManager.Instance.LoseLife();
                ResetAndServe();
            }
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

        // Increment score
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore();

        // Get opponent position
        startPosition = transform.position;

        // Aim for opponent with middle of opponent play area as backup
        if (opponent != null)
            targetPosition = opponent.position;
        else
            targetPosition = new Vector3(0f, 0.5f, 5f);

        // Calcualte distance for flight
        journeyLength = Vector3.Distance(startPosition, targetPosition);

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
        
        // Fly to middle of player's side for testing
        startPosition = transform.position;
        targetPosition = new Vector3(0f, 0.5f, -5f);

        // Calculate flight distance
        journeyLength = Vector3.Distance(startPosition, targetPosition);

        // Reset flight and start new flight
        journeyProgress = 0f;
        isFlying = true;
        isOnPlayerSide = false;
        isGrounded = false;
    }

    // Birdie position during flight
    void UpdateFlight()
    {
        // Get dynamic flight speed from ScoreManager with fallback
        float currentSpeed = ScoreManager.Instance != null ? ScoreManager.Instance.GetCurrentFlightSpeed() : 8f;

        // Increase progress
        journeyProgress += (currentSpeed / journeyLength) * Time.deltaTime;

        // Calculate position
        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, journeyProgress);

        // Calculate arc height
        float arcProgress = 4f * journeyProgress * (1f - journeyProgress);
        float currentArcHeight = arcProgress * arcHeight;

        // Scale sprite to simulate arc
        float scaleMultiplier = 1f + (currentArcHeight);
        transform.localScale = originalScale * scaleMultiplier;

        // Apply position
        transform.position = currentPos;

        // Check if flight is over
        if (journeyProgress >= 1f)
        {
            // Fligh over
            isFlying = false;
            journeyProgress = 0f;

            // Reset scale
            transform.localScale = originalScale;

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

    // Reset birdie to opponent's position and serve to player when player fails to hit the birdie back
    void ResetAndServe()
    {

    // Move to opponent's position with middle of opponent field as backup
    if (opponent != null)
        transform.position = opponent.position;
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


    // Visual debug in editor
    void OnDrawGizmosSelected()
    {
        // Draw hit range when birdie selected
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }

}
