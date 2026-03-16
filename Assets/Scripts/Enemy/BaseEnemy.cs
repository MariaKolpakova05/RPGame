using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IDamageDealer
{
    [Header("Base Stats")]
    [SerializeField] protected float maxHealth = 50f;
    [SerializeField] protected float physicalDamage = 10f;
    [SerializeField] protected float magicalDamage = 10f;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackCooldown = 1f;
    
    [Header("References")]
    [SerializeField] protected Transform player;
    [SerializeField] protected LayerMask playerLayer;
    
    protected NavMeshAgent agent;
    protected Animator animator;
    protected HealthSystem healthSystem;
    protected float lastAttackTime;
    protected bool isAttacking;
    
    // IDamageable implementation
    public float CurrentHealth => healthSystem.CurrentHealth;
    public float MaxHealth => healthSystem.MaxHealth;
    public bool IsDead => healthSystem.IsDead;
    
    // IDamageDealer implementation
    public float PhysicalDamage => physicalDamage;
    public float MagicalDamage => magicalDamage;
    public DamageType DamageType { get; protected set; }
    
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        
        // Поиск игрока, если не назначен
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        
        // Подписка на события здоровья
        if (healthSystem != null)
        {
            // Используем UnityEvents из HealthSystem
            var deathEvent = new UnityEngine.Events.UnityEvent();
            deathEvent.AddListener(Die);
        }
    }
    
    protected virtual void Update()
    {
        if (IsDead || player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            // Преследование игрока
            agent.SetDestination(player.position);
            
            if (animator != null)
                animator.SetBool("IsWalking", true);
            
            // Проверка на атаку
            if (distanceToPlayer <= attackRange && CanAttack())
            {
                Attack();
            }
        }
        else
        {
            // Бездействие
            agent.ResetPath();
            if (animator != null)
                animator.SetBool("IsWalking", false);
        }
    }
    
    protected virtual bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown && !isAttacking;
    }
    
    protected abstract void Attack();
    
    public virtual void DealDamage(IDamageable target)
    {
        float damage = DamageType == DamageType.Physical ? physicalDamage : magicalDamage;
        target.TakeDamage(damage, DamageType);
    }
    
    public void TakeDamage(float amount, DamageType damageType)
    {
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(amount, damageType);
            
            if (animator != null)
                animator.SetTrigger("TakeDamage");
        }
    }
    
    protected virtual void Die()
    {
        if (animator != null)
            animator.SetTrigger("Die");
        
        if (agent != null)
            agent.enabled = false;
        
        // Отключение коллайдера
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
        
        // Уничтожение через 3 секунды
        Destroy(gameObject, 3f);
    }
}
