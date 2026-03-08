// Created by Daniil Makarenko

using UnityEngine;
using UnityEngine.InputSystem;

public class DodgeballPlayerController : MonoBehaviour
{
    // Reference player's rigidbody for movement
    private Rigidbody rb;

    // Public accessor for tutorial manager
    public Rigidbody Rigidbody => rb;

    // Store movement input
    private float movementX; // left/right
    private float movementY; // forward/back

    // Animator values
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Movement control settings
    [Header("Movement Settings")]
    public float maxSpeed = 8f; // units per second
    public float acceleration = 40f; // units/second squared
    public float deceleration = 50f; // units/second squared
    public float inputDeadzone = 0.1f; // in case we add controller support
    
    // Court boundaries (adjust based on gymnasium size)
    [Header("Court Boundaries")]
    public float minX = -9f; // Left boundary
    public float maxX = 9f; // Right boundary
    public float minZ = -9f; // Front boundary
    public float maxZ = -1f; // Back boundary (net/center line)
    
    // For interact input buffer
    [Header("Interact Input Settings")]
    public float interactBufferTime = 0.2f; // How long interact stays active in seconds
    
    private float lastInteractPressTime = -999f; // When interact was last pressed

    // Ball interaction
    [Header("Ball Interaction Settings")]
    public float collectRange = 2f; // How close player needs to be to collect ball
    public float throwDuration = 0.5f; // How long throwing animation takes
    public bool isThrowing = false; // Is player currently throwing
    private float throwStartTime; // When current throw started
    
    // References for effects
    [Header("References")]
    public GameObject playerModel; // Visual representation for flash effect
    public AudioSource collectSound; // Sound when collecting ball
    public AudioSource throwSound; // Sound when throwing ball
    
    // Hit detection and invincibility
    [Header("Hit Settings")]
    public float invincibilityDuration = 1.5f; // How long invincibility lasts after hit
    public float flashInterval = 0.1f; // How fast player flashes during invincibility
    private bool isInvincible = false; // Is player currently invincible
    private float invincibilityStartTime; // When invincibility started
    private float lastFlashTime; // Last time flash state changed
    private bool isVisible = true; // Current visibility state for flashing
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get Rigidbody for player
        rb = GetComponent<Rigidbody>();

        // Player doesn't need to rotate
        rb.freezeRotation = true;
        
        // Ensure player model reference
        if (playerModel == null)
        {
            playerModel = gameObject;
        }

        // Get animator
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Default unity input function for movement
    void OnMove(InputValue movementValue)
    {
        // Store input regardless of throwing state
        // This ensures input is captured when throw ends
        Vector2 m = movementValue.Get<Vector2>();

        // Store for use in FixedUpdate
        movementX = m.x; // left/right
        movementY = m.y; // up/down
    }

    // Input system calls this when Interact is performed (left click or E key)
    void OnInteract(InputValue value)
    {
        if (value.isPressed)
            // Record press time
            lastInteractPressTime = Time.time; 
    }

    // Lets ball check if there's an interact input. Makes sure input only triggers once per press
    public bool GetInteractPressed()
    {
        // Calculate how much time has passed since previous press
        float timeSincePress = Time.time - lastInteractPressTime;

        // Return true if within buffer
        return timeSincePress <= interactBufferTime;
    }

    // Consume the interact input
    public void ConsumeInteract()
    {
        lastInteractPressTime = -999f;
    }

