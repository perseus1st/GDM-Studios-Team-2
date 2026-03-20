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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>(); 
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void OnUp(InputValue value)
    {
        if (value.isPressed)
            animator.SetTrigger("Up");
    }

    void OnRight(InputValue value)
    {
        if (value.isPressed)
            animator.SetTrigger("Right");
    }

    void OnDown(InputValue value)
    {
        if (value.isPressed)
            animator.SetTrigger("Down");
    }

    void OnLeft(InputValue value)
    {
        if (value.isPressed)
            animator.SetTrigger("Left");
    }
}
