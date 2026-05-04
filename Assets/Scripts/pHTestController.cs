using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class pHTestController : MonoBehaviour
{
    [System.Serializable]
    public class ChemicalSolution
    {
        public string chemicalName;       // e.g. "Hydrochloric Acid"
        public string formula;            // e.g. "HCl"
        public float pHValue;             // e.g. 1.0
        public GameObject beaker;         // Reference to beaker in scene
        public Color solutionColor;       // Color of the liquid in the beaker
        [HideInInspector]
        public bool tested;               // Has this been tested?
    }
    
    [Header("Chemical Solutions")]
    public ChemicalSolution[] chemicals;
    
    [Header("pH Paper Strips")]
    public GameObject[] phPaperStrips;    // The 5 PhPaper objects in the scene
    
    [Header("Pipette")]
    public GameObject pipette;
    
    [Header("pH Color Reference")]
    [Tooltip("Material colors for different pH ranges")]
    public Color acidColor = new Color(1f, 0.2f, 0.2f, 1f);      // Red (pH 0-3)
    public Color weakAcidColor = new Color(1f, 0.6f, 0.2f, 1f);   // Orange (pH 3-5)
    public Color neutralColor = new Color(0.2f, 0.8f, 0.2f, 1f);  // Green (pH 6-8)
    public Color weakBaseColor = new Color(0.2f, 0.5f, 0.9f, 1f); // Blue (pH 8-11)
    public Color baseColor = new Color(0.5f, 0.1f, 0.8f, 1f);     // Purple (pH 11-14)
    
    [Header("UI")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreText;
    
    [Header("Scoring")]
    public int pointsPerTest = 10;
    
    // Internal state
    private int totalScore = 0;
    private int testsCompleted = 0;
    private int nextPaperIndex = 0;
    public bool allTestsCompleted = false;
    
    // Pipette state
    private bool pipetteLoaded = false;
    private int loadedChemicalIndex = -1;
    private XRGrabInteractable pipetteGrab;
    private readonly Dictionary<Collider, int> beakerColliderToIndex = new Dictionary<Collider, int>();
    private readonly Dictionary<Collider, int> paperColliderToIndex = new Dictionary<Collider, int>();
    
    void Start()
    {
        // Fill in default chemical data for any entries missing names
        // (user may have assigned beakers but not typed names/formulas)
        EnsureChemicalData();
        EnsureUIReferences();
        
        // Setup pipette as grabbable
        SetupPipette();
        
        // Setup beakers as click-to-load (XRSimpleInteractable)
        SetupBeakers();
        
        // Setup pH paper strips as click-to-test (XRSimpleInteractable)
        SetupPhPaperStrips();
        
        // Add labels to beakers
        AddBeakerLabels();
        
        // Setup collision ignoring between pipette and everything
        SetupCollisionIgnoring();
        
        UpdateInstruction("═══ pH TEST EXPERIMENT ═══\n\n" +
                         "► Step 1: Point at a beaker\n" +
                         "   and press TRIGGER to load pipette.\n\n" +
                         $"{chemicals.Length} chemicals to test.");
        UpdateScore();
        
        Debug.Log($"✅ pHTestController ready. Pipette: {pipette != null}, " +
                  $"Beakers: {CountAssignedBeakers()}/{chemicals.Length}, " +
                  $"Papers: {phPaperStrips.Length}");
    }

    void Update()
    {
        // Fallback input path so simulator/mouse trigger can still run pH flow.
        if (allTestsCompleted)
            return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
        {
            TryHandleFallbackClick();
        }
    }
    
    int CountAssignedBeakers()
    {
        int count = 0;
        foreach (var c in chemicals)
            if (c.beaker != null) count++;
        return count;
    }
    
    void EnsureChemicalData()
    {
        // Default chemical data
        string[] defaultNames = { "Hydrochloric Acid", "Sodium Hydroxide", "Distilled Water", "Vinegar", "Ammonia Solution" };
        string[] defaultFormulas = { "HCl", "NaOH", "H₂O", "CH₃COOH", "NH₃" };
        float[] defaultPH = { 1.0f, 13.0f, 7.0f, 2.5f, 11.5f };
        
        // If no chemicals array at all, create it
        if (chemicals == null || chemicals.Length == 0)
        {
            chemicals = new ChemicalSolution[5];
            for (int i = 0; i < 5; i++)
            {
                chemicals[i] = new ChemicalSolution
                {
                    chemicalName = defaultNames[i],
                    formula = defaultFormulas[i],
                    pHValue = defaultPH[i]
                };
            }
            Debug.Log("✅ Created 5 default chemicals (no beakers assigned yet)");
            return;
        }
        
        // Fill in missing data for existing entries (user assigned beakers but left names empty)
        for (int i = 0; i < chemicals.Length; i++)
        {
            if (string.IsNullOrEmpty(chemicals[i].chemicalName) && i < defaultNames.Length)
            {
                chemicals[i].chemicalName = defaultNames[i];
                chemicals[i].formula = defaultFormulas[i];
                chemicals[i].pHValue = defaultPH[i];
                Debug.Log($"✅ Auto-filled chemical {i}: {defaultNames[i]} (pH {defaultPH[i]})");
            }
        }
    }
    
    void SetupPipette()
    {
        if (pipette == null) return;
        
        pipetteGrab = pipette.GetComponent<XRGrabInteractable>();
        if (pipetteGrab == null)
        {
            pipetteGrab = pipette.AddComponent<XRGrabInteractable>();
        }
        
        pipetteGrab.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        pipetteGrab.throwOnDetach = false;
        
        // Add Rigidbody
        Rigidbody rb = pipette.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = pipette.AddComponent<Rigidbody>();
        }
        rb.mass = 0.1f;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        // Add collider if missing
        Collider col = pipette.GetComponent<Collider>();
        if (col == null)
        {
            col = pipette.GetComponentInChildren<Collider>();
            if (col == null)
            {
                CapsuleCollider cap = pipette.AddComponent<CapsuleCollider>();
                cap.radius = 0.01f;
                cap.height = 0.2f;
                cap.direction = 1;
                Debug.Log("✅ Added CapsuleCollider to pipette");
            }
        }
    }
    
    void SetupBeakers()
    {
        beakerColliderToIndex.Clear();
        for (int i = 0; i < chemicals.Length; i++)
        {
            if (chemicals[i].beaker == null) continue;
            
            int index = i; // Capture for lambda
            GameObject beaker = chemicals[i].beaker;
            
            // Remove Rigidbody so beakers stay in place
            Rigidbody rb = beaker.GetComponent<Rigidbody>();
            if (rb != null) Destroy(rb);
            
            // Add XRSimpleInteractable for point-and-click
            XRSimpleInteractable simple = beaker.GetComponent<XRSimpleInteractable>();
            if (simple == null) simple = beaker.AddComponent<XRSimpleInteractable>();
            simple.interactionLayers = InteractionLayerMask.GetMask("Default");
            
            // Force all colliders to be SOLID (not triggers) so XR rays can hit them
            Collider[] allCols = beaker.GetComponentsInChildren<Collider>();
            if (allCols.Length == 0)
            {
                BoxCollider box = beaker.AddComponent<BoxCollider>();
                box.size = new Vector3(0.08f, 0.12f, 0.08f);
                box.isTrigger = false;
                box.gameObject.layer = LayerMask.NameToLayer("Default");
                beakerColliderToIndex[box] = index;
                allCols = new Collider[] { box };
            }
            else
            {
                foreach (Collider c in allCols)
                {
                    c.isTrigger = false;
                    if (c.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
                        c.gameObject.layer = LayerMask.NameToLayer("Default");
                    if (!beakerColliderToIndex.ContainsKey(c))
                        beakerColliderToIndex.Add(c, index);
                }
            }
            simple.colliders.Clear();
            foreach (var col in allCols.Where(c => c != null))
                simple.colliders.Add(col);
            
            // Click beaker → load pipette
            simple.selectEntered.AddListener((args) =>
            {
                OnBeakerClicked(index);
            });
            
            Debug.Log($"✅ Beaker {i} setup: {chemicals[i].chemicalName} ({chemicals[i].formula}) pH={chemicals[i].pHValue} colliders={allCols.Length}");
        }
    }
    
    void SetupPhPaperStrips()
    {
        paperColliderToIndex.Clear();
        for (int i = 0; i < phPaperStrips.Length; i++)
        {
            if (phPaperStrips[i] == null) continue;
            
            int index = i;
            GameObject paper = phPaperStrips[i];
            
            // Remove Rigidbody so papers stay in place
            Rigidbody rb = paper.GetComponent<Rigidbody>();
            if (rb != null) Destroy(rb);
            
            // Add XRSimpleInteractable for point-and-click
            XRSimpleInteractable simple = paper.GetComponent<XRSimpleInteractable>();
            if (simple == null) simple = paper.AddComponent<XRSimpleInteractable>();
            simple.interactionLayers = InteractionLayerMask.GetMask("Default");
            
            // Force all colliders to be SOLID (not triggers) so XR rays can hit them
            Collider[] allCols = paper.GetComponentsInChildren<Collider>();
            if (allCols.Length == 0)
            {
                BoxCollider box = paper.AddComponent<BoxCollider>();
                box.size = new Vector3(0.1f, 0.01f, 0.03f);
                box.isTrigger = false;
                box.gameObject.layer = LayerMask.NameToLayer("Default");
                paperColliderToIndex[box] = index;
                allCols = new Collider[] { box };
            }
            else
            {
                foreach (Collider c in allCols)
                {
                    c.isTrigger = false;
                    if (c.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
                        c.gameObject.layer = LayerMask.NameToLayer("Default");
                    if (!paperColliderToIndex.ContainsKey(c))
                        paperColliderToIndex.Add(c, index);
                }
            }
            simple.colliders.Clear();
            foreach (var col in allCols.Where(c => c != null))
                simple.colliders.Add(col);
            
            // Click paper → test with loaded chemical
            simple.selectEntered.AddListener((args) =>
            {
                OnPaperClicked(index);
            });
            
            // Set initial color to YELLOW (untested pH paper)
            SetPaperColor(phPaperStrips[i], new Color(1f, 0.95f, 0.2f, 1f));
            
            Debug.Log($"✅ pH Paper {i} setup with colliders={allCols.Length}");
        }
    }
    
    void AddBeakerLabels()
    {
        for (int i = 0; i < chemicals.Length; i++)
        {
            if (chemicals[i].beaker == null) continue;
            
            string labelText = $"{chemicals[i].chemicalName} ({chemicals[i].formula})";
            Color labelColor = GetColorForPH(chemicals[i].pHValue);
            
            AddLabel(chemicals[i].beaker, labelText, labelColor);
        }
    }
    
    void AddLabel(GameObject target, string labelText, Color labelColor)
    {
        if (target == null) return;

        Transform existing = target.transform.Find($"Label_{target.name}");
        if (existing != null)
            Destroy(existing.gameObject);

        GameObject labelObj = new GameObject($"Label_{target.name}");
        labelObj.transform.SetParent(target.transform, false);
        labelObj.transform.localPosition = new Vector3(0f, 0.18f, 0f);
        labelObj.transform.localScale = Vector3.one * 0.035f;
        labelObj.layer = LayerMask.NameToLayer("Ignore Raycast");

        TextMeshPro tmp = labelObj.AddComponent<TextMeshPro>();
        tmp.text = labelText;
        tmp.fontSize = 4f;
        tmp.color = labelColor;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.enableWordWrapping = false;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.outlineWidth = 0.2f;
        tmp.outlineColor = Color.black;

        // Billboard — always face camera
        labelObj.AddComponent<LookAtCamera>();
    }
    
    void SetupCollisionIgnoring()
    {
        if (pipette == null) return;
        
        Collider[] pipetteCols = pipette.GetComponentsInChildren<Collider>();
        if (pipetteCols.Length == 0) return;
        
        // Pipette ignores all beakers
        for (int i = 0; i < chemicals.Length; i++)
        {
            if (chemicals[i].beaker == null) continue;
            Collider[] beakerCols = chemicals[i].beaker.GetComponentsInChildren<Collider>();
            foreach (Collider pc in pipetteCols)
            {
                foreach (Collider bc in beakerCols)
                {
                    Physics.IgnoreCollision(pc, bc);
                }
            }
        }
        
        // Pipette ignores all pH papers
        for (int i = 0; i < phPaperStrips.Length; i++)
        {
            if (phPaperStrips[i] == null) continue;
            Collider[] paperCols = phPaperStrips[i].GetComponentsInChildren<Collider>();
            foreach (Collider pc in pipetteCols)
            {
                foreach (Collider pac in paperCols)
                {
                    Physics.IgnoreCollision(pc, pac);
                }
            }
        }
        
        Debug.Log("✅ Pipette ignores all beakers and pH papers");
    }
    
    // === INTERACTION HANDLERS ===
    
    // Called when player clicks a beaker
    void OnBeakerClicked(int chemicalIndex)
    {
        if (allTestsCompleted) return;
        if (chemicalIndex < 0 || chemicalIndex >= chemicals.Length) return;
        
        ChemicalSolution chem = chemicals[chemicalIndex];
        
        // Skip if already tested
        if (chem.tested)
        {
            UpdateInstruction($"✅ {chem.chemicalName} already tested!\nChoose a different beaker.");
            Debug.Log($"⚠️ {chem.chemicalName} already tested — skipping");
            return;
        }
        
        // Allow swapping chemicals
        if (pipetteLoaded && loadedChemicalIndex != chemicalIndex)
        {
            Debug.Log($"🔄 Swapping chemical: {chemicals[loadedChemicalIndex].chemicalName} → {chem.chemicalName}");
        }
        
        pipetteLoaded = true;
        loadedChemicalIndex = chemicalIndex;
        
        UpdateInstruction($"✅ Pipette loaded: {chem.chemicalName}\n({chem.formula})\n\n" +
                         "► Now point at a pH paper strip\n" +
                         "   and press TRIGGER to test it.");
        UpdateResult("");
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlayGrab();
        
        Debug.Log($"💧 Pipette loaded with: {chem.chemicalName} (pH {chem.pHValue}) [{testsCompleted}/{chemicals.Length} done]");
    }
    
    // Called when player clicks a pH paper strip
    void OnPaperClicked(int paperIndex)
    {
        if (allTestsCompleted) return;
        
        if (!pipetteLoaded || loadedChemicalIndex < 0)
        {
            UpdateInstruction("⚠️ Pipette is empty!\n\n► Point at a beaker first\n   and press TRIGGER to load.");
            return;
        }
        
        if (paperIndex >= phPaperStrips.Length || phPaperStrips[paperIndex] == null)
            return;
        
        ChemicalSolution chem = chemicals[loadedChemicalIndex];
        
        // Check if this chemical was already tested
        if (chem.tested)
        {
            UpdateInstruction($"✅ {chem.chemicalName} was already tested!\nLoad a different chemical.");
            return;
        }
        
        // Change pH paper color based on pH value
        Color paperColor = GetColorForPH(chem.pHValue);
        SetPaperColor(phPaperStrips[paperIndex], paperColor);
        
        // Show result
        string acidOrBase = GetAcidBaseLabel(chem.pHValue);
        string resultMessage = $"📋 {chem.chemicalName} ({chem.formula})\n" +
                              $"pH: {chem.pHValue:F1} — {acidOrBase}\n" +
                              $"Paper turned: {GetColorNameForPH(chem.pHValue)}";
        UpdateResult(resultMessage);
        UpdateInstruction($"✅ Test complete for {chem.chemicalName}!");
        
        // Mark as tested and score
        chem.tested = true;
        totalScore += pointsPerTest;
        testsCompleted++;
        
        // Reset pipette
        pipetteLoaded = false;
        loadedChemicalIndex = -1;
        
        UpdateScore();
        
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySuccess();
        
        Debug.Log($"✅ pH Test: {chem.chemicalName} = pH {chem.pHValue} ({acidOrBase}) [{testsCompleted}/{chemicals.Length}]");
        
        // Check if all done
        if (testsCompleted >= chemicals.Length)
        {
            StartCoroutine(CompleteAllTests());
        }
        else
        {
            Invoke("ShowNextInstruction", 3f);
        }
    }
    
    void ShowNextInstruction()
    {
        UpdateInstruction($"Good! {testsCompleted}/{chemicals.Length} chemicals tested.\n\n" +
                         "► Point at another beaker\n" +
                         "   and press TRIGGER to continue.");
    }
    
    IEnumerator CompleteAllTests()
    {
        yield return new WaitForSeconds(2f);
        
        allTestsCompleted = true;
        
        UpdateInstruction("🎉 All pH tests complete!");
        UpdateResult($"Final Score: {totalScore}/{chemicals.Length * pointsPerTest}\n" +
                    "Great job identifying all solutions!");
        
        Debug.Log("🎉 === pH TEST SCENARIO COMPLETE ===");
    }
    
    // === COLOR MAPPING ===
    
    Color GetColorForPH(float pH)
    {
        if (pH < 3f)
            return acidColor;           // Strong acid — Red
        else if (pH < 5f)
            return weakAcidColor;       // Weak acid — Orange
        else if (pH < 8f)
            return neutralColor;        // Neutral — Green
        else if (pH < 11f)
            return weakBaseColor;       // Weak base — Blue
        else
            return baseColor;           // Strong base — Purple
    }
    
    string GetColorNameForPH(float pH)
    {
        if (pH < 3f) return "Red (Strong Acid)";
        else if (pH < 5f) return "Orange (Weak Acid)";
        else if (pH < 8f) return "Green (Neutral)";
        else if (pH < 11f) return "Blue (Weak Base)";
        else return "Purple (Strong Base)";
    }
    
    string GetAcidBaseLabel(float pH)
    {
        if (pH < 3f) return "Strong Acid";
        else if (pH < 6f) return "Weak Acid";
        else if (pH < 8f) return "Neutral";
        else if (pH < 11f) return "Weak Base";
        else return "Strong Base";
    }
    
    void SetPaperColor(GameObject paper, Color color)
    {
        Renderer[] renderers = paper.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            // Create a new material instance so we don't affect other papers
            Material mat = new Material(r.material);
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
            mat.color = color;
            r.material = mat;
        }
    }

    void EnsureUIReferences()
    {
        if (instructionText != null && resultText != null && scoreText != null)
            return;

        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (var t in allTexts)
        {
            string n = t.gameObject.name.ToLowerInvariant();
            if (instructionText == null && n.Contains("instruction"))
                instructionText = t;
            else if (resultText == null && (n.Contains("result") || n.Contains("output")))
                resultText = t;
            else if (scoreText == null && (n.Contains("score") || n.Contains("progress")))
                scoreText = t;
        }

        if (instructionText == null || resultText == null || scoreText == null)
        {
            Debug.LogWarning("⚠️ pH UI refs not fully assigned. Assign instruction/result/score texts in Inspector for best reliability.");
        }
    }

    void TryHandleFallbackClick()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, 20f))
            return;

        if (beakerColliderToIndex.TryGetValue(hit.collider, out int beakerIndex))
        {
            OnBeakerClicked(beakerIndex);
            return;
        }

        if (paperColliderToIndex.TryGetValue(hit.collider, out int paperIndex))
        {
            OnPaperClicked(paperIndex);
        }
    }
    
    // === UI HELPERS ===
    
    void UpdateInstruction(string text)
    {
        if (instructionText != null)
            instructionText.text = text;
        Debug.Log("pH Instruction: " + text);
    }
    
    void UpdateResult(string text)
    {
        if (resultText != null)
            resultText.text = text;
    }
    
    void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {totalScore} | Tests: {testsCompleted}/{chemicals.Length}";
    }
}
