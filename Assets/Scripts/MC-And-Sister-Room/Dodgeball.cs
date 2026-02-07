
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dodgeball : MonoBehaviour, IInteractable
{
    private Transform interactIcon;
    private BoxCollider boxCollider;

    void Start()
    {
        interactIcon = transform.Find("InteractIcon");
        interactIcon.LookAt(Camera.main.transform.position);
        interactIcon.gameObject.SetActive(false);
        if (GameManager.Instance.IsMinigameCompleted("dodgeball"))
        {
            transform.Find("ItemModel").gameObject.SetActive(false);
            transform.Find("BoxModel").gameObject.SetActive(true);
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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            player.SetInteractable(null);
            interactIcon.gameObject.SetActive(false);
        }
    }
}