using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    private int currentScore = 0;
    private int enemiesKilled = 0;
    private bool bossSpawned = false;
    private bool _victoryTriggered = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //не уничтожать при загрузке сцены
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        GameEvents.ScoreChanged?.Invoke(currentScore); //оповещение UI
    }
    
    public void OnEnemyKilled()
    {
        enemiesKilled++;
        GameEvents.EnemyKilled?.Invoke(enemiesKilled);
        
        //спавн босса после убийства 3 мобов
        if (enemiesKilled >= 3 && !bossSpawned)
        {
            SpawnBoss();
            bossSpawned = true;
            GameEvents.BossAppeared?.Invoke();
        }
        
        // Победная мелодия после 5 убийств
        if (enemiesKilled >= 5 && !_victoryTriggered)
        {
            _victoryTriggered = true;
            GameEvents.VictoryAchieved?.Invoke();
        }
    }
    
    private void SpawnBoss()
    {
        //найти спавнер босса и активировать
        BossSpawner[] spawners = FindObjectsByType<BossSpawner>(FindObjectsSortMode.None);
        foreach (var spawner in spawners)
        {
            if (!spawner.HasSpawned)
            {
                spawner.SpawnBoss();
                break;
            }
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        enemiesKilled = 0;
        bossSpawned = false;      // ← важно, иначе на новой игре босс не заспавнится
        _victoryTriggered = false; // ← если добавляла флаг для победы
        GameEvents.ScoreChanged?.Invoke(currentScore);
        GameEvents.EnemyKilled?.Invoke(enemiesKilled);
        Debug.Log("ScoreManager сброшен");
    }
    //чтение из вне
    public int CurrentScore => currentScore;
    public int EnemiesKilled => enemiesKilled;
}