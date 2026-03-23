using UnityEngine;

public class SisterAnimator : MonoBehaviour
{
    private Animator animator;
    private float lastInputTime;
    public float inputCooldown = 0.05f; 

    void Awake()
    {
        animator = GetComponent<Animator>(); 
        lastInputTime = Time.time;
    }
    private bool Cooldown(float pressedTime)
    {
        return pressedTime - lastInputTime >= inputCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void triggerUp()
    {
        float pressedTime = Time.time; 
        if (Cooldown(pressedTime))
            animator.SetTrigger("Up");
            lastInputTime = pressedTime; 
    }

    public void triggerRight()
    {
        float pressedTime = Time.time; 
        if (Cooldown(pressedTime))
            animator.SetTrigger("Right");
            lastInputTime = pressedTime; 
    }

    public void triggerDown()
    {
        float pressedTime = Time.time; 
        if (Cooldown(pressedTime))
            animator.SetTrigger("Down");
            lastInputTime = pressedTime;
    }

    public void triggerLeft()
    {
        float pressedTime = Time.time;
        if (Cooldown(pressedTime))
            animator.SetTrigger("Left");
            lastInputTime = pressedTime; 
    }
}
