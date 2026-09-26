using UnityEngine;
using System;
//управление здоровьем для всех объектов(игрок, враги и босс)
public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f; //максимальное здоровье
    [SerializeField] private HealthBarUI healthBar; //ссылка на полоску здоровья 
    
    private float currentHealth; //текущее здоровье
    
    public event Action OnDeath; //событие смерти персонажа
    public event Action<float> OnHealthChanged; //событие изменения здоровья
    public event Action<float> OnDamaged;
    
    public Transform Transform => transform; //возвращает позицию объекта
    public bool IsAlive => currentHealth > 0; //объект жив если здоровье > 0
    
    private void Start()
    {
        currentHealth = maxHealth; //максимальное здоровье при старте
        UpdateHealthBar(); //обновление здоровья
    }
    
    public void TakeDamage(float amount, DamageType type)
    {
        if (!IsAlive) return; //нет удара если мертв
        
        currentHealth -= amount; 
        currentHealth = Mathf.Max(0, currentHealth); 
        
        OnHealthChanged?.Invoke(currentHealth / maxHealth); //оповещение для подписчиков этого события, что хп изменилось
        OnDamaged?.Invoke(amount);
        UpdateHealthBar(); 
        
        if (!IsAlive)
        {
            OnDeath?.Invoke(); //событие смерти если умерли
        }
    }
    
    public void Heal(float amount)
    {
        if (!IsAlive) return;
        
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth / maxHealth);
        UpdateHealthBar();
    }
    
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth / maxHealth);
        }
    }
    
    public float CurrentHealth => currentHealth; //для чтения из других скриптов
    public float MaxHealth => maxHealth;
}