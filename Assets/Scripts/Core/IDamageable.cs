using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount, DamageType damageType);
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsDead { get; }
}
