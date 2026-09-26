using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;
    public bool HasSpawned { get; private set; } = false;
    
    public void SpawnBoss()
    {
        if (!HasSpawned && bossPrefab != null)
        {
            Instantiate(bossPrefab, transform.position, Quaternion.identity);
            HasSpawned = true;
        }
    }
}