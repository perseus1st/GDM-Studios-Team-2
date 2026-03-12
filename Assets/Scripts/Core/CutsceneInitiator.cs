using UnityEngine;

public class CutsceneInitiator : MonoBehaviour
{
    private CutsceneHandler cutsceneHandler;

    public void Start()
    {
        cutsceneHandler = GetComponent<CutsceneHandler>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            // Here the implementation is for a player colliding with a certain thing. 
            // I need to check what the if condition needs to be for a cutscene that gets triggered on entry of the unity scene
            cutsceneHandler.PlayNextElement();
        }
        
    }
}
