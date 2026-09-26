using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    [Header("Melee Settings")]
    [SerializeField] private WeaponType weaponType = WeaponType.Sword;

    protected override void Start()
    {
        base.Start();
        damageType = DamageType.Physical;
        ScoreValue = 15;
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
            case WeaponType.Sword:
                attackDamage = 10f; attackCooldown = 1.0f; break;
            case WeaponType.Axe:
                attackDamage = 15f; attackCooldown = 1.5f; break;
        }
    }

    public override void PerformAttack()
    {
        if (animator != null) animator.SetTrigger("Attack");

        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange * 1.2f)
        {
            var dmg = player.GetComponent<IDamageable>();
            dmg?.TakeDamage(attackDamage, damageType);
        }
    }
}