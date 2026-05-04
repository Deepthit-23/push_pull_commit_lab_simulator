using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using System.Collections;

public class FlameTestController : MonoBehaviour
{
    [Header("Bunsen Burner")]
    [Tooltip("The Bunsen Burner GameObject (BunsenBurnerFlame auto-added)")]
    public GameObject bunsenBurner;
    
    [Tooltip("How close wire loop tip must be to flame")]
    public float detectionRadius = 0.35f;
    
    [Header("Wire Loop")]
    public GameObject wireLoop;
    public Transform wireLoopTip;
    
    [Header("Samples — Drag petri dishes here")]
    public GameObject sampleLithium;
    public GameObject sampleSodium;
    public GameObject samplePotassium;
    public GameObject sampleCopper;
    public GameObject sampleCalcium;
    
    [Header("UI — Drag TMP Text objects here")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreText;
    
    [Header("Scoring")]
    public int pointsPerCorrectIdentification = 10;
    
    // State
    private string currentSampleLoaded = "";
    private bool isSampleOnLoop = false;
    private bool isInFlame = false;
    private float timeInFlame = 0f;
    private float requiredFlameTime = 2f;
    private bool flameColorRevealed = false;
    
    private int totalScore = 0;
    private int samplesCompleted = 0;
    public bool allSamplesCompleted = false;
    
    private bool lithiumTested, sodiumTested, potassiumTested, copperTested, calciumTested;
    
    private BunsenBurnerFlame burnerFlame;
    private XRGrabInteractable wireLoopGrab;
    private int currentStep = 0;
    
    // Flame color data
    public struct FlameTestResult
    {
        public string chemicalName, elementSymbol, colorName;
        public Color flameColor;
        
        public FlameTestResult(string name, string symbol, Color color, string colorName)
        {
            chemicalName = name;
            elementSymbol = symbol;
            flameColor = color;
            this.colorName = colorName;
        }
    }
    
    private FlameTestResult[] flameTestData;
    
    void Start()
    {
        flameTestData = new FlameTestResult[]
        {
            new FlameTestResult("Lithium", "Li", new Color(0.95f, 0.1f, 0.15f, 1f), "Crimson Red"),
            new FlameTestResult("Sodium", "Na", new Color(1f, 0.9f, 0f, 1f), "Bright Yellow"),
            new FlameTestResult("Potassium", "K", new Color(0.7f, 0.4f, 0.95f, 1f), "Lilac/Violet"),
            new FlameTestResult("Copper", "Cu", new Color(0.05f, 0.9f, 0.45f, 1f), "Blue-Green"),
            new FlameTestResult("Calcium", "Ca", new Color(1f, 0.45f, 0.1f, 1f), "Orange-Red")
        };
        
        // === BUNSEN BURNER ===
        if (bunsenBurner != null)
        {
            // Remove Rigidbody so it stays in place
            Rigidbody burnerRb = bunsenBurner.GetComponent<Rigidbody>();
            if (burnerRb != null) Destroy(burnerRb);
            
            burnerFlame = bunsenBurner.GetComponent<BunsenBurnerFlame>();
            if (burnerFlame == null)
                burnerFlame = bunsenBurner.AddComponent<BunsenBurnerFlame>();
        }
        
        // === FREEZE SAMPLES (no physics) ===
        FreezeSample(sampleLithium);
        FreezeSample(sampleSodium);
        FreezeSample(samplePotassium);
        FreezeSample(sampleCopper);
        FreezeSample(sampleCalcium);
        
        // === ADD LABELS TO SAMPLES ===
        AddLabel(sampleLithium, "Lithium (Li)", new Color(1f, 0.4f, 0.4f));
        AddLabel(sampleSodium, "Sodium (Na)", new Color(1f, 1f, 0.4f));
        AddLabel(samplePotassium, "Potassium (K)", new Color(0.8f, 0.6f, 1f));
        AddLabel(sampleCopper, "Copper (Cu)", new Color(0.4f, 1f, 0.7f));
        AddLabel(sampleCalcium, "Calcium (Ca)", new Color(1f, 0.7f, 0.4f));
        
        // === SETUP WIRE LOOP (must be before samples & ignore calls) ===
        SetupWireLoop();
        
        // === SETUP SAMPLES AS VR INTERACTABLE ===
        SetupSample(sampleLithium, "Lithium");
        SetupSample(sampleSodium, "Sodium");
        SetupSample(samplePotassium, "Potassium");
        SetupSample(sampleCopper, "Copper");
        SetupSample(sampleCalcium, "Calcium");
        
        // === SETUP COLLISION IGNORING (delayed so BunsenBurnerFlame.Start adds its colliders first) ===
        StartCoroutine(SetupCollisionIgnoringDelayed());
        
        // === INITIAL INSTRUCTION ===
        currentStep = 1;
        ShowStep();
        UpdateScore();
        
        Debug.Log($"✅ FlameTestController ready. Wire loop: {wireLoop != null}, Burner: {bunsenBurner != null}, Samples assigned: Li={sampleLithium != null} Na={sampleSodium != null} K={samplePotassium != null} Cu={sampleCopper != null} Ca={sampleCalcium != null}");
    }
    
    /// <summary>
    /// Wait one frame so BunsenBurnerFlame.Start() has time to add its colliders,
    /// then set up collision ignoring.
    /// </summary>
    IEnumerator SetupCollisionIgnoringDelayed()
    {
        // Wait 2 frames so every Start() method has finished adding colliders
        yield return null;
        yield return null;
        
        SetupCollisionIgnoring();
    }
    
    /// <summary>
    /// Makes the wire loop pass through burner and all samples.
    /// </summary>
    void SetupCollisionIgnoring()
    {
        if (wireLoop == null) return;
        
        Collider[] wireCols = wireLoop.GetComponentsInChildren<Collider>();
        if (wireCols.Length == 0)
        {
            Debug.LogWarning("⚠️ Wire loop has no colliders — can't set up collision ignoring!");
            return;
        }
        
        // Ignore wire ↔ bunsen burner (including all children like the flame)
        if (bunsenBurner != null)
        {
            Collider[] burnerCols = bunsenBurner.GetComponentsInChildren<Collider>();
            foreach (Collider wc in wireCols)
            {
                foreach (Collider bc in burnerCols)
                {
                    Physics.IgnoreCollision(wc, bc, true);
                }
            }
            Debug.Log($"✅ Wire loop ignores Bunsen burner ({burnerCols.Length} colliders)");
        }
        
        // Ignore wire ↔ all sample dishes
        GameObject[] allSamples = { sampleLithium, sampleSodium, samplePotassium, sampleCopper, sampleCalcium };
        foreach (GameObject sample in allSamples)
        {
            if (sample == null) continue;
            Collider[] sampleCols = sample.GetComponentsInChildren<Collider>();
            foreach (Collider wc in wireCols)
            {
                foreach (Collider sc in sampleCols)
                {
                    Physics.IgnoreCollision(wc, sc, true);
                }
            }
        }
        Debug.Log("✅ Wire loop ignores all sample dishes");
    }
    
    void FreezeSample(GameObject sample)
    {
        if (sample == null) return;
        Rigidbody rb = sample.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);
    }
    
