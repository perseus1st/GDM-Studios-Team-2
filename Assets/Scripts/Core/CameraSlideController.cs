using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSlideController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;
    public float slideSpeed;

    [Header("Grid Settings")]
    public int columns;
    public int rows;
    public int missingPanels;

    [Header("Panel SFX")]
    public string[] panelSFX; 

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private int clickCount = 0;

    public SceneController sceneController;

    // Exact spacing based on your panel coordinates
    private float cellWidth = 400f;    // X difference between columns
    private float cellHeight = 275f;   // Y difference between rows

    // Input System
    public InputActionReference nextPanelAction;

    void Awake()
    {
        sceneController = FindFirstObjectByType<SceneController>();
        if (sceneController == null) {
            Debug.LogError("No SceneController found in the scene!");
        }
    }

    void OnEnable()
    {
        if (nextPanelAction != null && nextPanelAction.action != null)
        {
            nextPanelAction.action.Enable();
            nextPanelAction.action.performed += OnNextPanel;
        }
        else
        {
            Debug.LogError("NextPanel Action is not assigned");
        }
    }

    void OnDisable()
    {
        if (nextPanelAction != null && nextPanelAction.action != null)
            nextPanelAction.action.performed -= OnNextPanel;
    }

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        startPosition = mainCamera.transform.position;
        targetPosition = startPosition;
    }

    void Update()
    {
        if (isMoving)
        {
            Vector3 currentPos = mainCamera.transform.position;
            float distance = Vector3.Distance(currentPos, targetPosition);

            // Dynamic speed: proportional to distance
            float dynamicSpeed = Mathf.Clamp(distance * 5f, 500f, 1500f); 
            // distance*5f = bigger distance → faster
            // min = 500 units/sec, max = 1500 units/sec

            mainCamera.transform.position = Vector3.MoveTowards(currentPos, targetPosition, dynamicSpeed * Time.deltaTime);

            // Snap to target if very close
            if (Vector3.Distance(mainCamera.transform.position, targetPosition) < 0.01f)
            {
                mainCamera.transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    private void OnNextPanel(InputAction.CallbackContext context)
    {
        if (isMoving) return;

        int totalPanels = columns * rows - missingPanels;

        // If we are already at the last panel → go to next scene
        if (clickCount >= totalPanels - 1)
        {
            sceneController.StartAnimation("MC_Room");
            return;
        }

        // Otherwise move to the next panel
        clickCount++;

        int row = clickCount / columns;
        int col = clickCount % columns;

        targetPosition = new Vector3(startPosition.x + cellWidth * col, startPosition.y - cellHeight * row, startPosition.z);

        isMoving = true;

        // Play SFX if assigned
        if (panelSFX != null && clickCount < panelSFX.Length)
        {
            string sfxName = panelSFX[clickCount];
            if (!string.IsNullOrEmpty(sfxName))
            {
                Debug.Log("num: " + clickCount);
                Debug.Log("name: " + sfxName);
                Cutscene1AudioManager.INSTANCE.PlaySFX(sfxName);
                // Cutscene1AudioManager.INSTANCE.PlaySFX(sfxName);
            }
        }
    }
}