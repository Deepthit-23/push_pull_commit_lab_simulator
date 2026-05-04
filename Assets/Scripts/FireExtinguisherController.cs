using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FireExtinguisherController : MonoBehaviour
{
    [Header("Components")]
    public ParticleSystem sprayParticles;
    public Transform sprayOrigin;
    
    [Header("Settings")]
    public float sprayRange = 6f;
    public float extinguishRate = 15f;
    [Header("Audio")]
public AudioSource spraySound;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isSpraying = false;
    private bool isGrabbed = false;
    
    
    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
            grabInteractable.activated.AddListener(OnTriggerPressed);
            grabInteractable.deactivated.AddListener(OnTriggerReleased);
        }
        
        if (sprayParticles != null)
        {
            sprayParticles.Stop();
        }
    }
    
    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        Debug.Log("Fire extinguisher grabbed!");
    }
    
    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        StopSpray();
    }
    
    void OnTriggerPressed(ActivateEventArgs args)
    {
        if (isGrabbed)
        {
            StartSpray();
        }
    }
    
    void OnTriggerReleased(DeactivateEventArgs args)
    {
        StopSpray();
    }
    
    

void StartSpray()
{
    if (!isSpraying && sprayParticles != null)
    {
        isSpraying = true;
        sprayParticles.Play();
        
        // Play spray sound
        if (spraySound != null)
        {
            spraySound.Play();
            Debug.Log("🔊 Spray sound playing");
        }
        else
        {
            Debug.LogWarning("⚠️ Spray sound AudioSource is NULL!");
        }
        
        Debug.Log("✅ SPRAYING STARTED!");
    }
}

void StopSpray()
{
    if (isSpraying && sprayParticles != null)
    {
        isSpraying = false;
        sprayParticles.Stop();
        
        // Stop spray sound
        if (spraySound != null)
        {
            spraySound.Stop();
            Debug.Log("🔇 Spray sound stopped");
        }
        
        Debug.Log("⛔ SPRAYING STOPPED!");
    }
}
    
    void Update()
{
    // TEST 1: E key instantly kills ALL fires
    if (Input.GetKeyDown(KeyCode.E))
    {
        Debug.Log("=== E KEY: EMERGENCY EXTINGUISH ALL FIRES ===");
        FireController[] allFires = FindObjectsOfType<FireController>();
        Debug.Log($"Found {allFires.Length} fires in scene");
        
        foreach (FireController fire in allFires)
        {
            Debug.Log($"Extinguishing: {fire.gameObject.name}");
            fire.Extinguish(1000f); // Massive amount
        }
    }
    
    // TEST 2: X key sprays
    if (Input.GetKeyDown(KeyCode.X))
    {
        Debug.Log("=== X KEY: START SPRAY ===");
        StartSpray();
    }
    
    if (Input.GetKeyUp(KeyCode.X))
    {
        Debug.Log("=== X KEY: STOP SPRAY ===");
        StopSpray();
    }
    
    // Spray checking
    if (isSpraying)
    {
        CheckFireExtinguishing();
    }
}
    
    void CheckFireExtinguishing()
{
    if (sprayOrigin == null) return;

    // Simple forward raycast - VERY LONG range for testing
    
    
    // Draw a visible ray in Scene view
    Debug.DrawRay(sprayOrigin.position, sprayOrigin.forward * 20f, Color.red, 1f);
    
    // Check EVERYTHING in front of spray
    RaycastHit[] allHits = Physics.RaycastAll(sprayOrigin.position, sprayOrigin.forward, 20f);
    
    Debug.Log($"=== SPRAY CHECK === Found {allHits.Length} objects");
    
    foreach (RaycastHit h in allHits)
    {
        Debug.Log($"  - Hit: {h.collider.gameObject.name}");
        
        // Check this object AND its parents for FireController
        FireController fire = h.collider.GetComponentInParent<FireController>();
        if (fire == null)
        {
            fire = h.collider.GetComponent<FireController>();
        }
        
        if (fire != null)
        {
            Debug.Log($"🔥 FOUND FIRE! Extinguishing...");
            fire.Extinguish(extinguishRate * Time.deltaTime);
            return; // Found fire, done
        }
    }
    
    Debug.Log("❌ No fire found in spray path");
}
    
    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
            grabInteractable.activated.RemoveListener(OnTriggerPressed);
            grabInteractable.deactivated.RemoveListener(OnTriggerReleased);
        }
    }
}