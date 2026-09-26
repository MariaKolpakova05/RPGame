using UnityEngine;

public class SaveInteractor
{
    private readonly IPlayerRepository _playerRepo;
    private readonly IEnemyRepository _enemyRepo;

    public SaveInteractor(IPlayerRepository playerRepo, IEnemyRepository enemyRepo)
    {
        _playerRepo = playerRepo;
        _enemyRepo = enemyRepo;
    }

    public void SaveGame(PlayerController player, ScoreManager score)
    {
        var playerData = new PlayerData
        {
            health = player.CurrentHealth,
            maxHealth = player.MaxHealth,
            mana = player.CurrentMana,
            maxMana = player.MaxMana,
            positionX = player.transform.position.x,
            positionY = player.transform.position.y,
            positionZ = player.transform.position.z,
            score = score != null ? score.CurrentScore : 0
        };
        _playerRepo.Save(playerData);

        var enemies = Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        var positions = new float[enemies.Length * 3];
        for (int i = 0; i < enemies.Length; i++)
        {
            positions[i * 3] = enemies[i].transform.position.x;
            positions[i * 3 + 1] = enemies[i].transform.position.y;
            positions[i * 3 + 2] = enemies[i].transform.position.z;
        }
        _enemyRepo.Save(positions);

        Debug.Log("Игра сохранена (player + enemies)");
    }

    public bool HasSave() => _playerRepo.HasSavedData();

    public void LoadGame(PlayerController player, ScoreManager score)
    {
        var playerData = _playerRepo.Load();
        if (playerData == null)
        {
            Debug.LogWarning("Нет сохранения игрока");
            return;
        }

        // Позиция
        var cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;
        player.transform.position = new Vector3(playerData.positionX, playerData.positionY, playerData.positionZ);
        if (cc) cc.enabled = true;

        // HP
        float hpDiff = playerData.health - player.CurrentHealth;
        if (hpDiff > 0) player.Heal(hpDiff);
        else if (hpDiff < 0) player.TakeDamage(-hpDiff, DamageType.Physical);

        // MP
        player.SetMana(playerData.mana);

        // Score
        if (score != null && playerData.score > score.CurrentScore)
            score.AddScore(playerData.score - score.CurrentScore);

        // Враги
        var positions = _enemyRepo.Load();
        if (positions != null)
        {
            var enemies = Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
            int count = Mathf.Min(enemies.Length, positions.Length / 3);
            for (int i = 0; i < count; i++)
            {
                var agent = enemies[i].GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent) agent.enabled = false;
                enemies[i].transform.position = new Vector3(
                    positions[i * 3], positions[i * 3 + 1], positions[i * 3 + 2]);
                if (agent) agent.enabled = true;
            }
        }

        Debug.Log("Игра загружена");
    }

    public void DeleteSave()
    {
        _playerRepo.Clear();
        _enemyRepo.Clear();
    }
}