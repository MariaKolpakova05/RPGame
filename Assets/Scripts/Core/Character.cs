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
            healthComponent.OnDeath += HandleDeath; //подписка на событие сметри
        }
    }
    
    //обработка смерти
    protected virtual void HandleDeath()
    {
        isAlive = false;
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }
     //отписка от события
    protected virtual void OnDestroy()
    {
        if (healthComponent != null)
        {
            healthComponent.OnDeath -= HandleDeath;
        }
    }
}