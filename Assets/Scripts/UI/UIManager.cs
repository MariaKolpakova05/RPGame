using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    
    [Header("Magic Cooldown")]
    [SerializeField] private Image magicIcon;
    [SerializeField] private float magicCooldown = 2f;
    
    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    
    private PlayerCombat playerCombat;
    private HealthSystem playerHealth;
    private float currentMagicCooldown;
    
    void Start()
    {
        // Поиск игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<HealthSystem>();
            playerCombat = player.GetComponent<PlayerCombat>();
        }
        
        // Настройка кнопки рестарта
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    
    void Update()
    {
        UpdateHealthBar();
        UpdateMagicCooldown();
    }
    
    void UpdateHealthBar()
    {
        if (playerHealth != null && healthSlider != null)
        {
            healthSlider.value = playerHealth.CurrentHealth / playerHealth.MaxHealth;
            
            if (healthText != null)
                healthText.text = $"{playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
        }
    }
    
    void UpdateMagicCooldown()
    {
        if (playerCombat != null && magicIcon != null)
        {
            // Здесь нужно получить текущий кулдаун из PlayerCombat
            // Для примера используем простую логику
            if (currentMagicCooldown > 0)
            {
                currentMagicCooldown -= Time.deltaTime;
                magicIcon.fillAmount = currentMagicCooldown / magicCooldown;
            }
            else
            {
                magicIcon.fillAmount = 1f;
            }
        }
    }
    
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}