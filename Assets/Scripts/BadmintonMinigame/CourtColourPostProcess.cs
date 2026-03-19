// Made by Daniil Makarenko

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CourtColorPostProcess : MonoBehaviour
{
    [Header("Effect Material")]
    public Material effectMaterial;
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (effectMaterial != null)
        {
            // Apply the shader as a post-process effect
            Graphics.Blit(source, destination, effectMaterial);
        }
        else
        {
            // No material, just copy source to destination
            Graphics.Blit(source, destination);
        }
    }
}