using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    [SerializeField] private Transform attackPoint;
    
    protected override void Start()
    {
        base.Start();
        DamageType = DamageType.Physical;
        attackRange = 2f; // Ближний бой
    }
    
    protected override void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        if (animator != null)
            animator.SetTrigger("Attack");
        
        // Проверка попадания через анимацию (вызовется через Animation Event)
    }
    
    // Вызывается из анимации атаки
    public void PerformHit()
    {
        Collider[] hitPlayer = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayer);
        
        foreach (Collider playerCol in hitPlayer)
        {
            IDamageable damageable = playerCol.GetComponent<IDamageable>();
            if (damageable != null)
            {
                DealDamage(damageable);
            }
        }
        
        isAttacking = false;
    }
    
    // Вызывается в конце анимации атаки
    public void AttackFinished()
    {
        isAttacking = false;
    }
}
