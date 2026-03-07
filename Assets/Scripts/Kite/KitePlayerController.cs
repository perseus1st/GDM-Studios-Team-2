//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class KitePlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private CharacterController controller;
    private UnityEngine.Vector2 moveInput;
    private UnityEngine.Vector3 velocity;
    public bool invincible = false;
    public float invincibilityTimer = 3f;
    public float flashInterval = 0.1f;
    private float lastFlashTime;
    private bool isVisible = true;
    public GameObject sprite;
    float timer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
    }
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        // Debug.Log($"Move Input: {moveInput}");
    }

    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * speed * Time.deltaTime);

        if (invincible) {

            UpdateInvincibilityFlash();

            Debug.Log($"Invincibility for {3-timer} seconds");
            timer += Time.deltaTime;
            if (timer >= invincibilityTimer)
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
            // gameObject.transform.find("KiteModel").GetComponent<Renderer>().enabled = isVisible;
            sprite.GetComponent<Renderer>().enabled = isVisible;


            float a = 0.05f; // arbitrary scaling factor for flash speed
            flashInterval = a*(-timer+invincibilityTimer+a); // to flash faster as invincibility ends
        }
    }

        // End invincibility and restore visibility
    void EndInvincibility()
    {
        invincible = false;
        isVisible = true;
        timer = 0f;
        Debug.Log("No longer invincible");
        sprite.GetComponent<Renderer>().enabled = isVisible;
    }

    public void isHit()
    {
        this.invincible = true;
    }
}
