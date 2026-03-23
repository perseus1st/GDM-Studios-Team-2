using UnityEngine;

public class UIButtonSound : MonoBehaviour
{
    public void PlayClickSound()
    {
        if (AudioManager.INSTANCE != null)
        {
            AudioManager.INSTANCE.PlaySFX("ButtonClick");
        }
    }
}