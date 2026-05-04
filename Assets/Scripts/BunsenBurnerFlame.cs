using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Attach this to the Bunsen Burner GameObject.
/// Creates and manages the flame particle system.
/// Interaction: Click on burner OR press B key to toggle flame.
/// </summary>
public class BunsenBurnerFlame : MonoBehaviour
{
    [Header("Flame Settings")]
    [Tooltip("Offset from burner center — adjust Y to place flame at nozzle top")]
    public Vector3 flameOffset = new Vector3(0f, 0.3f, 0f);
    
    [Tooltip("Is flame currently lit?")]
    public bool isLit = false;
    
    [Header("Auto-Created (don't assign)")]
    public ParticleSystem flameParticles;
    public Light flameLight;
    
    [HideInInspector]
    public Color originalFlameColor = new Color(0.2f, 0.4f, 1f, 1f);
    
    private float lastToggleTime = 0f;
    private float toggleCooldown = 0.5f;
    
    void Start()
    {
        CreateFlameSystem();
        
        if (GetComponent<Collider>() == null)
        {
            BoxCollider box = gameObject.AddComponent<BoxCollider>();
            box.size = new Vector3(0.25f, 0.5f, 0.25f);
            box.center = new Vector3(0f, 0.25f, 0f);
        }
        
        // Setup VR interaction so pointing + trigger works
        SetupXRInteraction();
        
        if (!isLit)
        {
            flameParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (flameLight != null) flameLight.enabled = false;
        }
        
        Debug.Log("✅ Bunsen burner ready — Point + Trigger, Click, or press B");
    }
    
    void SetupXRInteraction()
    {
        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
            interactable = gameObject.AddComponent<XRSimpleInteractable>();
        
        // When the VR controller selects (trigger press), toggle flame
        interactable.selectEntered.AddListener((SelectEnterEventArgs args) =>
        {
            if (Time.time - lastToggleTime > toggleCooldown)
            {
                lastToggleTime = Time.time;
                ToggleFlame();
            }
        });
        
        Debug.Log("✅ XR interaction setup for Bunsen burner");
    }
    
    void Update()
    {
        // B key — always works
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleFlame();
            return;
        }
        
