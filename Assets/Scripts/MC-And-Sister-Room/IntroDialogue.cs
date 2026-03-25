using UnityEngine;
using System.Collections;

public class IntroDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float fadeDuration = 1f;

    private CanvasGroup canvasGroup;
    private bool isDialogueActive = false;

    void Start()
    {
        // Add a CanvasGroup to the panel if it doesn't have one
        canvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = dialoguePanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 1f;
        ShowDialogue();
    }

    private void ShowDialogue()
    {
        dialoguePanel.SetActive(true);
        canvasGroup.alpha = 1f;
        isDialogueActive = true;
        playerController.SetDialogueActive(true);
    }

    private void HideDialogue()
    {
        isDialogueActive = false;
        playerController.SetDialogueActive(false);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        dialoguePanel.SetActive(false);
    }

    public void OnInteractDialogue()
    {
        if (isDialogueActive)
        {
            HideDialogue();
        }
    }
}