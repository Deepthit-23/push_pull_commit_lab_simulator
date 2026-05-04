#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class CreateInstructionPanel : MonoBehaviour
{
    [MenuItem("Tools/Create Flame Test Instruction Panel")]
    static void CreateFlameTestPanel()
    {
        CreatePanel("FlameTestInstructionPanel", new Vector3(-2f, 2f, -3f), new Vector3(0, 0, 0));
    }
    
    [MenuItem("Tools/Create pH Test Instruction Panel")]
    static void CreatepHTestPanel()
    {
        CreatePanel("pHTestInstructionPanel", new Vector3(3f, 2f, -3f), new Vector3(0, 0, 0));
    }
    
    static void CreatePanel(string panelName, Vector3 position, Vector3 rotation)
    {
        // === ROOT CANVAS ===
        GameObject canvasObj = new GameObject(panelName);
        canvasObj.transform.position = position;
        canvasObj.transform.eulerAngles = rotation;
        canvasObj.layer = LayerMask.NameToLayer("UI");
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Set canvas size
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(800, 600);
        canvasRect.localScale = new Vector3(0.002f, 0.002f, 0.002f);
        
        // === BACKGROUND PANEL ===
        GameObject bgPanel = new GameObject("Background");
        bgPanel.transform.SetParent(canvasObj.transform, false);
        bgPanel.layer = LayerMask.NameToLayer("UI");
        
        Image bgImage = bgPanel.AddComponent<Image>();
        bgImage.color = new Color(0.05f, 0.08f, 0.15f, 0.92f); // Dark blue, slightly transparent
        
        RectTransform bgRect = bgPanel.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // === TITLE TEXT ===
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(bgPanel.transform, false);
        titleObj.layer = LayerMask.NameToLayer("UI");
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = panelName.Contains("Flame") ? "🔥 FLAME TEST" : "🧪 pH TEST";
        titleText.fontSize = 48;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = new Color(1f, 0.85f, 0.3f); // Gold
        titleText.alignment = TextAlignmentOptions.Center;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.85f);
        titleRect.anchorMax = new Vector2(1, 1f);
        titleRect.offsetMin = new Vector2(20, 0);
        titleRect.offsetMax = new Vector2(-20, -10);
        
        // === INSTRUCTION TEXT (main — this gets updated by script) ===
        GameObject instrObj = new GameObject("InstructionText");
        instrObj.transform.SetParent(bgPanel.transform, false);
        instrObj.layer = LayerMask.NameToLayer("UI");
        
        TextMeshProUGUI instrText = instrObj.AddComponent<TextMeshProUGUI>();
        instrText.text = "Loading instructions...";
        instrText.fontSize = 28;
        instrText.color = Color.white;
        instrText.alignment = TextAlignmentOptions.TopLeft;
        instrText.enableWordWrapping = true;
        instrText.overflowMode = TextOverflowModes.Overflow;
        
        RectTransform instrRect = instrObj.GetComponent<RectTransform>();
        instrRect.anchorMin = new Vector2(0, 0.35f);
        instrRect.anchorMax = new Vector2(1, 0.85f);
        instrRect.offsetMin = new Vector2(30, 0);
        instrRect.offsetMax = new Vector2(-30, 0);
        
        // === RESULT TEXT ===
        GameObject resultObj = new GameObject("ResultText");
        resultObj.transform.SetParent(bgPanel.transform, false);
        resultObj.layer = LayerMask.NameToLayer("UI");
        
        TextMeshProUGUI resultText = resultObj.AddComponent<TextMeshProUGUI>();
        resultText.text = "";
        resultText.fontSize = 26;
        resultText.color = new Color(0.3f, 1f, 0.5f); // Green
        resultText.alignment = TextAlignmentOptions.TopLeft;
        resultText.enableWordWrapping = true;
        
        RectTransform resultRect = resultObj.GetComponent<RectTransform>();
        resultRect.anchorMin = new Vector2(0, 0.1f);
        resultRect.anchorMax = new Vector2(1, 0.35f);
        resultRect.offsetMin = new Vector2(30, 0);
        resultRect.offsetMax = new Vector2(-30, 0);
        
        // === SCORE TEXT ===
        GameObject scoreObj = new GameObject("ScoreText");
        scoreObj.transform.SetParent(bgPanel.transform, false);
        scoreObj.layer = LayerMask.NameToLayer("UI");
        
        TextMeshProUGUI scoreTextTMP = scoreObj.AddComponent<TextMeshProUGUI>();
        scoreTextTMP.text = "Score: 0 | Samples: 0/5";
        scoreTextTMP.fontSize = 24;
        scoreTextTMP.color = new Color(0.6f, 0.8f, 1f); // Light blue
        scoreTextTMP.alignment = TextAlignmentOptions.Center;
        scoreTextTMP.fontStyle = FontStyles.Bold;
        
        RectTransform scoreRect = scoreObj.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 0f);
        scoreRect.anchorMax = new Vector2(1, 0.1f);
        scoreRect.offsetMin = new Vector2(20, 5);
        scoreRect.offsetMax = new Vector2(-20, 0);
        
        // === BORDER (thin outline) ===
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(canvasObj.transform, false);
        borderObj.layer = LayerMask.NameToLayer("UI");
        
        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = new Color(0.3f, 0.6f, 1f, 0.6f); // Blue border
        
        Outline outline = borderObj.AddComponent<Outline>();
        outline.effectColor = new Color(0.3f, 0.6f, 1f, 0.8f);
        outline.effectDistance = new Vector2(3, 3);
        
        RectTransform borderRect = borderObj.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(-5, -5);
        borderRect.offsetMax = new Vector2(5, 5);
        borderImage.raycastTarget = false;
        borderImage.color = new Color(0, 0, 0, 0); // Invisible fill, only outline shows
        
        // Move border behind background
        borderObj.transform.SetAsFirstSibling();
        
        // === SELECT ===
        Selection.activeGameObject = canvasObj;
        
        Debug.Log($"✅ {panelName} created! Assign the TMP text objects to your controller script.");
        
        EditorUtility.DisplayDialog(
            $"{panelName} Created!",
            $"Panel created at position {position}.\n\n" +
            "It has 3 text fields:\n" +
            "• InstructionText — drag to controller's 'Instruction Text' field\n" +
            "• ResultText — drag to controller's 'Result Text' field\n" +
            "• ScoreText — drag to controller's 'Score Text' field\n\n" +
            "Move the panel so you can see it from where you stand.",
            "OK"
        );
    }
}
#endif
