
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rackets : MonoBehaviour, IInteractable
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
        if (GameManager.Instance.IsMinigameCompleted("badminton"))
        {
            itemModel.SetActive(false);
            boxModel.SetActive(true);
            interactIcon.transform.localPosition = new UnityEngine.Vector3(0.57f,-0.232f,0.09f);
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = new UnityEngine.Vector3(0.6741f, -0.273f, -0.14185f);
            boxCollider.size = new UnityEngine.Vector3(0.3008f, 1.548f, 0.5297f);
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