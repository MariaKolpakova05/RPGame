using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Effects")]
    [SerializeField] private GameObject physicalAttackEffect;
    [SerializeField] private GameObject magicalAttackEffect;
    [SerializeField] private GameObject hitEffect;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsRunningHash    = Animator.StringToHash("isRunning");
    private static readonly int PhysicalAttackHash = Animator.StringToHash("PhysicalAttack");
    private static readonly int MagicAttackHash = Animator.StringToHash("MagicalAttack");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int DeathHash = Animator.StringToHash("Death");

    public void SetMovement(float speed, bool isWalking, bool isRunning)
    {
        if (animator == null) return;
        animator.SetFloat(SpeedHash, speed);
        animator.SetBool(IsWalkingHash, isWalking);
        animator.SetBool(IsRunningHash, isRunning);
    }

    public void PlayPhysicalAttack()
    {
        animator?.SetTrigger(PhysicalAttackHash);
        if (physicalAttackEffect != null)
            Instantiate(physicalAttackEffect, transform.position + transform.forward * 1.5f + Vector3.up, Quaternion.identity);
    }

    public void PlayMagicAttack()
    {
        animator?.SetTrigger(MagicAttackHash);
        if (magicalAttackEffect != null)
            Instantiate(magicalAttackEffect, transform.position + Vector3.up * 2f, Quaternion.identity);
    }

    public void PlayHit()
    {
        animator?.SetTrigger(HitHash);
        if (hitEffect != null)
            Instantiate(hitEffect, transform.position + Vector3.up, Quaternion.identity);
    }

    public void PlayDeath() => animator?.SetTrigger(DeathHash);
}