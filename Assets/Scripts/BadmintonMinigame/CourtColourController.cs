// Created by Daniil Makarenko

using UnityEngine;

public class CourtColorController : MonoBehaviour
{

    [Header("References")]
    public Material BadmintonColorGradingMaterial;
    
    [Header("Color Settings")]
    public Color darkColor = new Color(24f, 0.0f, 42f, 255f);
    public float darkenAmount = 0.7f; // 0 = no darkening, 1 = black
    public float desaturateAmount = 0.8f; // 0 = full color, 1 = grayscale
    public float darknessHold1 = 0.33f; // Full darkness below this value
    public float darknessHold2 = 0.66f; // Half darkness below this value
    
    [Header("Transition Settings")]
    public float transitionSpeedIncrease = 0.5f; // Speed when score increases
    public float transitionSpeedDecrease = 2f;   // Speed when score decreases (faster)
    
    [Header("Court Position (Screen Space 0-1)")]
    public float courtCenterX = 0.5f;
    public float courtCenterY = 0.5f;
    public float innerRadiusX = 0.3f;
    public float outerRadiusX = 0.6f;
    public float innerRadiusY = 0.3f;
    public float outerRadiusY = 0.6f;
    public float fadeSmoothness = 1.5f;
    
    [Header("Court Bounds (Screen Space 0-1)")]
    public float courtMinX = 0.2f;
    public float courtMaxX = 0.8f;
    public float courtMinY = 0.1f;
    public float courtMaxY = 0.9f;
    
    private float currentProgress = 0f;
    private float targetProgress = 0f;
    private float newTarget = 0f;
    

    
    void Start()
    {
        if (BadmintonColorGradingMaterial != null)
        {
            UpdateMaterialProperties();
        }
        
        // Subscribe to score changes if ScoreManager exists
        if (ScoreManager.Instance != null)
        {
            // You'll need to add an event to ScoreManager for this
            // Or call UpdateProgress() when score changes
        }
    }
    
    void Update()
    {
        // Smooth transition to target progress
        if (Mathf.Abs(currentProgress - targetProgress) > 0.001f)
        {
            float speed = (targetProgress > currentProgress) ? transitionSpeedIncrease : transitionSpeedDecrease;
            currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * speed);
            
            if (BadmintonColorGradingMaterial != null)
            {
                BadmintonColorGradingMaterial.SetFloat("_ProgressAmount", currentProgress);
            }
        }
        
        // Update progress based on score
        UpdateProgressFromScore();
    }
    
    void UpdateProgressFromScore()
    {
        if (ScoreManager.Instance == null) return;
        
        int currentScore = ScoreManager.Instance.GetScore();
        int maxScore = ScoreManager.Instance.scoreToComplete;
        
        // Calculate progress (0 to 1)
        float linearProgress = ((float)currentScore / maxScore);
        if (linearProgress <= darknessHold1)
        {
        newTarget = 0;
        }
        else if (linearProgress <= darknessHold2)
        {
        newTarget = darknessHold1;
        }
        else 
        {
        newTarget = darknessHold1 + Mathf.InverseLerp(darknessHold2, 1f, linearProgress);
        }
        
	newTarget = Mathf.Clamp(newTarget,0f,1f);
	
        if (newTarget != targetProgress)
        {
            targetProgress = newTarget;
        }
    }
    
    void UpdateMaterialProperties()
    {
        if (BadmintonColorGradingMaterial == null) return;
        
        BadmintonColorGradingMaterial.SetColor("_DarkColor", darkColor);
        BadmintonColorGradingMaterial.SetFloat("_DarkenAmount", darkenAmount);
        BadmintonColorGradingMaterial.SetFloat("_DesaturateAmount", desaturateAmount);
        BadmintonColorGradingMaterial.SetFloat("_InnerRadiusX", innerRadiusX);
        BadmintonColorGradingMaterial.SetFloat("_OuterRadiusX", outerRadiusX);
        BadmintonColorGradingMaterial.SetFloat("_InnerRadiusY", innerRadiusY);
        BadmintonColorGradingMaterial.SetFloat("_OuterRadiusY", outerRadiusY);
        BadmintonColorGradingMaterial.SetFloat("_FadeSmoothness", fadeSmoothness);
        BadmintonColorGradingMaterial.SetFloat("_CourtCenterX", courtCenterX);
        BadmintonColorGradingMaterial.SetFloat("_CourtCenterY", courtCenterY);
        BadmintonColorGradingMaterial.SetFloat("_ProgressAmount", currentProgress);
        BadmintonColorGradingMaterial.SetFloat("_CourtMinX", courtMinX);
        BadmintonColorGradingMaterial.SetFloat("_CourtMaxX", courtMaxX);
        BadmintonColorGradingMaterial.SetFloat("_CourtMinY", courtMinY);
        BadmintonColorGradingMaterial.SetFloat("_CourtMaxY", courtMaxY);
    }
    
    void OnValidate()
    {
        // Update in editor when values change
        if (BadmintonColorGradingMaterial != null)
        {
            UpdateMaterialProperties();
        }
    }
}