    /// <summary>
    /// Adds a floating label above each sample dish
    /// </summary>
    void AddLabel(GameObject sample, string labelText, Color labelColor)
    {
        if (sample == null) return;
        
        // Remove any existing labels to avoid duplicates
        Transform existingLabel = sample.transform.Find($"Label_{sample.name}");
        if (existingLabel != null) Destroy(existingLabel.gameObject);
        
        // Create a world-space canvas above the sample
        GameObject labelObj = new GameObject($"Label_{sample.name}");
        labelObj.transform.SetParent(sample.transform);
        labelObj.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        labelObj.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
        
        // Set label layer to Ignore Raycast so it never blocks VR raycasts
        labelObj.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        Canvas canvas = labelObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        // Prevent this canvas from blocking any raycasts or interactions
        CanvasGroup canvasGroup = labelObj.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        
        RectTransform canvasRect = labelObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(120, 40);
        
        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(labelObj.transform, false);
        textObj.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = labelText;
        tmp.fontSize = 20;
        tmp.color = labelColor;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.enableWordWrapping = false;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.raycastTarget = false;  // Don't block VR raycasts
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Make label always face the camera
        LookAtCamera lookAt = labelObj.AddComponent<LookAtCamera>();
    }
    
    void SetupWireLoop()
    {
        if (wireLoop == null) return;
        
        // Ensure wire loop has a collider (needed for grab + collision ignoring)
        Collider wireCol = wireLoop.GetComponent<Collider>();
        if (wireCol == null)
        {
            // Check children for colliders
            wireCol = wireLoop.GetComponentInChildren<Collider>();
            if (wireCol == null)
            {
                // No collider anywhere — add a capsule collider for the wire shape
                CapsuleCollider cap = wireLoop.AddComponent<CapsuleCollider>();
                cap.radius = 0.015f;
                cap.height = 0.3f;
                cap.direction = 1; // Y-axis
                cap.center = new Vector3(0f, 0.15f, 0f);
                Debug.Log("✅ Added CapsuleCollider to wire loop");
            }
        }
        
        wireLoopGrab = wireLoop.GetComponent<XRGrabInteractable>();
        if (wireLoopGrab == null) wireLoopGrab = wireLoop.AddComponent<XRGrabInteractable>();
        
        // Use Kinematic so the wire passes through the burner without physics blocking
        wireLoopGrab.movementType = XRBaseInteractable.MovementType.Kinematic;
        wireLoopGrab.throwOnDetach = false;
        
        Rigidbody rb = wireLoop.GetComponent<Rigidbody>();
        if (rb == null) rb = wireLoop.AddComponent<Rigidbody>();
        rb.mass = 0.1f;
        rb.useGravity = false;  // Kinematic doesn't need gravity
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        if (wireLoopTip == null)
        {
            GameObject tip = new GameObject("WireLoopTip");
            tip.transform.SetParent(wireLoop.transform);
            tip.transform.localPosition = new Vector3(0f, 0.29f, 0f);
            wireLoopTip = tip.transform;
            Debug.Log("✅ Created WireLoopTip at local (0, 0.29, 0)");
        }
    }
    
