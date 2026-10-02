using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private UIView view;

    private UIModel model;
    private PlayerController player;
    private SaveInteractor saveInteractor;

    private void Awake()
    {
        model = new UIModel();

        // SaveInteractor берём из ServiceLocator (зарегистрирован в GameBootstrapper)
        if (ServiceLocator.TryGet<SaveInteractor>(out var interactor))
        {
            saveInteractor = interactor;
        }
        else
        {
            // Fallback: если GameBootstrapper не отработал (например, запуск сразу игровой сцены)
            saveInteractor = new SaveInteractor(
                new JsonPlayerRepository(),
                new JsonEnemyRepository()
            );
            ServiceLocator.Register(saveInteractor);
            Debug.LogWarning("SaveInteractor не был зарегистрирован в GameBootstrapper. Создан локально.");
        }
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();

        GameEvents.PlayerDied += OnPlayerDied;
        GameEvents.ScoreChanged += OnScoreChanged;
        GameEvents.BossMessage    += view.ShowBossMessage;
        GameEvents.BossDefeated   += OnBossDefeated;
        GameEvents.VictoryAchieved   += OnVictoryAchieved;

        view.OnSaveRequested += SaveGame;
        view.OnLoadRequested += LoadGame;
        view.OnMainMenuRequested += QuitToMainMenu;
        view.OnResumeRequested += ResumeGame;
        view.OnRestartRequested += RestartGame;

        view.ShowGameOver(false);
        view.ShowPauseMenu(false);
        view.ShowVictory(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Начальный счёт
        if (ServiceLocator.TryGet<ScoreManager>(out var score))
            OnScoreChanged(score.CurrentScore);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePauseMenu();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (player == null) return;

        // Теперь данные напрямую из PlayerController (MVC для игрока)
        view.UpdateHealth(player.CurrentHealth, player.MaxHealth);
        view.UpdateMana(player.CurrentMana, player.MaxMana);
        view.UpdateMagicCooldown(Mathf.Clamp01(player.TimeSinceLastMagic / player.MagicCooldownTime));
    }

    private void OnPlayerDied()
    {
        model.IsGameOver = true;
        view.ShowGameOver(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Через ServiceLocator
        if (ServiceLocator.TryGet<IAudioService>(out var audio))
            audio.PlayDeathSound();
    }

    private void OnScoreChanged(int score)
    {
        model.Score = score;
        view.UpdateScore(score);
    }

    public void TogglePauseMenu()
    {
        model.IsPaused = !model.IsPaused;
        view.ShowPauseMenu(model.IsPaused);
        
        Time.timeScale = model.IsPaused ? 0 : 1;
        Cursor.lockState = model.IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = model.IsPaused;
    }

    public void ResumeGame()
    {
        model.IsPaused = false;
        view.ShowPauseMenu(false);
        view.ShowVictory(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SaveGame()
    {
        if (player == null)
        {
            Debug.LogError("Player не найден, сохранение отменено");
            return;
        }

        // Оркестрация внутри SaveInteractor
        if (ServiceLocator.TryGet<ScoreManager>(out var score))
            saveInteractor.SaveGame(player, score);
        else
            saveInteractor.SaveGame(player, null);

        Debug.Log("Игра сохранена");
    }

    public void LoadGame()
    {
        if (player == null)
        {
            Debug.LogError("Player не найден, загрузка отменена");
            return;
        }

        if (ServiceLocator.TryGet<ScoreManager>(out var score))
            saveInteractor.LoadGame(player, score);
        else
            saveInteractor.LoadGame(player, null);

        ResumeGame();
    }

    public void RestartGame()
    {
        if (ServiceLocator.TryGet<ScoreManager>(out var score))
            score.ResetScore();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        if (ServiceLocator.TryGet<ScoreManager>(out var score))
            score.ResetScore();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnBossDefeated()
    {
        Debug.Log("Босс побеждён!");
    }

    private void OnVictoryAchieved()
    {
        // Не показываем победу, если игрок уже мёртв
        if (model.IsGameOver) return;

        model.IsPaused = false;
        view.ShowPauseMenu(false);
        view.ShowGameOver(false);
        view.ShowVictory(true);

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (ServiceLocator.TryGet<IAudioService>(out var audio))
            audio.PlayVictoryMusic();

        Debug.Log("Победа!");
    }

    private void OnDestroy()
    {
        GameEvents.PlayerDied -= OnPlayerDied;
        GameEvents.ScoreChanged -= OnScoreChanged;
        GameEvents.BossMessage  -= view.ShowBossMessage;
        GameEvents.BossDefeated -= OnBossDefeated;
        GameEvents.VictoryAchieved -= OnVictoryAchieved;
    }

    
}