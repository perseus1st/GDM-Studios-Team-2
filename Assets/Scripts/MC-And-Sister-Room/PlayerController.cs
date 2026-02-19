
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private IInteractable currentInteractable;
    private CharacterController controller;
    private UnityEngine.Vector2 moveInput;
    private UnityEngine.Vector3 velocity;

    private const float sqrt2 = 1.189207f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
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
        Vector3 diagonalMove = Quaternion.Euler(0, 35, 0) * move;
        controller.Move(diagonalMove * speed * Time.deltaTime);
    }
}
