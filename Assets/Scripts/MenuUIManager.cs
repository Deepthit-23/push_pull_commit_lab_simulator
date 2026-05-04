using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject instructionsPanel;
    public GameObject scenarioSelectPanel; // NEW
    
    void Start()
    {
        ShowMainMenu();
    }
    
    public void ShowMainMenu()
    {
        Debug.Log("Showing Main Menu");
        
        // Show main menu, hide everything else
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
            
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
            
        if (scenarioSelectPanel != null)
            scenarioSelectPanel.SetActive(false);
    }
    
    public void ShowInstructions()
    {
        Debug.Log("Showing Instructions");
        
        // Hide main menu, show instructions
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
            
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
            
        if (scenarioSelectPanel != null)
            scenarioSelectPanel.SetActive(false);
    }
    
    public void ShowScenarioSelect()
    {
        Debug.Log("Showing Scenario Select");
        
        // Hide main menu, show scenario select
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
            
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
            
        if (scenarioSelectPanel != null)
            scenarioSelectPanel.SetActive(true);
    }
}