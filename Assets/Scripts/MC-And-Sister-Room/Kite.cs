
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Kite : MonoBehaviour, IInteractable
{
    private Transform interactIcon;

    void Start()
    {
        interactIcon = transform.Find("InteractIcon");
        interactIcon.LookAt(Camera.main.transform.position);
        interactIcon.gameObject.SetActive(false);
        if (GameManager.Instance.IsMinigameCompleted("kite"))
        {
            transform.position = new Vector3(-2.61f,0.26f,0.9f);
            transform.Find("ItemModel").gameObject.SetActive(false);
            transform.Find("BoxModel").gameObject.SetActive(true);
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