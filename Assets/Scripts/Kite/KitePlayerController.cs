//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class KitePlayerController : MonoBehaviour
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
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log($"Move Input: {moveInput}");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * speed * Time.deltaTime);
    }
}
