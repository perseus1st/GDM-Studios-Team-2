
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MCRoomDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject doorObject;
    [SerializeField] private GameObject interactIconObject;

    private Transform interactIcon;
    private Transform doorModel;
    public SceneController sceneController;

    void Start()
    {
        doorModel = doorObject.transform;
        interactIcon = interactIconObject.transform;
        interactIcon.LookAt(Camera.main.transform.position);
        interactIcon.Rotate(0, 180, 0);
        interactIcon.gameObject.SetActive(false);
    }

    public void Interact()
    {
        sceneController.StartAnimation("Sister_Room");
        // SceneManager.LoadScene("Sister_Room");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.SetInteractable(this);
            interactIcon.gameObject.SetActive(true);
            doorModel.gameObject.layer = 6;
            interactIconObject.layer = 5;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.SetInteractable(null);
            interactIcon.gameObject.SetActive(false);
            doorModel.gameObject.layer = 0;
        }
    }
}