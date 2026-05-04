using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PPEManager : MonoBehaviour
{
    [Header("PPE Status")]
    public bool coatWorn = false;
    public bool gogglesWorn = false;
    public bool gloveWorn = false;
    
    [Header("UI References")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI scoreText;
    
    [Header("Scoring")]
    public int scorePerItem = 10;
    private int totalScore = 0;
    
    void Start()
    {
        UpdateUI();
    }
    
    public void OnItemWorn(WearableItem.PPEType ppeType)
    {
        // Mark item as worn
        switch (ppeType)
        {
            case WearableItem.PPEType.LabCoat:
                if (!coatWorn)
                {
                    coatWorn = true;
                    AddScore(scorePerItem);
                    Debug.Log("Lab coat worn! +10 points");
                }
                break;
                
            case WearableItem.PPEType.Goggles:
                if (!gogglesWorn)
                {
                    gogglesWorn = true;
                    AddScore(scorePerItem);
                    Debug.Log("Goggles worn! +10 points");
                }
                break;
                
            case WearableItem.PPEType.Glove:
                if (!gloveWorn)
                {
                    gloveWorn = true;
                    AddScore(scorePerItem);
                    Debug.Log("Glove worn! +10 points");
                }
                break;
        }
        
        UpdateUI();
        CheckCompletion();
    }
    
    void AddScore(int points)
    {
        totalScore += points;
    }
    
    void UpdateUI()
    {
        // Update checklist
        if (statusText != null)
        {
            string status = "PPE Checklist:\n";
            status += (coatWorn ? "✓" : "☐") + " Lab Coat\n";
            status += (gogglesWorn ? "✓" : "☐") + " Safety Goggles\n";
            status += (gloveWorn ? "✓" : "☐") + " Lab Gloves";
            
            statusText.text = status;
        }
        
        // Update score
        if (scoreText != null)
        {
            scoreText.text = "Score: " + totalScore;
        }
    }
    
    void CheckCompletion()
    {
        if (coatWorn && gogglesWorn && gloveWorn)
        {
            Debug.Log("All PPE equipped! Safe to enter lab!");
            if (statusText != null)
            {
                statusText.text = "✓ All PPE Equipped!\nSafe to enter lab!";
            }
            
            // Bonus points for completing
            AddScore(10);
            UpdateUI();
        }
    }
}
