using UnityEngine;

public class PlayerCombat : MonoBehaviour, IDamageDealer
{
    [Header("Damage Stats")]
    [SerializeField] private float physicalDamage = 20f;
    [SerializeField] private float magicalDamage = 15f;
    [SerializeField] private float magicCooldown = 2f;
    [SerializeField] private float attackRange = 2f;
    
    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private GameObject magicProjectilePrefab;
    [SerializeField] private Transform magicSpawnPoint;
    
    private Animator animator;
    private float lastMagicTime;
    private bool canUseMagic => Time.time >= lastMagicTime + magicCooldown;
    
    public float PhysicalDamage => physicalDamage;
    public float MagicalDamage => magicalDamage;
    public DamageType DamageType { get; private set; }
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Физическая атака (левая кнопка мыши)
        if (Input.GetMouseButtonDown(0))
        {
            PerformPhysicalAttack();
        }
        
        // Магическая атака (правая кнопка мыши)
        if (Input.GetMouseButtonDown(1) && canUseMagic)
        {
            PerformMagicAttack();
        }
    }
    
    void PerformPhysicalAttack()
    {
        animator.SetTrigger("PhysicalAttack");
        DamageType = DamageType.Physical;
        
        // Проверка попадания
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);
        
        foreach (Collider enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                DealDamage(damageable);
            }
        }
    }
    
    void PerformMagicAttack()
    {
        animator.SetTrigger("MagicAttack");
        lastMagicTime = Time.time;
        
        // Создание магического снаряда
        if (magicProjectilePrefab != null && magicSpawnPoint != null)
        {
            GameObject projectile = Instantiate(magicProjectilePrefab, 
                magicSpawnPoint.position, Quaternion.identity);
            
            MagicProjectile magicScript = projectile.GetComponent<MagicProjectile>();
            if (magicScript != null)
            {
                magicScript.Initialize(magicalDamage, transform.forward);
            }
        }
    }
    
    public void DealDamage(IDamageable target)
    {
        float damage = DamageType == DamageType.Physical ? physicalDamage : magicalDamage;
        target.TakeDamage(damage, DamageType);
    }
    
    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
