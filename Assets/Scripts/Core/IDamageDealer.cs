using UnityEngine;

public interface IDamageDealer
{
    float PhysicalDamage { get; }
    float MagicalDamage { get; }
    DamageType DamageType { get; }
    void DealDamage(IDamageable target);
}
