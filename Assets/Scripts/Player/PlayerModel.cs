using System;

public class PlayerModel
{
    // Здоровье
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }

    // Мана
    public float CurrentMana { get; private set; }
    public float MaxMana { get; private set; }
    public float ManaRegenPerSecond { get; set; } = 5f;

    // Кулдауны
    public float PhysicalCooldown { get; set; } = 0.5f;
    public float MagicCooldown { get; set; } = 2f;
    public float MagicManaCost { get; set; } = 20f;

    // Урон
    public float PhysicalDamage { get; set; } = 25f;
    public float MagicalDamage { get; set; } = 30f;
    public float AttackRange { get; set; } = 3f;

    // Состояние
    public bool IsAlive => CurrentHealth > 0;
    public float LastPhysicalAttackTime { get; private set; } = -999f;
    public float LastMagicAttackTime { get; private set; } = -999f;

    // События — View и UI подписываются
    public event Action<float, float> OnHealthChanged;   // current, max
    public event Action<float, float> OnManaChanged;
    public event Action OnDied;
    public event Action<float> OnDamaged;

    public PlayerModel(float maxHealth = 100f, float maxMana = 100f)
    {
        MaxHealth = maxHealth;
        MaxMana = maxMana;
        CurrentHealth = maxHealth;
        CurrentMana = maxMana;
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;
        CurrentHealth = Math.Max(0, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnDamaged?.Invoke(amount);
        if (!IsAlive) OnDied?.Invoke();
    }

    public void Heal(float amount)
    {
        if (!IsAlive) return;
        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void RegenerateMana(float deltaTime)
    {
        if (CurrentMana >= MaxMana) return;
        CurrentMana = Math.Min(MaxMana, CurrentMana + ManaRegenPerSecond * deltaTime);
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
    }

    public bool TrySpendMana(float amount)
    {
        if (CurrentMana < amount) return false;
        CurrentMana -= amount;
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
        return true;
    }

    public void SetMana(float value)
    {
        CurrentMana = Math.Clamp(value, 0, MaxMana);
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
    }

    public bool CanPhysicalAttack(float now) => now >= LastPhysicalAttackTime + PhysicalCooldown;
    public bool CanMagicAttack(float now) => now >= LastMagicAttackTime + MagicCooldown && CurrentMana >= MagicManaCost;

    public void MarkPhysicalAttack(float now) => LastPhysicalAttackTime = now;
    public void MarkMagicAttack(float now) => LastMagicAttackTime = now;

    public float TimeSinceLastMagic => UnityEngine.Time.time - LastMagicAttackTime;
    public float MagicCooldownTime => MagicCooldown;
}