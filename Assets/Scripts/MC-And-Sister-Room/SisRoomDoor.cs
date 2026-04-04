
using UnityEngine;
using UnityEngine.SceneManagement;

public class SisRoomDoor : MonoBehaviour, IInteractable
{
    private Transform interactIcon;
    private int miniGamesCompleted;
    private PlayerController playerRef; // Added by Daniil 04-04-2026
    [SerializeField] private Transform doorModel; 

    public AudioSource doorOpenSound;

    void Start()
    {
        interactIcon = transform.Find("InteractIcon");
        interactIcon.LookAt(Camera.main.transform.position);
        interactIcon.gameObject.SetActive(false);
        miniGamesCompleted = GameManager.Instance.getNumMinigameCompleted();
    }

    public void Interact()
    {
        if (playerRef != null) // Added by Daniil 04-04-2026
            playerRef.SetDialogueActive(true); // Stops player movement
        doorOpenSound.Play();
        Invoke("LoadCutscene2", 1f);
    }
    
    private void LoadCutscene2()
    {
        SceneController sceneController = FindAnyObjectByType<SceneController>();
        if (sceneController != null)
            sceneController.StartAnimation("Cutscene2");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (miniGamesCompleted == 3 && other.TryGetComponent(out PlayerController player))
        {
            playerRef = player; // Added by Daniil 04-04-2026
            player.SetInteractable(this);
            interactIcon.gameObject.SetActive(true);
            doorModel.gameObject.layer = 6;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (miniGamesCompleted == 3 && other.TryGetComponent(out PlayerController player))
        {
            player.SetInteractable(null);
            interactIcon.gameObject.SetActive(false);
            doorModel.gameObject.layer = 0;
        }
    }
}