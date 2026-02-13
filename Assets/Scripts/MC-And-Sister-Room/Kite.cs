
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Kite : MonoBehaviour, IInteractable
{
    private Transform interactIcon;
    private BoxCollider boxCollider;
    private GameObject itemModel;
    private GameObject boxModel;
    public SceneController sceneController;
    public static GameManager INSTANCE;

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
        if (GameManager.Instance.IsMinigameCompleted("kite"))
        {
            itemModel.SetActive(false);
            boxModel.SetActive(true);
            interactIcon.transform.localPosition = new UnityEngine.Vector3(0.507f,-0.126f,1.003f);
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = new UnityEngine.Vector3(0.48f, -0.241f, 1.035f);
            boxCollider.size = new UnityEngine.Vector3(0.634f, 1.48f, 0.784f);
        }
    }

    public void Interact()
    {
        Debug.Log("Interact called!");

        if (sceneController == null)
            Debug.LogError("sceneController is null!");

        sceneController.StartAnimation("Kite_Minigame");
        // SceneManager.LoadScene("Kite_Minigame");
        //After minigames are done, this line should be removed from this script and included in the minigame scripts
        GameManager.Instance.MarkMinigameCompleted("kite");
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