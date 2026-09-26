using UnityEngine;

public class RangedEnemy : EnemyBase
{
    [Header("Ranged Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private WeaponType weaponType = WeaponType.Staff;

    protected override void Start()
    {
        base.Start();
        damageType = DamageType.Magical;
        attackRange = 8f;
        ScoreValue = 20;
        ApplyWeaponStats();
    }

    public void SetWeapon(WeaponType type)
    {
        weaponType = type;
        ApplyWeaponStats();
    }

    private void ApplyWeaponStats()
    {
        switch (weaponType)
        {
            case WeaponType.Staff:
                attackDamage = 8f; attackCooldown = 2.0f; break;
            case WeaponType.Wand:
                attackDamage = 6f; attackCooldown = 1.2f; break;
        }
    }

    public override void PerformAttack()
    {
        if (animator != null) animator.SetTrigger("Attack");

        if (projectilePrefab != null && shootPoint != null && player != null)
        {
            var go = Instantiate(projectilePrefab, shootPoint.position,
                Quaternion.LookRotation(player.position - shootPoint.position));
            var proj = go.GetComponent<Projectile>();
            proj?.Initialize(player.position, attackDamage, damageType);
        }
    }
}