// Created by Daniil Makarenko

using UnityEngine;

public class OpponentController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float minMoveDistance = 0.5f;  // Minimum distance to move
    public float maxMoveDistance = 3f;    // Maximum distance to move
    public float maxSpeed = 10f;          // Maximum movement speed (units/s)
    public float acceleration = 40f;      // Acceleration rate (units/s²)
    public float deceleration = 50f;      // Deceleration rate (units/s²)
    
    [Header("Court Boundaries")]
    public float minX = -8f;   // Left boundary
    public float maxX = 8f;    // Right boundary
    public float minZ = 1f;    // Front boundary (opponent side)
    public float maxZ = 8f;    // Back boundary

    // Animator values
    private Animator animator;
    [SerializeField] private Animator racketAnimator;
    private SpriteRenderer spriteRenderer;
    
    private Vector3 targetPosition;   // Where opponent is moving to
    private Vector3 currentVelocity;  // Current movement velocity
    private bool isMoving = false;    // Is opponent currently moving
    private Rigidbody rb;             // Rigidbody for physics movement
    private bool isPlayingHitAnimation = false; // Is hit animation currently playing
    private bool hitAnimationTriggered = false; // Has the actual hit been processed yet
    public bool racket = true; 
    public float hitAnimationDelay = 0.3f; // Delay between animation start and birdie hit
    public float hitAnimationLength = 0.4f; // Length of hit animation for freezing player

    
    void Start()
    {
        // Get or add Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Get animator
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        // Configure Rigidbody
        rb.freezeRotation = true;
        rb.useGravity = false;  // Opponent doesn't need gravity
        
        // Start at current position
        targetPosition = transform.position;
        currentVelocity = Vector3.zero;
    }
    
    void FixedUpdate()
    {
        if (isMoving)
        {
            animator.SetBool("IsMoving", isMoving);
            // racketAnimator.SetBool("IsMoving", isMoving);
            animator.SetBool("Racket", racket); 
            
            // Calculate direction to target (only XZ plane)
            Vector3 currentPos = transform.position;
            Vector3 directionToTarget = new Vector3(
                targetPosition.x - currentPos.x,
                0f,
                targetPosition.z - currentPos.z
            );
            
            float distanceToTarget = directionToTarget.magnitude;
            
            // Check if reached target (within small threshold)
            if (distanceToTarget < 0.1f)
            {
                // Reached target, start decelerating
                isMoving = false;
                animator.SetBool("IsMoving", isMoving);
                racketAnimator.SetBool("IsMoving", isMoving);
            }
            
            if (isMoving)
            {
                // Normalize direction
                Vector3 moveDirection = directionToTarget.normalized;
                
                // Calculate desired velocity
                Vector3 desiredVelocity = moveDirection * maxSpeed;
                
                // Accelerate toward desired velocity
                float step = acceleration * Time.fixedDeltaTime;
                currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, desiredVelocity.x, step);
                currentVelocity.z = Mathf.MoveTowards(currentVelocity.z, desiredVelocity.z, step);
            }
            else
            {
                // Decelerate to stop
                float step = deceleration * Time.fixedDeltaTime;
                currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, 0f, step);
                currentVelocity.z = Mathf.MoveTowards(currentVelocity.z, 0f, step);
                
                // Check if fully stopped
                if (currentVelocity.sqrMagnitude < 0.001f)
                {
                    currentVelocity = Vector3.zero;
                    rb.linearVelocity = Vector3.zero;
                }
            }
            
            // Apply velocity to rigidbody
            rb.linearVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        }
        else
        {
            // Not moving, decelerate to full stop
            if (currentVelocity.sqrMagnitude > 0.001f)
            {
                float step = deceleration * Time.fixedDeltaTime;
                currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, 0f, step);
                currentVelocity.z = Mathf.MoveTowards(currentVelocity.z, 0f, step);
                rb.linearVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
            }
            else
            {
                currentVelocity = Vector3.zero;
                rb.linearVelocity = Vector3.zero;
            }
        }
    }
    
    // Call this when opponent hits the birdie
    public void MoveToRandomPosition()
    {
        // Calculate random offset from current position
        float randomDistance = Random.Range(minMoveDistance, maxMoveDistance);
        float randomAngle = Random.Range(0f, 360f);
        
        // Convert to X and Z offset
        Vector3 offset = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance,
            0f,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance
        );
        
        // Calculate new target position
        Vector3 newTarget = transform.position + offset;
        
        // Clamp to court boundaries
        newTarget.x = Mathf.Clamp(newTarget.x, minX, maxX);
        newTarget.z = Mathf.Clamp(newTarget.z, minZ, maxZ);
        newTarget.y = transform.position.y; // Keep same height
        
        // Set as target and start moving
        targetPosition = newTarget;
        isMoving = true;
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
}