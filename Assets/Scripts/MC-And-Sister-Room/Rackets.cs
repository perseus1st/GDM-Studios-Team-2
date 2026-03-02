
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rackets : MonoBehaviour, IInteractable
{
    private Transform interactIcon;
    private GameObject itemModel;
    private BoxCollider boxCollider;
    private GameObject boxModel;
    public SceneController sceneController;

    void Awake()
    {
        // If sceneController not set in Inspector, find it automatically
        if (sceneController == null)
        {
            sceneController = FindAnyObjectByType<SceneController>();
        }

        if (sceneController == null)
            Debug.LogError("SceneController is missing in the scene!");
    }


    void Start()
    {
        itemModel = transform.Find("ItemModel").gameObject;
        boxModel = transform.Find("BoxModel").gameObject;
        interactIcon = transform.Find("InteractIcon");
        interactIcon.LookAt(Camera.main.transform.position);
        interactIcon.gameObject.SetActive(false);
        if (GameManager.Instance.IsMinigameCompleted("badminton"))
        {
            itemModel.SetActive(false);
            boxModel.SetActive(true);
            interactIcon.transform.localPosition = new UnityEngine.Vector3(0.57f,-0.232f,0.09f);
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = new UnityEngine.Vector3(0.552f, -0.273f, -0.0417f);
            boxCollider.size = new UnityEngine.Vector3(0.545f, 1.546f, 0.73f);
        }
    }

    public void Interact()
    {
        sceneController.StartAnimation("Badminton_Minigame");
        // SceneManager.LoadScene("Badminton_Minigame");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.SetInteractable(this);
            interactIcon.gameObject.SetActive(true);
            itemModel.layer = 6;
            boxModel.layer = 6;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.SetInteractable(null);
            interactIcon.gameObject.SetActive(false);
            itemModel.layer = 0;
            boxModel.layer = 0;
        }
    }
}