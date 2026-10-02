using UnityEngine;
//база для всех персонажей (игрок, враг, босс)
public abstract class Character : MonoBehaviour
{
    [Header("Character Stats")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float runSpeed = 8f;
    [SerializeField] protected float rotationSpeed = 10f;
    
    protected HealthComponent healthComponent;
    protected CharacterController characterController;
    protected Animator animator;
    
    protected bool isAlive = true;

    private static readonly int HitHash   = Animator.StringToHash("Hit");
    private static readonly int DeathHash = Animator.StringToHash("Death");
    
    //вызывается при создании объекта
    protected virtual void Awake()
    {
        //нахождение здоровья, контролера и аниматора на объекте
        healthComponent = GetComponent<HealthComponent>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    
    //вызывается перед первым кадром
    protected virtual void Start()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDeath += HandleDeath;
            healthComponent.OnDamaged += HandleDamaged; //подписка на событие сметри и получение урона
        }
    }

    protected virtual void HandleDamaged(float amount)
    {
        if (animator != null && isAlive)
            animator.SetTrigger(HitHash);
    }
    
    //обработка смерти
    protected virtual void HandleDeath()
    {
        isAlive = false;
        if (animator != null)
        {
            animator.SetTrigger(DeathHash);
        }
    }
     //отписка от события
    protected virtual void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDeath -= HandleDeath;
            healthComponent.OnDamaged -= HandleDamaged;
        }
    }
}