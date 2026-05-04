using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using System.Collections; // ADDED: Required for IEnumerator

public class HazardQuiz : MonoBehaviour
{
    [System.Serializable]
    public class QuizQuestion
    {
        public string questionText;
        public HazardType correctAnswer;
    }
    
    public enum HazardType
    {
        Flammable,
        Toxic,
        Corrosive,
        Oxidizer,
        Biohazard
    }
    
    [Header("Quiz Settings")]
    public QuizQuestion[] questions;
    public int currentQuestionIndex = 0;
    
    [Header("UI")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI feedbackText;
    
    [Header("Scoring")]
    public int pointsPerCorrect = 10;
    private int totalScore = 0;
    
    [Header("Bottles")]
    public GameObject flammableBottle;
    public GameObject toxicBottle;
    public GameObject corrosiveBottle;
    public GameObject oxidizerBottle;
    public GameObject biohazardBottle;
    
    void Start()
    {
        SetupBottleInteractions();
        ShowCurrentQuestion();
    }
    
    void SetupBottleInteractions()
    {
        SetupBottle(flammableBottle, HazardType.Flammable);
        SetupBottle(toxicBottle, HazardType.Toxic);
        SetupBottle(corrosiveBottle, HazardType.Corrosive);
        SetupBottle(oxidizerBottle, HazardType.Oxidizer);
        SetupBottle(biohazardBottle, HazardType.Biohazard);
    }
    
    void SetupBottle(GameObject bottle, HazardType type)
    {
        if (bottle == null)
        {
            Debug.LogWarning("Bottle is NULL for type: " + type);
            return;
        }
        
        Debug.Log("Setting up GRABBABLE bottle: " + bottle.name + " for type: " + type);
        
        // Remove XRSimpleInteractable if it exists
        XRSimpleInteractable simpleInteractable = bottle.GetComponent<XRSimpleInteractable>();
        if (simpleInteractable != null)
        {
            Destroy(simpleInteractable);
        }
        
        // Add XRGrabInteractable for grabbing
        XRGrabInteractable grabInteractable = bottle.GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = bottle.AddComponent<XRGrabInteractable>();
        }
        
        // Configure grab settings
        grabInteractable.throwOnDetach = false;
        grabInteractable.trackPosition = true;
        grabInteractable.trackRotation = true;
        grabInteractable.movementType = XRBaseInteractable.MovementType.Kinematic;
        
        // Add Rigidbody if missing
        Rigidbody rb = bottle.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = bottle.AddComponent<Rigidbody>();
        }
        
        // Configure Rigidbody
        rb.mass = 0.5f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        // Store original position - FIXED: Changed to BottleData
        BottleData bottleData = bottle.GetComponent<BottleData>();
        if (bottleData == null)
        {
            bottleData = bottle.AddComponent<BottleData>();
            
        }
        bottleData.originalPosition = bottle.transform.position;
        bottleData.originalRotation = bottle.transform.rotation;
        
        // Subscribe to GRAB event
       grabInteractable.selectEntered.AddListener((args) => {
    Debug.Log("🎯 BOTTLE GRABBED: " + type);

    // 🔥 FORCE RELEASE
    if (grabInteractable.isSelected)
    {
        grabInteractable.interactionManager.SelectExit(
            grabInteractable.firstInteractorSelecting,
            grabInteractable
        );
    }

    OnBottleSelected(type, bottle, rb);
});
        Debug.Log("✅ Grabbable bottle setup complete: " + bottle.name);
    }
    
    void ShowCurrentQuestion()
    {
        if (currentQuestionIndex >= questions.Length)
        {
            CompleteQuiz();
            return;
        }
        
        if (questionText != null)
        {
            questionText.text = questions[currentQuestionIndex].questionText;
        }
        
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }
    
    void OnBottleSelected(HazardType selectedType, GameObject bottle, Rigidbody rb)
    {
        HazardType correctType = questions[currentQuestionIndex].correctAnswer;
        
        if (selectedType == correctType)
        {
            // Correct answer!
            Debug.Log("Correct! +" + pointsPerCorrect + " points");
            totalScore += pointsPerCorrect;
            
            // Play success sound
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySuccess();
            }
            
            if (feedbackText != null)
            {
                feedbackText.text = "✓ Correct!";
                feedbackText.color = Color.green;
            }
            
            // Make bottle kinematic so it stops falling
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            
            // Return bottle to original position after short delay
            StartCoroutine(ReturnBottleToPosition(bottle, 0.5f));
            
            // Move to next question
            currentQuestionIndex++;
            Invoke("ShowCurrentQuestion", 2f);
        }
        else
        {
            // Wrong answer
            Debug.Log("Incorrect. Try again.");
            
            // Play error sound
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayError();
            }
            
            if (feedbackText != null)
            {
                feedbackText.text = "✗ Try again";
                feedbackText.color = Color.red;
            }
            
            // Return bottle to position immediately
            StartCoroutine(ReturnBottleToPosition(bottle, 0.3f));
            
            Invoke("ClearFeedback", 1.5f);
        }
        
        UpdateScore();
    }
    
    void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
    }
    
    void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + totalScore;
        }
    }
    
    void CompleteQuiz()
    {
        Debug.Log("Quiz complete! Total score: " + totalScore);
        
        if (questionText != null)
        {
            questionText.text = "✓ Quiz Complete!";
        }
        
        if (feedbackText != null)
        {
            feedbackText.text = "All hazards identified correctly!";
            feedbackText.color = Color.yellow;
        }
        
        // Bonus points
        totalScore += 20;
        UpdateScore();
    }
    
    IEnumerator ReturnBottleToPosition(GameObject bottle, float delay)
{
    yield return new WaitForSeconds(delay);

    if (bottle != null)
    {
        BottleData bottleData = bottle.GetComponent<BottleData>();
        Rigidbody rb = bottle.GetComponent<Rigidbody>();

        if (bottleData != null && rb != null)
        {
            // STOP physics completely
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;   // disable physics

            // Move bottle safely
            bottle.transform.position = bottleData.originalPosition;
            bottle.transform.rotation = bottleData.originalRotation;

            yield return new WaitForSeconds(0.2f);

            // Re-enable physics AFTER reposition
            rb.isKinematic = false;
        }
    }
}
}

// FIXED: Changed class name from BottlePositionTracker to BottleData
public class BottleData : MonoBehaviour
{
    public Vector3 originalPosition;
    public Quaternion originalRotation;
}