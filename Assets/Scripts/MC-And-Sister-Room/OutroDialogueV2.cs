using UnityEngine;
using TMPro;
using System.Collections;

public class OutroDialogueV2 : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI endOfGameText;
    [SerializeField] private float normalCharDelay = 0.05f;
    [SerializeField] private float fastCharDelay = 0.005f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private RectTransform interactIcon;
    [SerializeField] private float iconScaleAmount = 0.8f;
    [SerializeField] private float iconScaleDuration = 0.1f;

    [SerializeField] private string[] sentences;

    private int currentSentence = 0;
    private bool isDialogueActive = false;
    private bool sentenceComplete = false;
    private bool isFast = false;
    private CanvasGroup canvasGroup;
    private Coroutine typingCoroutine;

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
        currentSentence = 0;
        isDialogueActive = true;
        playerController.SetDialogueActive(true);
        typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentence]));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        sentenceComplete = false;
        isFast = false;
        dialogueText.text = "";

        for (int i = 0; i <= sentence.Length; i++)
        {
            dialogueText.text = sentence.Substring(0, i);
            yield return new WaitForSeconds(isFast ? fastCharDelay : normalCharDelay);
        }

        sentenceComplete = true;
    }

    public void OnInteractDialogue()
    {
        if (interactIcon != null)
            StartCoroutine(PulseIcon());

        if (!isDialogueActive) return;

        if (!sentenceComplete)
        {
            // Sentence still typing — complete it instantly
            isFast = true;
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            dialogueText.text = sentences[currentSentence];
            sentenceComplete = true;
        }
        else
        {
            // Sentence complete — advance to next or dismiss
            currentSentence++;

            if (currentSentence < sentences.Length)
                typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentence]));
            else
            {
                isDialogueActive = false;
                playerController.SetDialogueActive(false);
                SceneController sceneController = FindAnyObjectByType<SceneController>();
                if (sceneController != null)
                    sceneController.StartAnimation("MainMenu");
                else
                {
                    Debug.LogWarning("SceneController not found! Loading scene directly.");
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                }
            }
        }
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
    
    private IEnumerator PulseIcon()
    {
        Vector3 originalScale = interactIcon.localScale;
        Vector3 targetScale = originalScale * iconScaleAmount;

        float elapsed = 0f;
        while (elapsed < iconScaleDuration)
        {
            elapsed += Time.deltaTime;
            interactIcon.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / iconScaleDuration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < iconScaleDuration)
        {
            elapsed += Time.deltaTime;
            interactIcon.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / iconScaleDuration);
            yield return null;
        }
    
        interactIcon.localScale = originalScale;
    }
}