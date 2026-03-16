using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private UnityEvent onDamage;
    [SerializeField] private UnityEvent onDeath;
    
    private float currentHealth;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, DamageType damageType)
    {
        if (IsDead) return;
    
        currentHealth -= amount;
    
        // Получаем компонент Animator и отправляем сигнал
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("TakeDamage");
        }
    
        onDamage?.Invoke();
    
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (anim != null)
            {
                anim.SetTrigger("Die");
            }
            onDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
}