using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Auto-builds the main world-space UI panel for ExperimentFlowManager.
/// Only creates the main instruction/progress panel — 
/// Flame Test and pH Test panels already exist in the scene.
/// </summary>
public class ExperimentUIBuilder : MonoBehaviour
{
    [Header("Controller (auto-found if empty)")]
    public ExperimentFlowManager flowManager;
    
    [Header("Panel Positioning")]
    [Tooltip("Where to place the main UI panel")]
    public Vector3 mainUIPanelPosition = new Vector3(0f, 2.2f, 3f);
    
    [Header("Panel Look")]
    public Color panelBackgroundColor = new Color(0.05f, 0.05f, 0.12f, 0.85f);
    public Color headerColor = new Color(0.3f, 0.8f, 1f, 1f);
    public Color instructionColor = new Color(0.95f, 0.95f, 1f, 1f);
    public Color progressColor = new Color(1f, 0.85f, 0.3f, 1f);
    
    void Awake()
    {
        if (flowManager == null) flowManager = FindObjectOfType<ExperimentFlowManager>();
        
        if (flowManager == null)
        {
            Debug.LogWarning("⚠️ ExperimentUIBuilder: No ExperimentFlowManager found!");
            return;
        }
        
        BuildMainPanel();
        Debug.Log("✅ Main UI panel built!");
    }
    
    void BuildMainPanel()
    {
        GameObject panelObj = new GameObject("MainUIPanel");
        panelObj.transform.position = mainUIPanelPosition;
        panelObj.transform.localScale = new Vector3(0.006f, 0.006f, 0.006f);
        panelObj.transform.SetParent(flowManager.transform, true);
        panelObj.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        // World-space canvas
        Canvas canvas = panelObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        CanvasGroup cg = panelObj.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(500, 160);
        
        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(panelObj.transform, false);
        bgObj.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = panelBackgroundColor;
        bgImage.raycastTarget = false;
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        Outline outline = bgObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.3f, 0.6f, 1f, 0.5f);
        outline.effectDistance = new Vector2(2f, 2f);
        
        // Title
        CreateText(panelObj.transform, "Title", "⚗️ VR LAB EXPERIMENTS",
                  new Vector2(0, 50), new Vector2(460, 40),
                  headerColor, 24, FontStyles.Bold);
        
        // Instruction
        TextMeshProUGUI instructionTMP = CreateText(panelObj.transform, "Instruction", "",
                  new Vector2(0, -10), new Vector2(460, 60),
                  instructionColor, 18, FontStyles.Normal);
        
        // Progress
        TextMeshProUGUI progressTMP = CreateText(panelObj.transform, "Progress", "",
                  new Vector2(0, -60), new Vector2(460, 30),
                  progressColor, 16, FontStyles.Bold);
        
        // Assign to flow manager
        flowManager.mainInstructionText = instructionTMP;
        flowManager.mainProgressText = progressTMP;
        
        // Billboard
        panelObj.AddComponent<LookAtCamera>();
    }
    
    TextMeshProUGUI CreateText(Transform parent, string name, string content,
                               Vector2 anchoredPos, Vector2 size,
                               Color color, int fontSize, FontStyles style)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        textObj.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = style;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.raycastTarget = false;
        
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;
        
        return tmp;
    }
}
