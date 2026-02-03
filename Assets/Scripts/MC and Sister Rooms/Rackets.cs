
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rackets : MonoBehaviour, IInteractable
{
    private Transform interactIcon;

    void Start()
    {
        interactIcon = transform.Find("InteractIcon");
        interactIcon.LookAt(Camera.main.transform.position);
        interactIcon.gameObject.SetActive(false);
        if (GameManager.Instance.IsMinigameCompleted("badminton"))
        {
            transform.position = new Vector3(-1.97f,0.26f,1.04f);
            transform.Find("ItemModel").gameObject.SetActive(false);
            transform.Find("BoxModel").gameObject.SetActive(true);
        }
    }

    public void Interact()
    {
        SceneManager.LoadScene("Badminton_Minigame");
        UnityEngine.Debug.Log(GameManager.Instance.highScores.ContainsKey("badminton"));
        UnityEngine.Debug.Log(GameManager.Instance.highScores["badminton"]);
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