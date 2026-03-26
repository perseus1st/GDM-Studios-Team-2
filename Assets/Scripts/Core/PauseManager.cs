using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;

    public PlayerInput playerInput;
    private bool isPaused = false;
    private AudioManager audioManager = AudioManager.INSTANCE;
    public SceneController sceneController;

    public string inputMapName = "Gameplay";

    private void Awake()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        // inputActions = new PlayerInputActions();
        // If sceneController not set in Inspector, find it automatically
        if (sceneController == null)
        {
            sceneController = FindAnyObjectByType<SceneController>();
        }

        if (sceneController == null)
            Debug.LogError("SceneController is missing in the scene!");
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
        AudioListener.pause = true;

        playerInput.SwitchCurrentActionMap("Pause");
        isPaused = true;

        if (AudioManager.INSTANCE != null)
            AudioManager.INSTANCE.PauseMusic();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;

        playerInput.SwitchCurrentActionMap(inputMapName);
        isPaused = false;

        if (AudioManager.INSTANCE != null)
            AudioManager.INSTANCE.ResumeMusic();
    }

    public void quit()
    {
        //check active scene
        Scene currentScene = SceneManager.GetActiveScene(); 
        if (currentScene.name == "Sister_Room" || currentScene.name == "MC_Room")
        {
            Debug.Log("quitting game");
            Application.Quit();
        } else
        {
            playerInput.SwitchCurrentActionMap(inputMapName);
            sceneController.StartAnimation("Sister_Room");
        }
    }
}