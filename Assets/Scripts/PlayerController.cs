//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private CharacterController controller;
    private UnityEngine.Vector2 moveInput;
    private UnityEngine.Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
    }

    public void onMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<UnityEngine.Vector2>();
        UnityEngine.Debug.Log($"Move Input: {moveInput}");

    }

    public void onInteract(InputAction.CallbackContext context)
    {
        UnityEngine.Debug.Log("LMB clicked");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * speed * Time.deltaTime);
    }
}