    // FixedUpdate called at fixed time intervals for consistent physics
    private void FixedUpdate()
    {
        // The goal is to have snappy and responsive movement with fast acceleration and deceleration

        // Get current velocity
        Vector3 currentVel = rb.linearVelocity;

        // If throwing, stop all movement immediately and don't process input
        if (isThrowing)
        {
            currentVel = new Vector3(0f, currentVel.y, 0f);
            rb.linearVelocity = currentVel;
            return;
        }

        // Turn 2D input vector into 3D movement vector
        Vector3 input = new Vector3(movementX, 0f, movementY);

        // Check for input above deadzone
        if (input.sqrMagnitude > inputDeadzone * inputDeadzone)
        {
            // Don't let player move faster diagonally
            input = input.normalized;

            // Calculate desired velocity
            Vector3 desiredXZ = input * maxSpeed;

            // Calculate velocity change step
            float step = acceleration * Time.fixedDeltaTime;

            // Change velocity without overshoot
            float newVX = Mathf.MoveTowards(currentVel.x, desiredXZ.x, step);
            float newVZ = Mathf.MoveTowards(currentVel.z, desiredXZ.z, step);

            // Make velocity vector
            currentVel = new Vector3(newVX, currentVel.y, newVZ);
        }
        else
        {
            // No input detected therefore decelerate
            float step = deceleration * Time.fixedDeltaTime;

            // Reduce velocity towards zero
            float newVX = Mathf.MoveTowards(currentVel.x, 0f, step);
            float newVZ = Mathf.MoveTowards(currentVel.z, 0f, step);

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
    
    // Update is called once per frame
    void Update()
    {
        // Handle invincibility flashing effect
        if (isInvincible)
        {
            UpdateInvincibilityFlash();
            
            // Check if invincibility period is over
            if (Time.time - invincibilityStartTime >= invincibilityDuration)
            {
                EndInvincibility();
            }
        }

        // Tells animator what to do
        if (animator != null)
        {
            // Updates animator parameters
            bool isMoving = Mathf.Abs(movementX) > inputDeadzone || Mathf.Abs(movementY) > inputDeadzone;
            animator.SetBool("IsMoving", isMoving);
            animator.SetFloat("MoveX", Mathf.Abs(movementX));
            animator.SetFloat("MoveY", movementY);
            animator.SetBool("IsThrowing", isThrowing);
            
            // Flips sprite when moving right
            if (spriteRenderer != null && Mathf.Abs(movementX) > inputDeadzone)
           {
                spriteRenderer.flipX = movementX > 0;
            }
        }
    }
    
    // Called when player is hit by a ball
    public void GetHit()
    {
        // Ignore if already invincible
        if (isInvincible)
            return;
        
        // Lose a life
        if (DodgeballScoreManager.Instance != null)
            DodgeballScoreManager.Instance.LoseLife();
        
        // Start invincibility
        StartInvincibility();
        
        // Cancel any ongoing throw
        if (isThrowing)
        {
            isThrowing = false;
        }
    }
    
    // Begin invincibility period with flashing
    void StartInvincibility()
    {
        isInvincible = true;
        invincibilityStartTime = Time.time;
        lastFlashTime = Time.time;
        isVisible = true;
    }
    
    // Update flashing effect during invincibility
    void UpdateInvincibilityFlash()
    {
        // Check if it's time to toggle visibility
        if (Time.time - lastFlashTime >= flashInterval)
        {
            isVisible = !isVisible;
            lastFlashTime = Time.time;
            
            // Toggle player model visibility
            if (playerModel != null)
            {
                Renderer[] renderers = playerModel.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    renderer.enabled = isVisible;
                }
            }
        }
    }
    
    // End invincibility and restore visibility
    void EndInvincibility()
    {
        isInvincible = false;
        
        // Ensure player is visible
        if (playerModel != null)
        {
            Renderer[] renderers = playerModel.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }
        }
        isVisible = true;
    }
    
    // Get current movement direction for tracking throws
    public Vector3 GetMovementDirection()
    {
        // Calculate movement direction from current velocity
        if (rb.linearVelocity.magnitude > 0.1f)
            return rb.linearVelocity.normalized;
        else
            return Vector3.zero;
    }
    
    // Check if player is currently invincible (for ball collision)
    public bool IsInvincible()
    {
        return isInvincible;
    }
    
    // Check if player is currently throwing (for ball collision)
    public bool IsThrowing()
    {
        return isThrowing;
    }
    
    // Visual debug in editor
    void OnDrawGizmosSelected()
    {
        // Draw collect range when player selected
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectRange);
        
        // Draw court boundaries
        Gizmos.color = Color.yellow;
        Vector3 bottomLeft = new Vector3(minX, transform.position.y, minZ);
        Vector3 bottomRight = new Vector3(maxX, transform.position.y, minZ);
        Vector3 topLeft = new Vector3(minX, transform.position.y, maxZ);
        Vector3 topRight = new Vector3(maxX, transform.position.y, maxZ);
        
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}