
using UnityEngine;
using UnityEngine.SceneManagement;

public class SisRoomDoor : MonoBehaviour, IInteractable
{
    private Transform interactIcon;
    private int miniGamesCompleted;
    private Transform doorModel;

    void Start()
    {
        interactIcon = transform.Find("InteractIcon");
        doorModel = transform.Find("model");
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
        if (miniGamesCompleted == 3 && other.TryGetComponent(out PlayerController player))
        {
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