    void SetupSample(GameObject sample, string sampleName)
    {
        if (sample == null) return;
        
        // XRSimpleInteractable — point + trigger to select (no movement)
        XRSimpleInteractable simple = sample.GetComponent<XRSimpleInteractable>();
        if (simple == null) simple = sample.AddComponent<XRSimpleInteractable>();
        
        // Keep collider as SOLID (not trigger) — XR raycasts skip triggers!
        Collider col = sample.GetComponent<Collider>();
        if (col == null)
        {
            BoxCollider box = sample.AddComponent<BoxCollider>();
            box.size = new Vector3(0.12f, 0.06f, 0.12f);
            // NOT setting isTrigger — must stay solid for XR ray detection
        }
        
        simple.selectEntered.AddListener((args) =>
        {
            LoadSampleOntoWireLoop(sampleName, sample);
        });
        
        Debug.Log($"✅ Sample setup: {sampleName} (collider: {sample.GetComponent<Collider>()?.GetType().Name})");
    }
    
    void LoadSampleOntoWireLoop(string sampleName, GameObject sampleObject)
    {
        // Skip if already tested
        if (IsSampleAlreadyTested(sampleName))
        {
            UpdateInstruction($"✅ {sampleName} was already tested!\nChoose a different sample.");
            Debug.Log($"⚠️ {sampleName} already tested — skipping");
            return;
        }
        
        // If a sample is already loaded and currently being heated, block
        if (isSampleOnLoop && isInFlame)
        {
            UpdateInstruction("⚠️ Wire loop is in the flame!\nFinish this test first.");
            return;
        }
        
        // If a sample is loaded but NOT in flame, allow swapping
        if (isSampleOnLoop && currentSampleLoaded != sampleName)
        {
            Debug.Log($"🔄 Swapping sample: {currentSampleLoaded} → {sampleName}");
            // Reset flame color if it was partially changed
            if (burnerFlame != null) burnerFlame.ResetFlameColor();
        }
        
        // Check if burner is lit first
        if (burnerFlame != null && !burnerFlame.isLit)
        {
            UpdateInstruction($"⚠️ Light the Bunsen burner first!\n" +
                             "Point at the burner and press trigger.");
            return;
        }
        
        currentSampleLoaded = sampleName;
        isSampleOnLoop = true;
        flameColorRevealed = false;
        timeInFlame = 0f;
        isInFlame = false;
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayGrab();
        
        Debug.Log($"🧪 {sampleName} loaded onto wire loop ({samplesCompleted}/5 completed so far)");
        
        currentStep = 3;
        ShowStep();
    }
    
