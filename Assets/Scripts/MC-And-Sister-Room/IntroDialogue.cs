using UnityEngine;
using TMPro;

public class IntroDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private PlayerController playerController;

    private bool isDialogueActive = false;

    void Start()
    {
        ShowDialogue();
    }

    private void ShowDialogue()
    {
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        playerController.SetDialogueActive(true);
    }

    private void HideDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        playerController.SetDialogueActive(false);
    }

    public void OnInteractDialogue()
    {
        if (isDialogueActive)
        {
            HideDialogue();
        }
    }
}