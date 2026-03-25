
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float animationDampTime = 0.05f;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] IInteractable currentInteractable;
    private CharacterController controller;
    private UnityEngine.Vector2 moveInput;
    private UnityEngine.Vector3 velocity;
    private Animator animator;
    private UnityEngine.Vector3 fixedCamForward;
    private UnityEngine.Vector3 fixedCamRight;

    private const float sqrt2 = 1.189207f;
    private bool dialogueActive = false; // Added by Daniil
    private IntroDialogue introDialogue; // Added by Daniil

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip footstepClip;

    [SerializeField] private float footstepVolume = 1f;


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

        introDialogue = FindObjectOfType<IntroDialogue>(); // Added by Daniil

        if (footstepSource != null)
        {
        footstepSource.clip = footstepClip;
        footstepSource.loop = true;
        footstepSource.playOnAwake = false;
        footstepSource.volume = footstepVolume;
        }        
    }

    public void onMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<UnityEngine.Vector2>();
        // moveInput.x = moveInput.x/(sqrt2) + moveInput.y/(sqrt2);
        // moveInput.y = moveInput.x/(sqrt2) + moveInput.y/(sqrt2);



    }

    public void onInteract(InputAction.CallbackContext context) // Edited by Daniil
    {
         if (context.performed)
         {
             if (dialogueActive)
             {
                 if (introDialogue != null)
                    introDialogue.OnInteractDialogue();
                 return;
             }
             if (currentInteractable != null)
             {
                currentInteractable.Interact();
            }
        }
    }

    public void SetInteractable(IInteractable interactable)
    {
        if (interactable == null) Debug.Log("interactable removed!");
        else Debug.Log("interactble set!");
        
        currentInteractable = interactable;
    }

    public void SetDialogueActive(bool active) // Added by Daniil
    {
        dialogueActive = active;
    }

    void HandleFootsteps(bool isMoving)
    {
        if (footstepSource == null || footstepClip == null)
            return;

        if (isMoving)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (dialogueActive)
        {
            animator.SetFloat("blend", 0f, animationDampTime, Time.deltaTime);
            HandleFootsteps(false); // stop footsteps during dialogue
            return;
        }

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        Vector3 moveDirection = fixedCamForward * move.z + fixedCamRight * move.x;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        velocity = moveDirection * speed;
        controller.Move(velocity * Time.deltaTime);
        bool isMoving = velocity.sqrMagnitude > 0.01f;
        HandleFootsteps(isMoving);

        if (velocity.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        animator.SetFloat("blend", moveInput.magnitude, animationDampTime, Time.deltaTime);
    }
}
