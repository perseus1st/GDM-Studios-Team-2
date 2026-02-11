
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dodgeball : MonoBehaviour, IInteractable
{
    private Transform interactIcon;
    private BoxCollider boxCollider;
    private GameObject itemModel;
    private GameObject boxModel;

    void Start()
    {
        itemModel = transform.Find("ItemModel").gameObject;
        boxModel = transform.Find("BoxModel").gameObject;
        interactIcon = transform.Find("InteractIcon");
        interactIcon.LookAt(Camera.main.transform.position);
        interactIcon.gameObject.SetActive(false);
        if (GameManager.Instance.IsMinigameCompleted("dodgeball"))
        {
            itemModel.SetActive(false);
            boxModel.SetActive(true);
            interactIcon.transform.localPosition = new UnityEngine.Vector3(-0.38f,-0.106f,0.083f);
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = new UnityEngine.Vector3(-0.38f, -0.286f, -0.031f);
            boxCollider.size = new UnityEngine.Vector3(0.686f, 1.573f, 0.87f);
        }
    }

    public void Interact()
    {
        SceneManager.LoadScene("Dodgeball_Minigame");
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