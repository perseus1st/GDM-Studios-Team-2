
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float animationDampTime = 0.05f;
    [SerializeField] private float rotationSpeed = 5f;

    private IInteractable currentInteractable;
    private CharacterController controller;
    private UnityEngine.Vector2 moveInput;
    private UnityEngine.Vector3 velocity;
    private Animator animator;
    private UnityEngine.Vector3 fixedCamForward;
    private UnityEngine.Vector3 fixedCamRight;

    private const float sqrt2 = 1.189207f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0;
        camRight.y = 0;

        fixedCamForward = camForward.normalized;
        fixedCamRight = camRight.normalized;
        
    }

    public void onMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<UnityEngine.Vector2>();
        // moveInput.x = moveInput.x/(sqrt2) + moveInput.y/(sqrt2);
        // moveInput.y = moveInput.x/(sqrt2) + moveInput.y/(sqrt2);



    }

    public void onInteract(InputAction.CallbackContext context)
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    public void SetInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        Vector3 moveDirection = fixedCamForward * move.z + fixedCamRight * move.x;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        velocity = moveDirection * speed;
        controller.Move(velocity * Time.deltaTime);

        if (velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        animator.SetFloat("blend", moveInput.magnitude, animationDampTime, Time.deltaTime);
    }
}