        // Mouse click on burner — works with XR Simulator
        if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main == null) return;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 20f))
            {
                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                {
                    if (Time.time - lastToggleTime > toggleCooldown)
                    {
                        lastToggleTime = Time.time;
                        ToggleFlame();
                    }
                }
            }
        }
    }
    
    void CreateFlameSystem()
    {
        GameObject flameObj = new GameObject("BurnerFlame");
        flameObj.transform.SetParent(transform);
        flameObj.transform.localPosition = flameOffset;
        flameObj.transform.localRotation = Quaternion.identity;
        
        flameParticles = flameObj.AddComponent<ParticleSystem>();
        flameParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
        var main = flameParticles.main;
        main.duration = 1f;
        main.loop = true;
        main.startLifetime = 0.4f;
        main.startSpeed = 0.3f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.05f);
        main.maxParticles = 80;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.1f;
        main.playOnAwake = false;
        
        originalFlameColor = new Color(0.2f, 0.45f, 1f, 0.9f);
        main.startColor = originalFlameColor;
        
        var emission = flameParticles.emission;
        emission.enabled = true;
        emission.rateOverTime = 60f;
        
        var shape = flameParticles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 8f;
        shape.radius = 0.008f;
        shape.radiusThickness = 1f;
        shape.rotation = new Vector3(-90f, 0f, 0f);
        
        var sizeOverLifetime = flameParticles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 0.5f);
        sizeCurve.AddKey(0.3f, 1f);
        sizeCurve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
        
        var colorOverLifetime = flameParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(CreateBlueFlameGradient());
        
        var noise = flameParticles.noise;
        noise.enabled = true;
        noise.strength = 0.03f;
        noise.frequency = 3f;
        noise.scrollSpeed = 0.5f;
        noise.damping = true;
        noise.octaveCount = 2;
        
        var renderer = flameParticles.GetComponent<ParticleSystemRenderer>();
        renderer.material = CreateFlameMaterial();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortMode = ParticleSystemSortMode.Distance;
        renderer.minParticleSize = 0f;
        renderer.maxParticleSize = 0.1f;
        
        GameObject lightObj = new GameObject("FlameLight");
        lightObj.transform.SetParent(flameObj.transform);
        lightObj.transform.localPosition = new Vector3(0f, 0.05f, 0f);
        
        flameLight = lightObj.AddComponent<Light>();
        flameLight.type = LightType.Point;
        flameLight.color = new Color(0.3f, 0.5f, 1f, 1f);
        flameLight.intensity = 1.5f;
        flameLight.range = 1f;
        flameLight.shadows = LightShadows.None;
    }
    
    Gradient CreateBlueFlameGradient()
    {
        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(new Color(0.2f, 0.4f, 1f), 0f),
                new GradientColorKey(new Color(0.3f, 0.6f, 1f), 0.3f),
                new GradientColorKey(new Color(1f, 0.8f, 0.3f), 0.6f),
                new GradientColorKey(new Color(1f, 0.6f, 0.1f), 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.8f, 0f),
                new GradientAlphaKey(1f, 0.3f),
                new GradientAlphaKey(0.6f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        return g;
    }
    
    Material CreateFlameMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Particles/Standard Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        
        Material flameMat = new Material(shader);
        flameMat.color = Color.white;
        flameMat.renderQueue = 3000;
        
        if (flameMat.HasProperty("_Surface"))
            flameMat.SetFloat("_Surface", 1);
        if (flameMat.HasProperty("_Blend"))
            flameMat.SetFloat("_Blend", 1);
        
        return flameMat;
    }
    
    // === PUBLIC METHODS ===
    
    public void ToggleFlame()
    {
        if (isLit) TurnOff();
        else TurnOn();
    }
    
    public void TurnOn()
    {
        isLit = true;
        if (flameParticles != null) flameParticles.Play();
        if (flameLight != null) flameLight.enabled = true;
        Debug.Log("🔥 Bunsen burner ON");
    }
    
    public void TurnOff()
    {
        isLit = false;
        if (flameParticles != null) flameParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (flameLight != null) flameLight.enabled = false;
        Debug.Log("🔥 Bunsen burner OFF");
    }
    
    public void SetChemicalFlameColor(Color chemColor)
    {
        if (flameParticles == null) return;
        
        // Clear existing particles so old-color particles disappear immediately
        flameParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
        var main = flameParticles.main;
        main.startColor = chemColor;
        
        var colorOverLifetime = flameParticles.colorOverLifetime;
        Gradient chemGradient = new Gradient();
        chemGradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(chemColor, 0f),
                new GradientColorKey(chemColor, 0.3f),
                new GradientColorKey(Color.Lerp(chemColor, Color.white, 0.3f), 0.6f),
                new GradientColorKey(chemColor * 0.7f, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.9f, 0f),
                new GradientAlphaKey(1f, 0.3f),
                new GradientAlphaKey(0.6f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(chemGradient);
        
        if (flameLight != null)
        {
            flameLight.color = chemColor;
            flameLight.intensity = 2.5f;
        }
        
        var emission = flameParticles.emission;
        emission.rateOverTime = 100f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.04f, 0.08f);
        
        // Restart the particle system with the new color
        flameParticles.Play();
        
        Debug.Log($"🔥 Flame color set to: {chemColor}");
    }
    
    public void SetFlameColor(Color newColor)
    {
        if (flameParticles == null) return;
        
        var main = flameParticles.main;
        main.startColor = newColor;
        
        var colorOverLifetime = flameParticles.colorOverLifetime;
        Gradient lerpGradient = new Gradient();
        lerpGradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(newColor, 0f),
                new GradientColorKey(Color.Lerp(newColor, new Color(0.3f, 0.6f, 1f), 0.3f), 0.3f),
                new GradientColorKey(Color.Lerp(newColor, Color.white, 0.2f), 0.6f),
                new GradientColorKey(newColor * 0.5f, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(0.8f, 0f),
                new GradientAlphaKey(1f, 0.3f),
                new GradientAlphaKey(0.6f, 0.7f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(lerpGradient);
        
        if (flameLight != null) flameLight.color = newColor;
        
        // Clear old particles so color change is visible immediately
        flameParticles.Clear();
    }
    
    public void ResetFlameColor()
    {
        if (flameParticles == null) return;
        
        // Clear old particles so reset is visible immediately
        flameParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
        var main = flameParticles.main;
        main.startColor = originalFlameColor;
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.05f);
        
        var colorOverLifetime = flameParticles.colorOverLifetime;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(CreateBlueFlameGradient());
        
        var emission = flameParticles.emission;
        emission.rateOverTime = 60f;
        
        if (flameLight != null)
        {
            flameLight.color = new Color(0.3f, 0.5f, 1f, 1f);
            flameLight.intensity = 1.5f;
        }
        
        // Restart with the original blue color
        if (isLit) flameParticles.Play();
    }
}
