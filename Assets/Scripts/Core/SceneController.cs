using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    // Settings

    public float delayBeforeAnimation = 0f;
    public float animationLength = 2f;

    public string triggerName = "NextLevel";
    public string skipFadeInScene;
    [SerializeField] private Animator canvasAnimator;
    private string currentScene;

    void Awake()
{
    if (canvasAnimator == null)
        canvasAnimator = FindAnyObjectByType<Animator>();

    if (canvasAnimator == null)
    {
        Debug.LogError("CanvasAnimator is missing in the scene!");
        return;
    }

    currentScene = SceneManager.GetActiveScene().name;

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
        // Debug.Log("Button pressed");
        
        StartCoroutine(PlayAnimationAndLoadLevel(sceneName));
    }

    private IEnumerator PlayAnimationAndLoadLevel(string sceneName)
    {
        // Debugging
        // Debug.Log("Process started");

        yield return new WaitForSecondsRealtime(delayBeforeAnimation);

        if (canvasAnimator != null)
        {
            
            canvasAnimator.SetTrigger(triggerName);

            // Debugging
            // Debug.Log("Next level transition");
        }

        else
        {
            Debug.LogWarning("Canvas animator is not assigned");
        }

        yield return new WaitForSecondsRealtime(animationLength);

        Debug.Log("Current: " + currentScene + ", next: " + sceneName);
        if ((currentScene == "MC_Room" && sceneName == "Sister_Room") || (currentScene == "Sister_Room" && sceneName == "Cutscene2"))
        {
            AudioManager.INSTANCE.PlaySFX("DoorClose");
        } 
        else if ((currentScene == "DDR_Minigame" 
                || currentScene == "Dodgeball_Minigame" 
                || currentScene == "Badminton_Minigame" 
                || currentScene == "Kite_Minigame") 
                && sceneName == "Sister_Room")
        {
            AudioManager.INSTANCE.PlaySFX("Box");
        }

        SceneManager.LoadScene(sceneName);
    }
}

