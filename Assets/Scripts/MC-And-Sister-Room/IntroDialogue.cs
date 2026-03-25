using UnityEngine;
using TMPro;
using System.Collections;

public class IntroDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private TextMeshProUGUI dialogueText1;
    [SerializeField] private TextMeshProUGUI dialogueText2;
    [SerializeField] private TextMeshProUGUI dialogueText3;

    [SerializeField] private string fullText1 = "Line one.";
    [SerializeField] private string fullText2 = "Line two.";
    [SerializeField] private string fullText3 = "Line three.";

    [SerializeField] private float normalCharDelay = 0.05f;
    [SerializeField] private float fastCharDelay = 0.01f;
    [SerializeField] private float fadeDuration = 1f;

    private CanvasGroup canvasGroup;
    private bool isDialogueActive = false;
    private bool textComplete = false;
    private bool isFast = false;

    void Start()
    {
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
        textComplete = false;
        isFast = false;
        isDialogueActive = true;

        dialogueText1.text = "";
        dialogueText2.text = "";
        dialogueText3.text = "";

        playerController.SetDialogueActive(true);
        StartCoroutine(TypeAllText());
    }

    private IEnumerator TypeText(TextMeshProUGUI textElement, string fullText)
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            textElement.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(isFast ? fastCharDelay : normalCharDelay);
        }
    }

    private IEnumerator TypeAllText()
    {
        // Type each section sequentially
        yield return StartCoroutine(TypeText(dialogueText1, fullText1));
        yield return StartCoroutine(TypeText(dialogueText2, fullText2));
        yield return StartCoroutine(TypeText(dialogueText3, fullText3));

        textComplete = true;
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
        if (!isDialogueActive) return;

        if (!textComplete && !isFast)
        {
            // First press — speed up
            isFast = true;
        }
        else if (textComplete || isFast)
        {
            // Second press — dismiss
            StopAllCoroutines();
            dialogueText1.text = fullText1;
            dialogueText2.text = fullText2;
            dialogueText3.text = fullText3;
            HideDialogue();
        }
    }
}