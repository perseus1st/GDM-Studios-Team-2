using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Typewriter : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textComponent;
    public GameObject dialoguePanel;

    [Header("Text Settings")]
    [TextArea(2, 5)]
    public List<string> sentences;
    public float typingSpeed = 0.02f;
    public float betweenSentences = 1f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("Player")]
    [SerializeField] private PlayerController playerController;

    private int currentSentenceIndex = 0;
    private Coroutine typingCoroutine;

    private CanvasGroup canvasGroup;
    private bool isDialogueActive = false;

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

        currentSentenceIndex = 0;
        isDialogueActive = true;

        if (playerController != null)
            playerController.SetDialogueActive(true);

        StartSentence();
    }

    private void StartSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        if (currentSentenceIndex >= sentences.Count)
        {
            EndDialogue();
            return;
        }

        typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentenceIndex]));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        textComponent.text = "";

        foreach (char letter in sentence)
        {
            textComponent.text += letter;

            if (letter == '.' || letter == ',' || letter == '!')
                yield return new WaitForSeconds(typingSpeed * 6);
            else
                yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(betweenSentences);

        currentSentenceIndex++;
        StartSentence();
    }
    public void OnInteractDialogue()
    {
        if (isDialogueActive)
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;

        if (playerController != null)
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
}