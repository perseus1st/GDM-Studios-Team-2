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
            Debug.Log($"Invincibility for {3-timer} seconds");
            timer += Time.deltaTime;
            if (timer >= invincibilityTimer)
            {
                invincible = false;
                timer = 0f;
                Debug.Log("No longer invincible");
            }
        }
    }

    public void isHit()
    {
        this.invincible = true;
    }
}
