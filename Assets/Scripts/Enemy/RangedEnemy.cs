using UnityEngine;

public class RangedEnemy : BaseEnemy
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 10f;
    
    protected override void Start()
    {
        base.Start();
        DamageType = DamageType.Magical;
        attackRange = maxDistance;
    }
    
    protected override void Update()
    {
        if (IsDead || player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            // Держим дистанцию
            if (distanceToPlayer < minDistance)
            {
                // Отходим назад
                Vector3 directionToPlayer = (transform.position - player.position).normalized;
                Vector3 movePosition = transform.position + directionToPlayer * 2f;
                agent.SetDestination(movePosition);
            }
            else if (distanceToPlayer > maxDistance)
            {
                // Подходим ближе
                agent.SetDestination(player.position);
            }
            else
            {
                // Оптимальная дистанция - стоим и стреляем
                agent.ResetPath();
                
                // Поворачиваемся к игроку
                Vector3 lookDirection = (player.position - transform.position).normalized;
                lookDirection.y = 0;
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
            
            if (animator != null)
                animator.SetBool("IsWalking", agent.hasPath);
            
            // Атака
            if (distanceToPlayer <= attackRange && CanAttack())
            {
                Attack();
            }
        }
        else
        {
            agent.ResetPath();
            if (animator != null)
                animator.SetBool("IsWalking", false);
        }
    }
    
    protected override void Attack()
    {
        lastAttackTime = Time.time;
        
        if (animator != null)
            animator.SetTrigger("Attack");
    }
    
    // Вызывается из анимации атаки
    public void ShootProjectile()
    {
        if (projectilePrefab != null && shootPoint != null && player != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            
            // Направление на игрока
            Vector3 direction = (player.position - shootPoint.position).normalized;
            
            MagicProjectile magicScript = projectile.GetComponent<MagicProjectile>();
            if (magicScript != null)
            {
                magicScript.Initialize(magicalDamage, direction);
            }
            
            // Игнорируем коллизию с самим врагом
            Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}