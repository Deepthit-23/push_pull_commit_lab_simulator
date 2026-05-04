using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class WearableItem : MonoBehaviour
{
    [Header("Attach Settings")]
    [Tooltip("Where this item should attach on the player")]
    public Transform attachPoint;
    
    [Tooltip("Type of PPE equipment")]
    public PPEType ppeType;
    
    [Header("Status")]
    public bool isWorn = false;
    
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private Collider itemCollider;
    private Transform originalParent;
    
    // Enum for PPE types
    public enum PPEType
    {
        LabCoat,
        Goggles,
        Glove
    }
    
    void Start()
    {
        // Get components
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
        originalParent = transform.parent;
        
        // Subscribe to grab events
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }
    
    void OnGrab(SelectEnterEventArgs args)
{
    Debug.Log($"Grabbed {ppeType}");
    
    if (AudioManager.instance != null)
    {
        AudioManager.instance.PlayGrab();
    }
}
    
    void OnRelease(SelectExitEventArgs args)
    {
        // Called when player releases the item
        Debug.Log($"Released {ppeType}");
        
        // Check if released near attach point
        if (attachPoint != null)
        {
            float distanceToAttachPoint = Vector3.Distance(transform.position, attachPoint.position);
            
            // If within 0.3 meters of attach point, snap to it
            if (distanceToAttachPoint < 0.6f && !isWorn)
            {
                WearItem();
            }
        }
    }
    
    void WearItem()
    {
        if (AudioManager.instance != null)
{
    AudioManager.instance.PlaySound(AudioManager.instance.attach, 0.8f);
}
        Debug.Log($"Wearing {ppeType}!");
        
        // Disable physics
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        // Make item invisible when worn
    MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
    if (meshRenderer != null)
    {
        meshRenderer.enabled = false;
    }
    
    // Also disable child renderers (like goggles parts)
    MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
    foreach (MeshRenderer renderer in childRenderers)
    {
        renderer.enabled = false;
    }
        
        // Disable grabbing
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }
        
        // Disable collider
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }
        
        // Attach to player
        transform.SetParent(attachPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        // Mark as worn
        isWorn = true;
        
      // Notify PPE Manager (we'll create this next)
        PPEManager manager = FindObjectOfType<PPEManager>();
       if (manager != null)
        {
            manager.OnItemWorn(ppeType);
        }
        ParticleSystem effect = attachPoint.GetComponentInChildren<ParticleSystem>();
    if (effect != null)
    {
        effect.Play();
    }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}