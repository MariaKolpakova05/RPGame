using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Attack")]
    [SerializeField] private float physicalDamage = 25f;
    [SerializeField] private float magicalDamage = 30f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float physicalCooldown = 0.5f;
    [SerializeField] private float magicCooldown = 2f;
    [SerializeField] private float magicManaCost = 20f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Refs")]
    [SerializeField] private PlayerView view;

    private PlayerModel model;
    private CharacterController characterController;
    private Camera mainCamera;
    private PlayerInput playerInput;
    private Vector3 moveDirection;
    

    public PlayerModel Model => model;
    public float CurrentHealth => model.CurrentHealth;
    public float MaxHealth => model.MaxHealth;
    public float CurrentMana => model.CurrentMana;
    public float MaxMana => model.MaxMana;
    public float TimeSinceLastMagic => model.TimeSinceLastMagic;
    public float MagicCooldownTime => model.MagicCooldownTime;

    // IDamageable
    public bool IsAlive => model.IsAlive;
    public Transform Transform => transform;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        model = new PlayerModel();
        model.PhysicalDamage = physicalDamage;
        model.MagicalDamage = magicalDamage;
        model.AttackRange = attackRange;
        model.PhysicalCooldown = physicalCooldown;
        model.MagicCooldown = magicCooldown;
        model.MagicManaCost = magicManaCost;

        model.OnDamaged += _ => view.PlayHit();
        model.OnDied += HandleDeath;

        playerInput = new PlayerInput();
        playerInput.Player.PhysicalAttack.performed += OnPhysicalAttack;
        playerInput.Player.MagicalAttack.performed += OnMagicAttack;
    }

    private void OnEnable() => playerInput.Enable();
    private void OnDisable() => playerInput.Disable();

    private void Update()
    {
        
        if (!model.IsAlive) return;

        model.RegenerateMana(Time.deltaTime);
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 input = playerInput.Player.Move.ReadValue<Vector2>();
        bool isRunning = playerInput.Player.Run.IsPressed();
        float speed = isRunning ? runSpeed : moveSpeed;

        Vector3 forward = mainCamera.transform.forward; forward.y = 0; forward.Normalize();
        Vector3 right = mainCamera.transform.right; right.y = 0; right.Normalize();
        moveDirection = (forward * input.y + right * input.x).normalized;

        bool isWalking = moveDirection.sqrMagnitude > 0.01f;
        if (isWalking)
        {
            characterController.Move(moveDirection * speed * Time.deltaTime);
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        float animSpeed = isWalking ? (isRunning ? 1f : 0.5f) : 0f;
        view.SetMovement(animSpeed, isWalking, isWalking && isRunning);

        
    }

    private void OnPhysicalAttack(InputAction.CallbackContext ctx)
    {
        if (!model.IsAlive) return;
        if (!model.CanPhysicalAttack(Time.time)) return;

        model.MarkPhysicalAttack(Time.time);
        view.PlayPhysicalAttack();

        Collider[] hits = Physics.OverlapSphere(
            transform.position + transform.forward * attackRange, attackRange, enemyLayer);

        foreach (var h in hits)
            h.GetComponent<IDamageable>()?.TakeDamage(model.PhysicalDamage, DamageType.Physical);

        if (ServiceLocator.TryGet<IAudioService>(out var audio))
            audio.PlayAttackSound();
    }

    private void OnMagicAttack(InputAction.CallbackContext ctx)
    {
        if (!model.IsAlive) return;
        if (!model.CanMagicAttack(Time.time)) return;
        if (!model.TrySpendMana(model.MagicManaCost)) return;

        model.MarkMagicAttack(Time.time);
        view.PlayMagicAttack();

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange * 2f, enemyLayer);
        foreach (var h in hits)
            h.GetComponent<IDamageable>()?.TakeDamage(model.MagicalDamage, DamageType.Magical);

        if (ServiceLocator.TryGet<IAudioService>(out var audio))
            audio.PlayMagicSound();
    }

    // IDamageable
    public void TakeDamage(float amount, DamageType type) => model.TakeDamage(amount);

    public void Heal(float amount) => model.Heal(amount);
    public void SetMana(float value) => model.SetMana(value);

    private void HandleDeath()
    {
        view.PlayDeath();
        GameEvents.PlayerDied?.Invoke();
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.Player.PhysicalAttack.performed -= OnPhysicalAttack;
            playerInput.Player.MagicalAttack.performed -= OnMagicAttack;
        }
    }
}