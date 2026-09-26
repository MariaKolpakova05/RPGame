using UnityEngine;
using UnityEngine.AI;

public enum ElementType { Fire, Ice, Earth, Ether }

public class BossEnemy : Character
{
    [Header("Boss Settings")]
    [SerializeField] private float chaseRange = 15f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float strongAttackCooldown = 4f;
    [SerializeField] private float baseDamage = 25f;
    [SerializeField] private WeaponType currentWeapon = WeaponType.BossSword;
    [SerializeField] private ElementType currentElement = ElementType.Fire;

    private NavMeshAgent navAgent;
    private Transform player;
    private HealthComponent bossHealth;

    public StateMachine StateMachine { get; private set; }
    public BossContext Context { get; private set; }

    public float AttackRange => attackRange;
    public float ChaseRange => chaseRange;
    public float LastStrongAttackTime { get; private set; }
    public float CurrentAttackCooldown => Context.IsPhase2 ? 0.8f : attackCooldown;
    public float CurrentStrongCooldown => Context.IsPhase2 ? 2.5f : strongAttackCooldown;

    public int ScoreValue { get; private set; } = 100;
    public bool HasBeenHit { get; private set; }
    public static bool PeacefulMode { get; set; } = false;

    private bool _phase2Triggered;

    protected override void Awake()
    {
        base.Awake();
        navAgent = GetComponent<NavMeshAgent>();
        bossHealth = GetComponent<HealthComponent>();
        StateMachine = new StateMachine();
    }

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        currentWeapon = (WeaponType)Random.Range((int)WeaponType.BossSword, (int)WeaponType.BossStaff + 1);
        currentElement = (ElementType)Random.Range(0, 4);
        UpdateElementVisuals();

        Context = new BossContext
        {
            Boss = this, Player = player,
            Agent = navAgent, Animator = animator, Health = bossHealth
        };

        if (bossHealth != null) bossHealth.OnDamaged += OnBossDamaged;

        StateMachine.Initialize(new BossIdleState(Context));
    }

    private void OnBossDamaged(float amount)
    {
        HasBeenHit = true;

        // Переход в фазу 2 при HP <= 50%
        if (!_phase2Triggered && bossHealth.CurrentHealth / bossHealth.MaxHealth <= 0.5f)
        {
            _phase2Triggered = true;
            StateMachine.ChangeState(new BossEnrageState(Context));
        }
    }

    private void Update()
    {
        if (!isAlive || player == null) return;
        StateMachine.Update();
    }

    //Фабричные методы для состояний — точка расширения
    public IState CreateChaseState() => Context.IsPhase2
        ? new BossPhase2ChaseState(Context)
        : (IState)new BossChaseState(Context);

    public IState CreateAttackState() => Context.IsPhase2
        ? new BossPhase2AttackState(Context)
        : (IState)new BossAttackState(Context);

    public IState CreateStrongAttackState() => Context.IsPhase2
        ? new BossPhase2StrongAttackState(Context)
        : (IState)new BossStrongAttackState(Context);

    //Действия
    public void FacePlayer()
    {
        if (player == null) return;
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    public void PerformNormalAttack()
    {
        if (animator) animator.SetTrigger("Attack");
        float damage = baseDamage * GetWeaponMultiplier() * GetElementMultiplier();

        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange * 1.5f)
            player.GetComponent<IDamageable>()?.TakeDamage(damage, DamageType.Physical);
    }

    public void PerformStrongAttack()
    {
        if (animator) animator.SetTrigger("StrongAttack");
        float damage = baseDamage * 2f * GetWeaponMultiplier() * GetElementMultiplier();

        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange * 2f)
            player.GetComponent<IDamageable>()?.TakeDamage(damage, DamageType.Magical);

        LastStrongAttackTime = Time.time;
    }

    public void Announce(string message) => Debug.Log($"Босс: {message}");

    private float GetWeaponMultiplier() => currentWeapon == WeaponType.BossSword ? 1.2f : 1.0f;

    private float GetElementMultiplier() => currentElement switch
    {
        ElementType.Fire => 1.3f,
        ElementType.Ice => 1.1f,
        ElementType.Earth => 1.2f,
        ElementType.Ether => 1.4f,
        _ => 1.0f
    };

    private void UpdateElementVisuals()
    {
        var rend = GetComponent<Renderer>();
        if (rend == null) return;
        rend.material.color = currentElement switch
        {
            ElementType.Fire => Color.red,
            ElementType.Ice => Color.blue,
            ElementType.Earth => Color.green,
            ElementType.Ether => Color.magenta,
            _ => Color.white
        };
    }

    protected override void HandleDeath()
    {
        base.HandleDeath();
        if (navAgent != null) navAgent.isStopped = true;
        if (bossHealth != null) bossHealth.OnDamaged -= OnBossDamaged;
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(ScoreValue);
        Destroy(gameObject, 3f);
    }
}