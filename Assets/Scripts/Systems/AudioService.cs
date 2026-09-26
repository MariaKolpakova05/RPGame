using UnityEngine;

public class AudioService : MonoBehaviour, IAudioService
{
    private static AudioService instance;
    private AudioSource audioSource;
    
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip magicSound;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Настройка громкости из сохранений
        audioSource.volume = PlayerPrefs.GetFloat("Volume", 1f);
    }
    
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && audioSource != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    
    public void PlayVictoryMusic()
    {
        if (victoryMusic != null && audioSource != null)
        {
            audioSource.PlayOneShot(victoryMusic);
        }
    }
    
    public void PlayDeathSound()
    {
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
    }
    
    public void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }
    
    public void PlayMagicSound()
    {
        if (magicSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(magicSound);
        }
    }
    
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
    
    public static AudioService Instance => instance;

    public float GetVolume() => audioSource != null ? audioSource.volume : 1f;
}