    // === STEP-BY-STEP INSTRUCTIONS ===
    void ShowStep()
    {
        switch (currentStep)
        {
            case 1:
                UpdateInstruction(
                    "═══ FLAME TEST EXPERIMENT ═══\n\n" +
                    "► Step 1: Point at the Bunsen burner\n" +
                    "   and press TRIGGER to light it.\n\n" +
                    "5 chemicals to identify by flame color.");
                UpdateResult("");
                break;
                
            case 2:
                UpdateInstruction(
                    "✅ Bunsen burner is lit!\n\n" +
                    "► Step 2: Point at a labeled sample\n" +
                    "   dish and press TRIGGER to load it\n" +
                    "   onto the wire loop.");
                break;
                
            case 3:
                UpdateInstruction(
                    $"✅ {currentSampleLoaded} loaded!\n\n" +
                    "► Step 3: GRAB the wire loop\n" +
                    "   and hold the tip inside the\n" +
                    "   Bunsen burner flame for 2 sec.");
                break;
                
            case 4:
                // Set dynamically during heating
                break;
                
            case 5:
                // Set by RevealFlameColor
                break;
                
            case 6:
                if (samplesCompleted >= 5)
                {
                    allSamplesCompleted = true;
                    UpdateInstruction(
                        "🎉 ALL FLAME TESTS COMPLETE! 🎉\n\n" +
                        $"Final Score: {totalScore}/50\n\n" +
                        "All chemicals identified!");
                }
                else
                {
                    UpdateInstruction(
                        $"✅ {samplesCompleted}/5 samples tested\n\n" +
                        "► Point at another sample dish\n" +
                        "   and press TRIGGER to continue.");
                    currentStep = 2;
                }
                break;
        }
    }
    
    void Update()
    {
        if (allSamplesCompleted) return;
        
        // Detect when burner gets lit → advance to step 2
        if (currentStep == 1 && burnerFlame != null && burnerFlame.isLit)
        {
            currentStep = 2;
            ShowStep();
        }
        
        // === FLAME DETECTION (only when wire loop is being HELD) ===
        bool isHoldingWire = wireLoopGrab != null && wireLoopGrab.isSelected;
        if (isSampleOnLoop && isHoldingWire && wireLoopTip != null && bunsenBurner != null)
        {
            if (burnerFlame != null && !burnerFlame.isLit) return;
            
            // Flame zone = the position of the flame particle system
            Vector3 flameZone;
            if (burnerFlame != null && burnerFlame.flameParticles != null)
            {
                // Use the actual flame particle position for accuracy
                flameZone = burnerFlame.flameParticles.transform.position;
            }
            else
            {
                flameZone = bunsenBurner.transform.position + 
                           Vector3.up * (burnerFlame != null ? burnerFlame.flameOffset.y : 0.3f);
            }
            
            float distance = Vector3.Distance(wireLoopTip.position, flameZone);
            
            // Draw debug line (green = in range, red = out of range)
            Debug.DrawLine(wireLoopTip.position, flameZone, 
                          distance <= detectionRadius ? Color.green : Color.red);
            
            // Log distance every 30 frames so you can check in console
            if (Time.frameCount % 30 == 0)
            {
                Debug.Log($"🔍 Wire tip distance to flame: {distance:F3}m (need ≤ {detectionRadius:F2}m) | Tip pos: {wireLoopTip.position} | Flame pos: {flameZone}");
            }
            
            if (distance <= detectionRadius)
            {
                if (!isInFlame)
                {
                    isInFlame = true;
                    currentStep = 4;
                    Debug.Log("🔥 Wire loop in flame zone!");
                }
                
                timeInFlame += Time.deltaTime;
                
                if (!flameColorRevealed && timeInFlame >= requiredFlameTime)
                {
                    RevealFlameColor();
                }
                else if (!flameColorRevealed)
                {
                    float pct = timeInFlame / requiredFlameTime * 100f;
                    UpdateInstruction($"🔥 Heating {currentSampleLoaded}... {pct:F0}%\n\nKeep holding in flame!");
                    LerpFlameColor(timeInFlame / requiredFlameTime);
                }
            }
            else
            {
                if (isInFlame)
                {
                    isInFlame = false;
                    if (!flameColorRevealed)
                    {
                        UpdateInstruction($"{currentSampleLoaded} loaded.\n\n⚠️ Move wire loop into the flame!");
                    }
                }
            }
        }
    }
    
