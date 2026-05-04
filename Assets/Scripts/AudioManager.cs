using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [Header("UI Sounds")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;
    
    [Header("Interaction Sounds")]
    public AudioClip grab;
    public AudioClip release;
    public AudioClip attach;
    
    [Header("Feedback Sounds")]
    public AudioClip success;
    public AudioClip error;
    public AudioClip complete;
    
    [Header("Ambient Sounds")]
    public AudioClip fireLoop;
    public AudioClip sprayLoop;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
    
    public void PlayButtonClick()
    {
        PlaySound(buttonClick, 0.5f);
    }
    
    public void PlayGrab()
    {
        PlaySound(grab, 0.7f);
    }
    
    public void PlaySuccess()
    {
        PlaySound(success, 0.8f);
    }
    
    public void PlayError()
    {
        PlaySound(error, 0.6f);
    }
}