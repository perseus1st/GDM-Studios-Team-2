using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator; 
    private const int IDLE = 0; 
     private const int LEFT = 1;
    private const int UP = 2;
    private const int DOWN = 3;
    private const int RIGHT = 4;
    private float lastInputTime;
    public float inputCooldown = 0.05f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>(); 
        lastInputTime = Time.time; 
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private bool Cooldown(float pressedTime)
    {
        return pressedTime - lastInputTime >= inputCooldown;
    }

    void OnUp(InputValue value)
    {
        float pressedTime = Time.time; 
        if (value.isPressed && Cooldown(pressedTime))
            animator.SetTrigger("Up");
            lastInputTime = pressedTime; 
    }

    void OnRight(InputValue value)
    {
        float pressedTime = Time.time; 
        if (value.isPressed && Cooldown(pressedTime))
            animator.SetTrigger("Right");
            lastInputTime = pressedTime; 
    }

    void OnDown(InputValue value)
    {
        float pressedTime = Time.time; 
        if (value.isPressed && Cooldown(pressedTime))
            animator.SetTrigger("Down");
            lastInputTime = pressedTime;
    }

    void OnLeft(InputValue value)
    {
        float pressedTime = Time.time;
        if (value.isPressed && Cooldown(pressedTime))
            animator.SetTrigger("Left");
            lastInputTime = pressedTime; 
    }
}
