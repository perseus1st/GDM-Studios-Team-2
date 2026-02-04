using UnityEngine;
using UnityEngine.SceneManagement;

public class SisRoomDoor : MonoBehaviour, IInteractable
{
    private Transform interactIcon;
    private int miniGamesCompleted;

    void Start()
    {
        interactIcon = transform.Find("InteractIcon");
        interactIcon.LookAt(Camera.main.transform.position);
        interactIcon.gameObject.SetActive(false);
        miniGamesCompleted = GameManager.Instance.getNumMinigameCompleted();
    }

    public void Interact()
    {
        SceneManager.LoadScene("Cutscene2");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (miniGamesCompleted == 4 && other.TryGetComponent(out PlayerController player))
        {
            player.SetInteractable(this);
            interactIcon.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (miniGamesCompleted == 4 && other.TryGetComponent(out PlayerController player))
        {
            player.SetInteractable(null);
            interactIcon.gameObject.SetActive(false);
        }
    }
}