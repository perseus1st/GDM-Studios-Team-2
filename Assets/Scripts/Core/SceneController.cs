using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Animation references
    public Animator canvasAnimator;
    
    // Settings

    public float delayBeforeAnimation = 1f;
    public float animationLength = 3f;

    public string triggerName = "NextLevel";
    public string skipFadeInScene = "Cutscene1";

    void Awake()
    {
        // If canvasAnimator not set in Inspector, find it automatically
        if (canvasAnimator == null)
        {
            canvasAnimator = FindAnyObjectByType<Animator>();
        }

        if (canvasAnimator == null)
            Debug.LogError("CanvasAnimator is missing in the scene!");

    }

    void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
            
        if (currentScene == skipFadeInScene)
        {
            canvasAnimator.Play("Idle_Animation", 0, 0f);
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

