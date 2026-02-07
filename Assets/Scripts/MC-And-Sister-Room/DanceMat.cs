
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DanceMat : MonoBehaviour, IInteractable
{
    private Transform interactIcon;
    private BoxCollider boxCollider;

    void Start()
    {
        interactIcon = transform.Find("InteractIcon");
        interactIcon.LookAt(Camera.main.transform.position);
        interactIcon.gameObject.SetActive(false);
        if (GameManager.Instance.IsMinigameCompleted("ddr"))
        {
            transform.Find("ItemModel").gameObject.SetActive(false);
            transform.Find("BoxModel").gameObject.SetActive(true);
            interactIcon.transform.localPosition = new UnityEngine.Vector3(-0.385f,-0.034f,1.13f);
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = new UnityEngine.Vector3(-0.328f, -0.25f, 1.165f);
            boxCollider.size = new UnityEngine.Vector3(0.718f, 1.5f, 0.809f);
        }
    }

    public void Interact()
    {
        SceneManager.LoadScene("DDR_Minigame");
        //After minigames are done, this line should be removed from this script and included in the minigame scripts
        GameManager.Instance.MarkMinigameCompleted("ddr");
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