using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;

    public PlayerInput playerInput;
    private bool isPaused = false;
    private AudioManager audioManager = AudioManager.INSTANCE;

    [SerializeField] string actionMapName = "Gameplay";

    private void Awake()
    {
        // inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        // inputActions.Player.Enable();
        // inputActions.Player.Pause.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        // inputActions.Player.Pause.performed -= OnPausePressed;
        // inputActions.Player.Disable();
    }

    public void OnPausePressed(InputAction.CallbackContext context)
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

        audioManager.PauseMusic();
        playerInput.SwitchCurrentActionMap("Pause");

        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;

        audioManager.UnpauseMusic();
        playerInput.ActivateInput();
        playerInput.SwitchCurrentActionMap(actionMapName);

        isPaused = false;
    }
}