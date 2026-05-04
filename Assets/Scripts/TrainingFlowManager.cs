using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TrainingFlowManager : MonoBehaviour
{
    public enum TrainingScenario
    {
        PPE,
        FireExtinguisher,
        HazardQuiz,
        ChemicalExperiment,  // NEW: Scenario 4
        Complete
    }
    
    [Header("Current State")]
    public TrainingScenario currentScenario = TrainingScenario.PPE;
    private bool scenarioCompleted = false;
    
    [Header("Scenario GameObjects")]
    public GameObject ppeArea;
    public GameObject fireArea;
    public GameObject quizArea;
    public GameObject chemExperimentArea;  // NEW: Drag ChemicalExperimentStation here
    
    [Header("UI")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI progressText;
    
    [Header("Managers")]
    public PPEManager ppeManager;
    public HazardQuiz hazardQuiz;
    public ExperimentUIManager experimentUIManager;  // NEW: Drag ExperimentUIManager here
    
    private int totalScore = 0;
    private bool hasAddedPPEScore = false;
    private bool hasAddedFireScore = false;
    private bool hasAddedQuizScore = false;
    private bool hasAddedChemScore = false;  // NEW
    
    void Start()
    {
        int selectedScenario = PlayerPrefs.GetInt("StartScenario", 0);
        
        if (selectedScenario == 1)
        {
            Debug.Log("Starting directly at PPE scenario");
            StartScenario(TrainingScenario.PPE);
            PlayerPrefs.DeleteKey("StartScenario");
        }
        else if (selectedScenario == 2)
        {
            Debug.Log("Starting directly at Fire scenario");
            StartScenario(TrainingScenario.FireExtinguisher);
            PlayerPrefs.DeleteKey("StartScenario");
        }
        else if (selectedScenario == 3)
        {
            Debug.Log("Starting directly at Quiz scenario");
            StartScenario(TrainingScenario.HazardQuiz);
            PlayerPrefs.DeleteKey("StartScenario");
        }
        else if (selectedScenario == 4)
        {
            // NEW: Start directly at Chemical Experiment
            Debug.Log("Starting directly at Chemical Experiment scenario");
            StartScenario(TrainingScenario.ChemicalExperiment);
            PlayerPrefs.DeleteKey("StartScenario");
        }
        else
        {
            // Full training from beginning
            Debug.Log("Starting full training from PPE");
            StartScenario(TrainingScenario.PPE);
        }
    }
    
    void Update()
    {
        if (!scenarioCompleted)
        {
            CheckScenarioCompletion();
        }
    }
    
    void StartScenario(TrainingScenario scenario)
    {
        currentScenario = scenario;
        scenarioCompleted = false;
        
        // Disable all areas
        if (ppeArea != null) ppeArea.SetActive(false);
        if (fireArea != null) fireArea.SetActive(false);
        if (quizArea != null) quizArea.SetActive(false);
        if (chemExperimentArea != null) chemExperimentArea.SetActive(false);  // NEW
        
        switch (scenario)
        {
            case TrainingScenario.PPE:
                if (ppeArea != null) ppeArea.SetActive(true);
                UpdateInstructions("Scenario 1/4: Put on all PPE equipment");
                UpdateProgress("Progress: PPE Safety");
                Debug.Log("=== PPE SCENARIO STARTED ===");
                break;
                
            case TrainingScenario.FireExtinguisher:
                if (fireArea != null) fireArea.SetActive(true);
                UpdateInstructions("Scenario 2/4: Extinguish the fire");
                UpdateProgress("Progress: Fire Safety");
                Debug.Log("=== FIRE EXTINGUISHER SCENARIO STARTED ===");
                break;
                
            case TrainingScenario.HazardQuiz:
                if (quizArea != null) quizArea.SetActive(true);
                UpdateInstructions("Scenario 3/4: Identify all hazardous chemicals");
                UpdateProgress("Progress: Hazard Recognition");
                Debug.Log("=== HAZARD QUIZ SCENARIO STARTED ===");
                break;

            case TrainingScenario.ChemicalExperiment:
                // NEW: Enable chemical experiment area
                if (chemExperimentArea != null) chemExperimentArea.SetActive(true);
                UpdateInstructions("Scenario 4/4: Mix chemicals safely!");
                UpdateProgress("Progress: Chemical Safety");
                Debug.Log("=== CHEMICAL EXPERIMENT SCENARIO STARTED ===");
                break;
                
            case TrainingScenario.Complete:
                ShowCompletionScreen();
                break;
        }
    }
    
    void CheckScenarioCompletion()
    {
        switch (currentScenario)
        {
            case TrainingScenario.PPE:
                if (ppeManager != null && ppeManager.coatWorn && 
                    ppeManager.gogglesWorn && ppeManager.gloveWorn)
                {
                    CompleteCurrentScenario();
                }
                break;
                
            case TrainingScenario.FireExtinguisher:
                FireController fire = FindObjectOfType<FireController>();
                if (fire != null && fire.isExtinguished)
                {
                    CompleteCurrentScenario();
                }
                break;
                
            case TrainingScenario.HazardQuiz:
                if (hazardQuiz != null && 
                    hazardQuiz.currentQuestionIndex >= hazardQuiz.questions.Length)
                {
                    CompleteCurrentScenario();
                }
                break;

            case TrainingScenario.ChemicalExperiment:
                // NEW: Check if experiment is complete
                // Experiment is complete when player has done at least one reaction
                if (experimentUIManager != null && 
                    experimentUIManager.experimentCompleted)
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
        
        Debug.Log($"✅ {currentScenario} scenario complete!");
        
        AddScoreForScenario(currentScenario);
        
        string completionMessage = "";
        switch (currentScenario)
        {
            case TrainingScenario.PPE:
                completionMessage = "✓ PPE Complete! Moving to Fire Safety...";
                break;
            case TrainingScenario.FireExtinguisher:
                completionMessage = "✓ Fire Safety Complete! Moving to Hazard Quiz...";
                break;
            case TrainingScenario.HazardQuiz:
                completionMessage = "✓ Quiz Complete! Moving to Chemical Experiment...";
                break;
            case TrainingScenario.ChemicalExperiment:
                completionMessage = "✓ Chemical Experiment Complete! Finishing training...";
                break;
        }
        
        UpdateInstructions(completionMessage);
        Invoke("NextScenario", 3f);
    }
    
    void AddScoreForScenario(TrainingScenario scenario)
    {
        int scoreToAdd = 0;
        
        switch (scenario)
        {
            case TrainingScenario.PPE:
                if (!hasAddedPPEScore)
                {
                    scoreToAdd = 40;
                    hasAddedPPEScore = true;
                }
                break;
                
            case TrainingScenario.FireExtinguisher:
                if (!hasAddedFireScore)
                {
                    scoreToAdd = 50;
                    hasAddedFireScore = true;
                }
                break;
                
            case TrainingScenario.HazardQuiz:
                if (!hasAddedQuizScore)
                {
                    scoreToAdd = 70;
                    hasAddedQuizScore = true;
                }
                break;

            case TrainingScenario.ChemicalExperiment:
                // NEW: 50 points for completing chemical experiment
                if (!hasAddedChemScore)
                {
                    scoreToAdd = 50;
                    hasAddedChemScore = true;
                }
                break;
        }
        
        if (scoreToAdd > 0)
        {
            totalScore += scoreToAdd;
            Debug.Log($"Added {scoreToAdd} points. Total: {totalScore}");
        }
    }
    
    void NextScenario()
    {
        currentScenario++;
        
        if (currentScenario > TrainingScenario.Complete)
        {
            currentScenario = TrainingScenario.Complete;
        }
        
        Debug.Log($"Moving to next scenario: {currentScenario}");
        StartScenario(currentScenario);
    }
    
    void UpdateInstructions(string text)
    {
        if (instructionText != null)
            instructionText.text = text;
        Debug.Log("Instructions: " + text);
    }
    
    void UpdateProgress(string text)
    {
        if (progressText != null)
            progressText.text = text;
        Debug.Log("Progress: " + text);
    }
    
    void ShowCompletionScreen()
    {
        Debug.Log("🎉 === TRAINING COMPLETE === 🎉");
        
        UpdateInstructions("🎉 TRAINING COMPLETE! 🎉");

        // Updated total score from 160 to 210
        string progressMessage = $"All scenarios finished!\nTotal Score: {totalScore}/210";
        UpdateProgress(progressMessage);
        
        Debug.Log($"Final Score: {totalScore}/210");
        
        if (ppeArea != null) ppeArea.SetActive(false);
        if (fireArea != null) fireArea.SetActive(false);
        if (quizArea != null) quizArea.SetActive(false);
        if (chemExperimentArea != null) chemExperimentArea.SetActive(false);  // NEW
        
        Invoke("ReturnToMenu", 5f);
    }
    
    void ReturnToMenu()
    {
        Debug.Log("Returning to main menu...");
        SceneManager.LoadScene("MainMenu");
    }
}