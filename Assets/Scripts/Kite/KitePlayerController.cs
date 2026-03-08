//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class KitePlayerController : MonoBehaviour
{
    [Header("Kite Settings")]
    [SerializeField] public float speed = 5f; // Speed of player
    public GameObject sprite;

    private CharacterController controller;
    private UnityEngine.Vector2 moveInput;
    private UnityEngine.Vector3 velocity;

    [Header("Invincibility")]
    public bool isInvincible = false; // Boolean to track invincibility of player
    public float invincibilityTimerLimit = 3f; // How long the player is invincible for
    float timer = 0f; // Timer for invincibility
    public float flashInterval; // How often the player flashes when invincible, decreases linearly
    private float lastFlashTime; // Tracks time of last flash
    private bool isVisible = true; // Tracks player's visibility

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * speed * Time.deltaTime);

        if (isInvincible)
        {
            UpdateInvincibilityFlash();

            timer += Time.deltaTime; // Increment timer until it reaches invincibilityTimerLimit
            Debug.Log($"Invincibility for {3 - timer} seconds");
            if (timer >= invincibilityTimerLimit)
            {
                EndInvincibility();
            }
        }
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
            sprite.GetComponent<Renderer>().enabled = isVisible;


            float a = 0.05f; // Arbitrary scaling factor for flash speed
            flashInterval = a * (-timer + invincibilityTimerLimit + a); // Linear function to flash faster as invincibility ends
        }
    }

    // End invincibility and restore visibility
    void EndInvincibility()
    {
        isInvincible = false;
        isVisible = true;
        timer = 0f;
        Debug.Log("No longer invincible");
        sprite.GetComponent<Renderer>().enabled = isVisible;
    }

    public void isHit()
    {
        this.isInvincible = true;
    }
}
