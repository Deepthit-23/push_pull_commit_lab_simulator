using UnityEngine;
using TMPro;

public class ExperimentUIManager : MonoBehaviour
{
    [Header("Result Panel")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultTitle;
    public TextMeshProUGUI resultDescription;
    public UnityEngine.UI.Image panelBackground;

    [Header("Colors")]
    public Color safeColor = new Color(0.1f, 0.8f, 0.1f, 0.9f);
    public Color dangerColor = new Color(0.8f, 0.1f, 0.1f, 0.9f);

    [Header("References")]
    public MixingBeaker mixingBeaker;
    public ChemicalBottle[] chemicalBottles;

    // NEW: TrainingFlowManager checks this to know experiment is done
    [HideInInspector]
    public bool experimentCompleted = false;

    void Start()
    {
        HideResultPanel();
    }

    public void ShowSafeReaction(string title, string description)
    {
        resultPanel.SetActive(true);
        resultTitle.text = "✓ " + title;
        resultDescription.text = description;
        panelBackground.color = safeColor;

        // NEW: Mark experiment as completed when any reaction happens
        experimentCompleted = true;

        Debug.Log("Showing safe reaction UI");
    }

    public void ShowDangerousReaction(string title, string description)
    {
        resultPanel.SetActive(true);
        resultTitle.text = "⚠ " + title;
        resultDescription.text = description;
        panelBackground.color = dangerColor;

        // NEW: Mark experiment as completed even for dangerous reactions
        // Player still learned what NOT to do!
        experimentCompleted = true;

        Debug.Log("Showing dangerous reaction UI");
    }

    public void HideResultPanel()
    {
        resultPanel.SetActive(false);
    }

    public void ResetExperiment()
    {
        HideResultPanel();

        // NEW: Reset completion flag so player can try again
        experimentCompleted = false;

        if (mixingBeaker != null)
            mixingBeaker.ResetBeaker();

        foreach (ChemicalBottle bottle in chemicalBottles)
        {
            if (bottle != null)
                bottle.ResetBottle();
        }

        Debug.Log("Experiment has been reset!");
    }
}