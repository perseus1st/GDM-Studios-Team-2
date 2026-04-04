using UnityEngine;
using System.Collections;

public class OutroDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private RectTransform textContainer;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float scrollDuration = 3f;
    [SerializeField] private float fastScrollDuration = 0.5f; // duration when sped up
    [SerializeField] private float scrollStartY = -500f;
    [SerializeField] private float scrollEndY = 0f;

    private CanvasGroup canvasGroup;
    private bool isDialogueActive = false;
    private bool scrollComplete = false;
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
        scrollComplete = false;
        isDialogueActive = true; // allow first press immediately
        isFast = false;

        textContainer.anchoredPosition = new Vector2(textContainer.anchoredPosition.x, scrollStartY);
        playerController.SetDialogueActive(true);
        StartCoroutine(ScrollText());
    }

    private IEnumerator ScrollText()
    {
        float elapsed = 0f;

        while (elapsed < (isFast ? fastScrollDuration : scrollDuration))
        {
            elapsed += Time.deltaTime;
            float duration = isFast ? fastScrollDuration : scrollDuration;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.Lerp(0f, 1f, t);
            float newY = Mathf.Lerp(scrollStartY, scrollEndY, smoothT);
            textContainer.anchoredPosition = new Vector2(textContainer.anchoredPosition.x, newY);
            yield return null;
        }

        textContainer.anchoredPosition = new Vector2(textContainer.anchoredPosition.x, scrollEndY);
        scrollComplete = true;
    }

    public void OnInteractDialogue()
    {
        if (!isDialogueActive) return;

        if (!scrollComplete && !isFast)
        {
            // First press — speed up scroll
            isFast = true;
        }
        else if (scrollComplete || isFast)
        {
            // Second press — trigger scene transition
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