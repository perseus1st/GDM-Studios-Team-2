
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Kite : MonoBehaviour, IInteractable
{
    private Transform interactIcon;
    private BoxCollider boxCollider;

    void Start()
    {
        interactIcon = transform.Find("InteractIcon");
        interactIcon.LookAt(Camera.main.transform.position);
        interactIcon.gameObject.SetActive(false);
        if (GameManager.Instance.IsMinigameCompleted("kite"))
        {
            transform.Find("ItemModel").gameObject.SetActive(false);
            transform.Find("BoxModel").gameObject.SetActive(true);
            interactIcon.transform.localPosition = new UnityEngine.Vector3(0.507f,-0.126f,1.003f);
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = new UnityEngine.Vector3(0.48f, -0.241f, 1.035f);
            boxCollider.size = new UnityEngine.Vector3(0.634f, 1.48f, 0.784f);
        }
    }

    public void Interact()
    {
        SceneManager.LoadScene("Kite_Minigame");
        //After minigames are done, this line should be removed from this script and included in the minigame scripts
        GameManager.Instance.MarkMinigameCompleted("kite");
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