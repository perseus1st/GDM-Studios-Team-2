using UnityEngine;

public class BadmintonPlayerAnimationEvent : MonoBehaviour
{
    private BadmintonPlayerController playerController;

    void Start()
    {
        // Get the parent's controller
        playerController = GetComponentInParent<BadmintonPlayerController>();
    }

    // Called by animation event
    public void OnHitAnimationComplete()
    {
        if (playerController != null)
        {
            playerController.OnHitAnimationComplete();
        }
    }
}
