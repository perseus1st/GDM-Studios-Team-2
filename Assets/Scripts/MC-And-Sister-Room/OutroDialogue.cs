using UnityEngine;
using System.Collections;

public class OutroDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private RectTransform textContainer;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float scrollDuration = 3f;
    [SerializeField] private float scrollStartY = -500f; // how far below the panel the text starts
    [SerializeField] private float scrollEndY = 0f;      // final resting position

    private CanvasGroup canvasGroup;
    private bool isDialogueActive = false;
    private bool scrollComplete = false;

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
        scrollComplete = false;
        isDialogueActive = false; // blocked until scroll finishes

        // Start text below the panel
        textContainer.anchoredPosition = new Vector2(textContainer.anchoredPosition.x, scrollStartY);

        playerController.SetDialogueActive(true);
        StartCoroutine(ScrollText());
    }

    private IEnumerator ScrollText()
    {
        float elapsed = 0f;

        while (elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scrollDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t); // smooth ease in/out
            float newY = Mathf.Lerp(scrollStartY, scrollEndY, smoothT);
            textContainer.anchoredPosition = new Vector2(textContainer.anchoredPosition.x, newY);
            yield return null;
        }

        textContainer.anchoredPosition = new Vector2(textContainer.anchoredPosition.x, scrollEndY);
        scrollComplete = true;
        isDialogueActive = true; // now allow interact to dismiss
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
        if (isDialogueActive && scrollComplete)
        {
            // Transition
            SceneController sceneController = FindAnyObjectByType<SceneController>();
            if (sceneController != null)
                sceneController.StartAnimation("MainMenu");
            else
            {
                Debug.LogWarning("SceneController not found! Loading scene directly without animation.");
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
    }
}