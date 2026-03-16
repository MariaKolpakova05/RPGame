using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;
    [SerializeField] private int meleeCount = 3;
    [SerializeField] private int rangedCount = 2;
    [SerializeField] private Vector2 spawnArea = new Vector2(40f, 40f);
    
    void Start()
    {
        SpawnEnemies();
    }
    
    void SpawnEnemies()
    {
        for (int i = 0; i < meleeCount; i++)
        {
            SpawnEnemy(meleeEnemyPrefab);
        }
        
        for (int i = 0; i < rangedCount; i++)
        {
            SpawnEnemy(rangedEnemyPrefab);
        }
    }
    
    void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            0,
            Random.Range(-spawnArea.y / 2, spawnArea.y / 2)
        );
        
        // Поиск точки на NavMesh
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(randomPosition, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas))
        {
            Instantiate(enemyPrefab, hit.position, Quaternion.identity);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnArea.x, 1, spawnArea.y));
    }
}