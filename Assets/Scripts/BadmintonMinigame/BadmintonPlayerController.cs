// Created by Daniil Makarenko

using UnityEngine;
using UnityEngine.InputSystem;

public class BadmintonPlayerController : MonoBehaviour
{
    // Reference player's rigidbody for movement
    private Rigidbody rb;

    // Public accessor for game completion
    public Rigidbody Rigidbody => rb;

    // Store movement input
    private float movementX; // left/right
    private float movementY; // forward/back

    // Animator values
    private Animator animator;
    [SerializeField] private Animator racketAnimator;
    private SpriteRenderer spriteRenderer;

    // Movement control sliders
    [Header("Movement Settings")]
    public float maxSpeed = 8f; // units per second
    public float acceleration = 40f; // units/second squared
    public float deceleration = 50f; // units/second squared
    public float inputDeadzone = 0.1f; // in case we add controller support
    public bool disableFlip = false; // added 3/2/2026 by Léon
    public bool racket = false; // added 3/5/2026 by Léon

    // Court boundaries 
    [Header("Court Boundaries")]
    public float minX = -9f; // Left boundary
    public float maxX = 9f; // Right boundary
    public float minZ = -9f; // Front boundary
    public float maxZ = -1f; // Back boundary (net/center line)
    
    // For hit input buffer
    [Header("Hit Input Settings")]
    public float hitBufferTime = 0.2f;  // How long click stays active in seconds
    public float hitAnimationDelay = 0.3f; // Delay between animation start and birdie hit
    public float hitAnimationLength = 0.4f; // Length of hit animation for freezing player
    
    private float lastHitPressTime = -999f;  // When click was last pressed
    private bool isPlayingHitAnimation = false; // Is hit animation currently playing
    private bool hitAnimationTriggered = false; // Has the actual hit been processed yet

    // Reference to PauseManager
    [Header("Pause Manager Reference")]
    [SerializeField] PauseManager pauseManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get Rigidbody for player
        rb = GetComponent<Rigidbody>();

        // Player doesn't need to rotate
        rb.freezeRotation = true;

        // Get animator
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }

    // Default unity input function
    void OnMove(InputValue movementValue)
    {
        // 2D input vector from WASD
        Vector2 m = movementValue.Get<Vector2>();

        // Store for use in FixedUpdate
        movementX = m.x; // left/right
        movementY = m.y; // up/down

    }

    // Input system calls this when a Hit is performed
    void OnInteract(InputValue value)
    {
        if (value.isPressed && !isPlayingHitAnimation)
        {
            // Record press time
            lastHitPressTime = Time.time;
            
            // Start hit animation
            PlayHitAnimation();
        }
    }

    // Input system calls this when the player presses ESC
    void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            pauseManager.Pause();
        }
    }

    // Input system calls this when the player presses ESC while paused
    void OnUnpause(InputValue value)
    {
        if (value.isPressed)
        {
            pauseManager.Resume();
        }
    }

    // Updates every frame
    void Update()
    {
        // Tells animator what to do
        if (animator != null)
        {
            // Only update movement animations if not playing hit animation
            if (!isPlayingHitAnimation)
            {
                // Updates animator parameters
                bool isMoving = Mathf.Abs(movementX) > inputDeadzone || Mathf.Abs(movementY) > inputDeadzone;
                animator.SetBool("IsMoving", isMoving);
                animator.SetFloat("MoveX", Mathf.Abs(movementX));
                animator.SetFloat("MoveY", movementY);
                racketAnimator.SetBool("IsMoving", isMoving); // added 3/2/2026 by Léon
                animator.SetBool("Racket", racket); // added 3/5/2026 by Léon
                
                // Flips sprite when moving right
                if (spriteRenderer != null && Mathf.Abs(movementX) > inputDeadzone && !disableFlip) // modified 3/2/2026 by Léon
                {
                    spriteRenderer.flipX = movementX > 0;
                }
            }
        }
    }

    // Trigger the hit animation
    public void PlayHitAnimation()
    {
        if (animator != null && !isPlayingHitAnimation)
        {
            isPlayingHitAnimation = true;
            hitAnimationTriggered = false;
            animator.SetTrigger("Hit");
            racketAnimator.SetTrigger("Hit");
        
            // Schedule the actual hit to happen after delay
            Invoke("TriggerDelayedHit", hitAnimationDelay);
        
            // Schedule animation end (adjust 0.4f to match animation length)
            Invoke("OnHitAnimationComplete", hitAnimationLength);
        }
    }

    // Called after the animation delay to mark that hit should be processed
    private void TriggerDelayedHit()
    {
        hitAnimationTriggered = true;
    }

    // Called by animation event at the end of hit animation
    public void OnHitAnimationComplete()
    {
        isPlayingHitAnimation = false;
        hitAnimationTriggered = false;
    }

    // Lets birdie check if theres a hit. Makes sure the hit only triggers once per click
    public bool GetHitPressed()
    {
        // Calculate how much time has passed since previous press
        float timeSincePress = Time.time - lastHitPressTime;

        // Return true if within buffer AND animation has reached the hit point
        return timeSincePress <= hitBufferTime && hitAnimationTriggered;
    }

    // Check if player is currently in hit animation (for birdie to pause timer)
    public bool IsInHitAnimation()
    {
        return isPlayingHitAnimation;
    }

    // Consume the hit
    public void ConsumeHit()
    {
        lastHitPressTime = -999f;
        hitAnimationTriggered = false;
    }

    // FixedUpdate called at fixed time intervals for consistent physics
    private void FixedUpdate()
    {
        // The goal is to have snappy and responsive movement with fast acceleration and desceleration

        // Don't move if playing hit animation
        if (isPlayingHitAnimation)
        {
            // Stop all movement during hit animation
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        // Turn 2D input vector into 3D movement vector
        Vector3 input = new Vector3(movementX, 0f, movementY);

        // get current velocity
        Vector3 currentVel = rb.linearVelocity;

        // Check for input above deadzone
        if (input.sqrMagnitude > inputDeadzone * inputDeadzone)
        {
            // Don't let player move faster diagonally
            input = input.normalized;

            // Calculate velocity
            Vector3 desiredXZ = input * maxSpeed;

            // Calculate velocity change
            float step = acceleration * Time.fixedDeltaTime;

            // Change velocity without overshoot
            float newVX = Mathf.MoveTowards(currentVel.x, desiredXZ.x, step);
            float newVZ = Mathf.MoveTowards(currentVel.z, desiredXZ.z, step);

            // Make velocity vector
            currentVel = new Vector3(newVX, currentVel.y, newVZ);
        }
        else
        {
            // no input detected therefore descelerate
            float step = deceleration * Time.fixedDeltaTime;

            // reduce velocity towards zero
            float newVX = Mathf.MoveTowards(currentVel.x, 0f, step);
            float newVZ = Mathf.MoveTowards(currentVel.z, 0F, step);

            // Make velocity vector
            currentVel = new Vector3(newVX, currentVel.y, newVZ);
        }

        // Apply velocity
        rb.linearVelocity = currentVel;
        
        // Clamp position to court boundaries
        Vector3 clampedPos = rb.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
        clampedPos.z = Mathf.Clamp(clampedPos.z, minZ, maxZ);
        rb.position = clampedPos;
    }
}