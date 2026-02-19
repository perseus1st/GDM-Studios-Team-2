
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DanceMat : MonoBehaviour, IInteractable
{
    private Transform interactIcon;
    private BoxCollider boxCollider;
    private GameObject itemModel;
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
        if (GameManager.Instance.IsMinigameCompleted("DDR"))
        {
            itemModel.SetActive(false);
            boxModel.SetActive(true);
            interactIcon.transform.localPosition = new UnityEngine.Vector3(-0.385f,-0.034f,1.13f);
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = new UnityEngine.Vector3(-0.328f, -0.25f, 1.165f);
            boxCollider.size = new UnityEngine.Vector3(0.69f, 1.5f, 0.809f);
        }
    }

    public void Interact()
    {
        // SceneManager.LoadScene("DDR_Minigame");
        sceneController.StartAnimation("DDR_Minigame");
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