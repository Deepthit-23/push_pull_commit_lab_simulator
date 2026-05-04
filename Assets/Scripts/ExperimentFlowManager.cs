using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ExperimentFlowManager : MonoBehaviour
{
    public enum ExperimentScenario
    {
        FlameTest,
        pHTest,
        Complete
    }
    
    [Header("Current State")]
    public ExperimentScenario currentScenario = ExperimentScenario.FlameTest;
    private bool scenarioCompleted = false;
    
    [Header("Scenario Areas")]
    public GameObject flameTestArea;
    public GameObject phTestArea;
    
    [Header("Controllers")]
    public FlameTestController flameTestController;
    public pHTestController phTestController;
    
    [Header("UI")]
    public TextMeshProUGUI mainInstructionText;
    public TextMeshProUGUI mainProgressText;
    
    [Header("Scoring")]
    private int totalScore = 0;
    private bool hasAddedFlameScore = false;
    private bool hasAddedpHScore = false;
    
    // Selection menu objects
    private GameObject selectionMenuObj;
    private GameObject phButtonObj;
    private TextMeshProUGUI phButtonText;
    private Image phButtonImage;
    
    void Start()
    {
        // Check if specific experiment was selected from menu
        int selectedExperiment = PlayerPrefs.GetInt("StartExperiment", 0);
        
        if (selectedExperiment == 1)
        {
            Debug.Log("Starting directly at Flame Test");
            PlayerPrefs.DeleteKey("StartExperiment");
            StartScenario(ExperimentScenario.FlameTest);
        }
        else if (selectedExperiment == 2)
        {
            Debug.Log("Starting directly at pH Test");
            PlayerPrefs.DeleteKey("StartExperiment");
            StartScenario(ExperimentScenario.pHTest);
        }
        else
        {
            // Show the selection menu
            ShowSelectionMenu();
        }
    }
    
    void Update()
    {
        if (!scenarioCompleted && selectionMenuObj == null)
        {
            CheckScenarioCompletion();
        }
    }
    
    // === SELECTION MENU ===
    
    void ShowSelectionMenu()
    {
        // Disable all experiment areas
        if (flameTestArea != null) flameTestArea.SetActive(false);
        if (phTestArea != null) phTestArea.SetActive(false);
        
        UpdateMainInstruction("═══ CHOOSE AN EXPERIMENT ═══\n\n" +
                             "Point at a button and\npress TRIGGER to start.");
        UpdateProgress("");
        
        // Position in front of the player / center of the scene
        Vector3 menuPos = transform.position + new Vector3(0f, 1.3f, 2f);
        if (flameTestArea != null)
        {
            menuPos = flameTestArea.transform.position + new Vector3(0f, 1.3f, 0.5f);
        }
        
        // Container (non-UI, just for grouping)
        selectionMenuObj = new GameObject("SelectionMenu");
        selectionMenuObj.transform.position = menuPos;
        
        // === FLAME TEST BUTTON ===
        CreateMenuButton(
            selectionMenuObj.transform,
            "FlameTestButton",
            "🔥 Flame Test",
            new Vector3(0f, 0.15f, 0f),
            new Color(0.9f, 0.3f, 0.1f, 1f),
            () => OnFlameTestSelected()
        );
        
        // === pH TEST BUTTON ===
        phButtonObj = CreateMenuButton(
            selectionMenuObj.transform,
            "pHTestButton",
            "🧪 pH Test",
            new Vector3(0f, -0.15f, 0f),
            new Color(0.1f, 0.6f, 0.9f, 1f),
            () => OnpHTestSelected()
        );

        // === EXIT BUTTON ===
        CreateMenuButton(
            selectionMenuObj.transform,
            "ExitButton",
            "🚪 Exit to Menu",
            new Vector3(0f, -0.45f, 0f),
            new Color(0.7f, 0.2f, 0.2f, 1f),
            () => ReturnToMenu()
        );
        
        Debug.Log("✅ Selection menu shown");
    }
    
    GameObject CreateMenuButton(Transform parent, string name, string text, Vector3 localPos, Color bgColor, System.Action onClick)
    {
        // Each button is its own world-space Canvas
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        btnObj.transform.localPosition = localPos;
        btnObj.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
        
        // World-space Canvas so UI elements render
        Canvas canvas = btnObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform canvasRect = btnObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(500, 90);
        
        // Collider for XR ray interaction (on the canvas object)
        BoxCollider col = btnObj.AddComponent<BoxCollider>();
        col.size = new Vector3(500f, 90f, 10f);
        
        // XR interaction
        XRSimpleInteractable xrInteractable = btnObj.AddComponent<XRSimpleInteractable>();
        xrInteractable.selectEntered.AddListener((args) => onClick());
        
        // Background (child, on Ignore Raycast so it doesn't block)
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(btnObj.transform, false);
        bgObj.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = bgColor;
        bgImage.raycastTarget = false;
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        if (name == "pHTestButton") phButtonImage = bgImage;
        
        // Text (child, on Ignore Raycast)
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        textObj.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 36;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.raycastTarget = false;
        
        if (name == "pHTestButton") phButtonText = tmp;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Billboard
        btnObj.AddComponent<LookAtCamera>();
        
        Debug.Log($"✅ Button created: {name} at {localPos}");
        return btnObj;
    }
    
    void HideSelectionMenu()
    {
        if (selectionMenuObj != null)
        {
            Destroy(selectionMenuObj);
            selectionMenuObj = null;
        }
    }
    
    // === BUTTON CALLBACKS ===
    
    void OnFlameTestSelected()
    {
        Debug.Log("▶ Flame Test selected!");
        if (AudioManager.instance != null)
            AudioManager.instance.PlayGrab();
        
        HideSelectionMenu();
        StartScenario(ExperimentScenario.FlameTest);
    }
    
    void OnpHTestSelected()
    {
        Debug.Log("▶ pH Test selected!");
        if (AudioManager.instance != null)
            AudioManager.instance.PlayGrab();
        
        HideSelectionMenu();
        StartScenario(ExperimentScenario.pHTest);
    }
    
    // === SCENARIO MANAGEMENT ===
    
    void StartScenario(ExperimentScenario scenario)
    {
        currentScenario = scenario;
        scenarioCompleted = false;
        
        // Disable all areas
        if (flameTestArea != null) flameTestArea.SetActive(false);
        if (phTestArea != null) phTestArea.SetActive(false);
        
        switch (scenario)
        {
            case ExperimentScenario.FlameTest:
                if (flameTestArea != null) flameTestArea.SetActive(true);
                UpdateMainInstruction("Experiment 1/2: Flame Test\nIdentify chemicals by their flame color.");
                UpdateProgress("Progress: Flame Test");
                Debug.Log("=== FLAME TEST EXPERIMENT STARTED ===");
                break;
                
            case ExperimentScenario.pHTest:
                if (phTestArea != null) phTestArea.SetActive(true);
                UpdateMainInstruction("Experiment 2/2: pH Test\nTest chemical solutions with pH paper.");
                UpdateProgress("Progress: pH Test");
                Debug.Log("=== pH TEST EXPERIMENT STARTED ===");
                break;
                
            case ExperimentScenario.Complete:
                ShowCompletionScreen();
                break;
        }
    }
    
    void CheckScenarioCompletion()
    {
        switch (currentScenario)
        {
            case ExperimentScenario.FlameTest:
                if (flameTestController != null && flameTestController.allSamplesCompleted)
                {
                    CompleteCurrentScenario();
                }
                break;
                
            case ExperimentScenario.pHTest:
                if (phTestController != null && phTestController.allTestsCompleted)
                {
                    CompleteCurrentScenario();
                }
                break;
        }
    }
    
    void CompleteCurrentScenario()
    {
        if (scenarioCompleted) return;
        scenarioCompleted = true;
        
        Debug.Log($"✅ {currentScenario} experiment complete!");
        AddScoreForScenario(currentScenario);
        
        switch (currentScenario)
        {
            case ExperimentScenario.FlameTest:
                UpdateMainInstruction("🎉 Flame Test Complete!\n\nReturning to menu...");
                Invoke("ShowSelectionMenu", 3f);
                break;
                
            case ExperimentScenario.pHTest:
                UpdateMainInstruction("🎉 pH Test Complete!\n\nReturning to menu...");
                Invoke("ShowSelectionMenu", 3f);
                break;
        }
    }
    
    void ShowCompletionOrMenu()
    {
        if (hasAddedFlameScore && hasAddedpHScore)
        {
            StartScenario(ExperimentScenario.Complete);
        }
        else
        {
            ShowSelectionMenu();
        }
    }
    
    // === SCORING ===
    
    void AddScoreForScenario(ExperimentScenario scenario)
    {
        int scoreToAdd = 0;
        
        switch (scenario)
        {
            case ExperimentScenario.FlameTest:
                if (!hasAddedFlameScore)
                {
                    scoreToAdd = 50;
                    hasAddedFlameScore = true;
                }
                break;
                
            case ExperimentScenario.pHTest:
                if (!hasAddedpHScore)
                {
                    scoreToAdd = 50;
                    hasAddedpHScore = true;
                }
                break;
        }
        
        if (scoreToAdd > 0)
        {
            totalScore += scoreToAdd;
            Debug.Log($"Added {scoreToAdd} points. Total: {totalScore}");
        }
    }
    
    // === UI HELPERS ===
    
    void UpdateMainInstruction(string text)
    {
        if (mainInstructionText != null)
        {
            mainInstructionText.text = text;
        }
        Debug.Log("Main Instruction: " + text);
    }
    
    void UpdateProgress(string text)
    {
        if (mainProgressText != null)
        {
            mainProgressText.text = text;
        }
    }
    
    void ShowCompletionScreen()
    {
        Debug.Log("🎉 === ALL EXPERIMENTS COMPLETE === 🎉");
        
        UpdateMainInstruction("🎉 ALL EXPERIMENTS COMPLETE! 🎉");
        
        string progressMessage = $"Both experiments finished!\nTotal Score: {totalScore}/100";
        UpdateProgress(progressMessage);
        
        Debug.Log($"Final Experiment Score: {totalScore}/100");
        
        // Disable all areas
        if (flameTestArea != null) flameTestArea.SetActive(false);
        if (phTestArea != null) phTestArea.SetActive(false);
        
        // Return to menu after delay
        Invoke("ReturnToMenu", 5f);
    }
    
    void ReturnToMenu()
    {
        Debug.Log("Returning to main menu...");
        SceneManager.LoadScene("MainMenu");
    }
}
