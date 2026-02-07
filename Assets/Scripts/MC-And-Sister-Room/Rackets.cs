
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rackets : MonoBehaviour, IInteractable
{
    private Transform interactIcon;
    private BoxCollider boxCollider;

    void Start()
    {
        interactIcon = transform.Find("InteractIcon");
        interactIcon.LookAt(Camera.main.transform.position);
        interactIcon.gameObject.SetActive(false);
        if (GameManager.Instance.IsMinigameCompleted("badminton"))
        {
            transform.Find("ItemModel").gameObject.SetActive(false);
            transform.Find("BoxModel").gameObject.SetActive(true);
            interactIcon.transform.localPosition = new UnityEngine.Vector3(0.57f,-0.232f,0.09f);
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = new UnityEngine.Vector3(0.552f, -0.273f, -0.0417f);
            boxCollider.size = new UnityEngine.Vector3(0.545f, 1.546f, 0.73f);
        }
    }

    public void Interact()
    {
        SceneManager.LoadScene("Badminton_Minigame");
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