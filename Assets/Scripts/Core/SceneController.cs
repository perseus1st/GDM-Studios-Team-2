using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    // Settings

    public float delayBeforeAnimation = 1f;
    public float animationLength = 3f;

    public string triggerName = "NextLevel";
    public string skipFadeInScene;
    [SerializeField] private Animator canvasAnimator;


    void Awake()
{
    if (canvasAnimator == null)
        canvasAnimator = FindAnyObjectByType<Animator>();

    if (canvasAnimator == null)
    {
        Debug.LogError("CanvasAnimator is missing in the scene!");
        return;
    }

    string currentScene = SceneManager.GetActiveScene().name;

    if (currentScene == "MainMenu")
    {
        // Skip fade-in by forcing the animation to its final frame
        canvasAnimator.Play("Scene_Fade_In", 0, 1f);
        canvasAnimator.Update(0f);
    }
}

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void StartAnimation(string sceneName)
    {
        // Debugging
        Debug.Log("Button pressed");

        
        StartCoroutine(PlayAnimationAndLoadLevel(sceneName));
    }

    private IEnumerator PlayAnimationAndLoadLevel(string sceneName)
    {
        // Debugging
        Debug.Log("Process started");

        yield return new WaitForSeconds(delayBeforeAnimation);

        if (canvasAnimator != null)
        {
            
            canvasAnimator.SetTrigger(triggerName);

            // Debugging
            Debug.Log("Next level transition");
        }

        else
        {
            Debug.LogWarning("Canvas animator is not assigned");
        }

        yield return new WaitForSeconds(animationLength);

        SceneManager.LoadScene(sceneName);
    }
}

