using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HoverGlow : MonoBehaviour
{
    public Material glowMaterial;

    private Material[] originalMaterials;
    private MeshRenderer[] renderers;
    private XRBaseInteractable interactable;

    void Start()
    {
        // Get ALL renderers (including children)
        renderers = GetComponentsInChildren<MeshRenderer>();

        // Store original materials
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].material;
        }

        interactable = GetComponent<XRBaseInteractable>();

        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
        }
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        Debug.Log("👀 Hovering " + gameObject.name);

        foreach (var r in renderers)
        {
            r.material = glowMaterial;
        }
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originalMaterials[i];
        }
    }
}