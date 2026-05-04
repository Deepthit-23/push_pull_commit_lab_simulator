using UnityEngine;

public class FireController : MonoBehaviour
{
    [Header("Fire Settings")]
    public float maxFireIntensity = 100f;
    public float currentFireIntensity = 100f;
    
    [Header("Components")]
    public ParticleSystem fireParticles;
    public Light fireLight;
    
    [Header("Scoring")]
    public int extinguishPoints = 50;
    
    public bool isExtinguished = false;
    private float initialEmissionRate;
    private float initialLightIntensity;
    
    void Start()
    {
        if (fireParticles != null)
        {
            var emission = fireParticles.emission;
            initialEmissionRate = emission.rateOverTime.constant;
        }
        
        if (fireLight != null)
        {
            initialLightIntensity = fireLight.intensity;
        }
    }
    
    public void Extinguish(float amount)
{
    if (isExtinguished)
    {
        Debug.Log("Fire already extinguished, ignoring");
        return;
    }
    
    Debug.Log($"💧 Extinguish called! Amount: {amount:F2}, Current: {currentFireIntensity:F1}");
    
    // Reduce fire intensity
    currentFireIntensity -= amount;
    currentFireIntensity = Mathf.Max(0, currentFireIntensity);
    
    // Calculate intensity percentage
    float intensityPercent = currentFireIntensity / maxFireIntensity;
    
    Debug.Log($"🔥 New intensity: {currentFireIntensity:F1} ({intensityPercent * 100:F0}%)");
    
    // Update particle emission rate
    if (fireParticles != null)
    {
        var emission = fireParticles.emission;
        emission.rateOverTime = initialEmissionRate * intensityPercent;
    }
    
    // Update light intensity
    if (fireLight != null)
    {
        fireLight.intensity = initialLightIntensity * intensityPercent;
    }
    
    // Check if fully extinguished
    if (currentFireIntensity <= 0 && !isExtinguished)
    {
        isExtinguished = true;
        CompleteExtinguish();
    }
}
    
    void CompleteExtinguish()
    {
        Debug.Log("Fire extinguished! +" + extinguishPoints + " points");
        
        // Stop fire
        if (fireParticles != null)
        {
            fireParticles.Stop();
        }
        
        if (fireLight != null)
        {
            fireLight.enabled = false;
        }
        
        // Award points (would connect to score manager)
        // For now just debug log
    }
}