using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadLaboratoryScene()
    {
        Debug.Log("Loading Laboratory Scene");
        SceneManager.LoadScene("LaboratoryScene");
    }
    
    public void LoadMainMenu()
    {
        Debug.Log("Loading Main Menu");
        SceneManager.LoadScene("MainMenu");
    }
    
    public void LoadScenario1()
    {
        Debug.Log("Loading Scenario 1 (PPE Training)");
        PlayerPrefs.SetInt("StartScenario", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("LaboratoryScene");
    }
    
    public void LoadScenario2()
    {
        Debug.Log("Loading Scenario 2 (Fire Extinguisher)");
        PlayerPrefs.SetInt("StartScenario", 2);
        PlayerPrefs.Save();
        SceneManager.LoadScene("LaboratoryScene");
    }
    
    public void LoadScenario3()
    {
        Debug.Log("Loading Scenario 3 (Hazard Quiz)");
        PlayerPrefs.SetInt("StartScenario", 3);
        PlayerPrefs.Save();
        SceneManager.LoadScene("LaboratoryScene");
    }

    // NEW: Load Scenario 4 (Chemical Experiment)
    public void LoadScenario4()
    {
        Debug.Log("Loading Scenario 4 (Chemical Experiment)");
        PlayerPrefs.SetInt("StartScenario", 4);
        PlayerPrefs.Save();
        SceneManager.LoadScene("LaboratoryScene");
    }
    
    public void LoadFullTraining()
    {
        Debug.Log("Loading Full Training");
        PlayerPrefs.SetInt("StartScenario", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("LaboratoryScene");
    }
    
    // === EXPERIMENT SCENE METHODS ===
    
    public void LoadExperimentScene()
    {
        Debug.Log("Loading Full Experiment (Flame + pH)");
        PlayerPrefs.SetInt("StartExperiment", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("ExperimentScene");
    }
    
    public void LoadFlameTestOnly()
    {
        Debug.Log("Loading Flame Test Experiment");
        PlayerPrefs.SetInt("StartExperiment", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("ExperimentScene");
    }
    
    public void LoadpHTestOnly()
    {
        Debug.Log("Loading pH Test Experiment");
        PlayerPrefs.SetInt("StartExperiment", 2);
        PlayerPrefs.Save();
        SceneManager.LoadScene("ExperimentScene");
    }
    
    public void QuitApplication()
    {
        Debug.Log("Quitting Application");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}