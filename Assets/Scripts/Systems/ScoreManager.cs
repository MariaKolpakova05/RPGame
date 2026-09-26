using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    private int currentScore = 0;
    private int enemiesKilled = 0;
    private bool bossSpawned = false;
    
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
        if (enemiesKilled >= 5)
        {
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
    //чтение из вне
    public int CurrentScore => currentScore;
    public int EnemiesKilled => enemiesKilled;
}