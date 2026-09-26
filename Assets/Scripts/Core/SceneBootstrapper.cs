using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBootstrapper : MonoBehaviour
{
    [Header("Scene Setup")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject uiPrefab;

    private void Awake()
    {
        // Применяем выбранный в главном меню режим
        bool peaceful = PlayerPrefs.GetInt("PeacefulMode", 0) == 1;
        EnemyBase.PeacefulMode = peaceful;
        BossEnemy.PeacefulMode = peaceful;
        Debug.Log($"Сцена загружена. Мирный режим: {peaceful}");

        SetupScene();
    }

    private void SetupScene()
    {
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            var player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            player.tag = "Player";
        }

        if (uiPrefab != null && FindFirstObjectByType<UIController>() == null)
            Instantiate(uiPrefab);

        // Подписываемся на события
        GameEvents.EnemyKilled += OnEnemyKilled;
        GameEvents.VictoryAchieved += OnVictoryAchieved;
    }

    private void OnEnemyKilled(int count) => Debug.Log($"Enemy killed: {count}/5");

    private void OnVictoryAchieved()
    {
        Debug.Log("Victory!");
        if (ServiceLocator.TryGet<IAudioService>(out var audio))
            audio.PlayVictoryMusic();
    }

    private void OnDestroy()
    {
        GameEvents.EnemyKilled -= OnEnemyKilled;
        GameEvents.VictoryAchieved -= OnVictoryAchieved;
    }
}