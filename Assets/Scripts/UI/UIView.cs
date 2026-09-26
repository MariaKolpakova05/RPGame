using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIView : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Mana (опционально)")]
    [SerializeField] private Image manaFill;
    [SerializeField] private TextMeshProUGUI mpText;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Magic Cooldown")]
    [SerializeField] private Image magicCooldownImage;

    [Header("Panels")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject pauseMenu;

    [Header("Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;

    public event Action OnSaveRequested;
    public event Action OnLoadRequested;
    public event Action OnMainMenuRequested;
    public event Action OnResumeRequested;
    public event Action OnRestartRequested;

    private void Awake()
    {
        if (saveButton) saveButton.onClick.AddListener(() => OnSaveRequested?.Invoke());
        if (loadButton) loadButton.onClick.AddListener(() => OnLoadRequested?.Invoke());
        if (mainMenuButton) mainMenuButton.onClick.AddListener(() => OnMainMenuRequested?.Invoke());
        if (resumeButton) resumeButton.onClick.AddListener(() => OnResumeRequested?.Invoke());
        if (restartButton) restartButton.onClick.AddListener(() => OnRestartRequested?.Invoke());
    }

    public void UpdateHealth(float current, float max)
    {
        if (healthFill != null)
            healthFill.fillAmount = max > 0 ? current / max : 0f;

        if (hpText != null)
            hpText.text = $"HP: {current:F0}/{max:F0}";
    }

    public void UpdateMana(float current, float max)
    {
        if (manaFill != null)
            manaFill.fillAmount = max > 0 ? current / max : 0f;

        if (mpText != null)
            mpText.text = $"MP: {current:F0}/{max:F0}";
    }

    public void UpdateScore(int score)
    {
        if (scoreText) scoreText.text = $"Score: {score}";
    }

    public void UpdateMagicCooldown(float progress)
    {
        if (magicCooldownImage) magicCooldownImage.fillAmount = progress;
    }

    public void ShowGameOver(bool show) => gameOverScreen?.SetActive(show);
    public void ShowPauseMenu(bool show) => pauseMenu?.SetActive(show);
}