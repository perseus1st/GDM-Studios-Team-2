
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dodgeball : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform interactIcon;
    [SerializeField] private GameObject itemModel;
    [SerializeField] private GameObject boxModel;
    private BoxCollider boxCollider;
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
        //itemModel = transform.Find("ItemModel").gameObject;
        //boxModel = transform.Find("BoxModel").gameObject;
        //interactIcon = transform.Find("InteractIcon");
        interactIcon.LookAt(Camera.main.transform.position);
        interactIcon.gameObject.SetActive(false);
        if (GameManager.Instance.IsMinigameCompleted("dodgeball"))
        {
            itemModel.SetActive(false);
            boxModel.SetActive(true);
            interactIcon.transform.localPosition = new UnityEngine.Vector3(-0.492f,-0.234f,-0.451f);
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = new UnityEngine.Vector3(-0.483445f, -0.286f, -0.256552f);
            boxCollider.size = new UnityEngine.Vector3(0.4788f, 1.573f, 0.4189f);
        }
    }

    public void Interact()
    {
        // SceneManager.LoadScene("Dodgeball_Minigame");
        sceneController.StartAnimation("Dodgeball_Minigame");
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