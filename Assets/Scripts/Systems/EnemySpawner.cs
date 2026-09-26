using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private bool randomizePosition = true;

    private void Start()
    {
        Vector3 pos = transform.position;
        if (randomizePosition)
        {
            Vector3 rnd = Random.insideUnitSphere * spawnRadius;
            rnd.y = 0;
            pos += rnd;
        }
        SpawnEnemy(pos);
    }

    public void SpawnEnemy(Vector3 pos)
    {
        if (enemyPrefab == null) return;
        var enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

        var melee = enemy.GetComponent<MeleeEnemy>();
        if (melee != null) melee.SetWeapon(weaponType);

        var ranged = enemy.GetComponent<RangedEnemy>();
        if (ranged != null) ranged.SetWeapon(weaponType);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}