    void LerpFlameColor(float t)
    {
        if (burnerFlame == null) return;
        Color target = GetFlameColorForSample(currentSampleLoaded);
        Color current = Color.Lerp(burnerFlame.originalFlameColor, target, t);
        burnerFlame.SetFlameColor(current);
    }
    
    void RevealFlameColor()
    {
        flameColorRevealed = true;
        FlameTestResult result = GetFlameTestResult(currentSampleLoaded);
        
        // Set full chemical color
        if (burnerFlame != null)
            burnerFlame.SetChemicalFlameColor(result.flameColor);
        
        UpdateResult($"🔥 {result.chemicalName} ({result.elementSymbol})\nFlame Color: {result.colorName}");
        
        currentStep = 5;
        UpdateInstruction(
            $"✅ IDENTIFIED: {result.chemicalName}\n\n" +
            $"   Element: {result.elementSymbol}\n" +
            $"   Flame: {result.colorName}\n\n" +
            "   Resetting in 4 seconds...");
        
        if (!IsSampleAlreadyTested(currentSampleLoaded))
        {
            totalScore += pointsPerCorrectIdentification;
            MarkSampleAsTested(currentSampleLoaded);
            samplesCompleted++;
            UpdateScore();
            
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySuccess();
        }
        
        Invoke("ResetForNextSample", 4f);
    }
    
    void ResetForNextSample()
    {
        if (burnerFlame != null) burnerFlame.ResetFlameColor();
        
        currentSampleLoaded = "";
        isSampleOnLoop = false;
        isInFlame = false;
        timeInFlame = 0f;
        flameColorRevealed = false;
        
        currentStep = 6;
        ShowStep();
    }
    
    // === HELPERS ===
    Color GetFlameColorForSample(string n)
    {
        foreach (var d in flameTestData)
            if (d.chemicalName == n) return d.flameColor;
        return burnerFlame != null ? burnerFlame.originalFlameColor : Color.blue;
    }
    
    FlameTestResult GetFlameTestResult(string n)
    {
        foreach (var d in flameTestData)
            if (d.chemicalName == n) return d;
        return flameTestData[0];
    }
    
    bool IsSampleAlreadyTested(string n)
    {
        switch (n)
        {
            case "Lithium": return lithiumTested;
            case "Sodium": return sodiumTested;
            case "Potassium": return potassiumTested;
            case "Copper": return copperTested;
            case "Calcium": return calciumTested;
        }
        return false;
    }
    
    void MarkSampleAsTested(string n)
    {
        switch (n)
        {
            case "Lithium": lithiumTested = true; break;
            case "Sodium": sodiumTested = true; break;
            case "Potassium": potassiumTested = true; break;
            case "Copper": copperTested = true; break;
            case "Calcium": calciumTested = true; break;
        }
    }
    
    void UpdateInstruction(string t)
    {
        if (instructionText != null) instructionText.text = t;
        Debug.Log("Instruction: " + t);
    }
    
    void UpdateResult(string t)
    {
        if (resultText != null) resultText.text = t;
    }
    
    void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {totalScore} | Samples: {samplesCompleted}/5";
    }
}

/// <summary>
/// Simple billboard — makes labels always face the camera
/// </summary>
public class LookAtCamera : MonoBehaviour
{
    private Camera mainCam;
    
    void Start()
    {
        mainCam = Camera.main;
    }
    
    void LateUpdate()
    {
        if (mainCam != null)
        {
            transform.LookAt(transform.position + mainCam.transform.rotation * Vector3.forward,
                            mainCam.transform.rotation * Vector3.up);
        }
    }
}
