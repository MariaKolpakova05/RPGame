using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : Character
{
    [Header("Enemy Settings")]
    [SerializeField] protected float chaseRange = 10f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackCooldown = 1.5f;
    [SerializeField] protected float fleeHealthThreshold = 0.3f;
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected DamageType damageType = DamageType.Physical;

    protected NavMeshAgent navAgent;
    protected Transform player;
    protected float lastAttackTime;

    public StateMachine StateMachine { get; private set; }
    protected EnemyContext Context { get; private set; }

    public float ChaseRange => chaseRange;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;

    public int ScoreValue { get; protected set; } = 10;

    public static bool PeacefulMode { get; set; } = false;

    protected override void Awake()
    {
        base.Awake();
        navAgent = GetComponent<NavMeshAgent>();
        StateMachine = new StateMachine();
    }

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ScoreValue = 10;

        Context = new EnemyContext
        {
            Enemy = this,
            Player = player,
            Agent = navAgent,
            Animator = animator,
            Health = healthComponent,
            IsPeaceful = PeacefulMode
        };

        StateMachine.Initialize(new IdleState(Context, chaseRange));
    }

    protected virtual void Update()
    {
        if (!isAlive || player == null) return;

        Context.IsPeaceful = PeacefulMode;
        StateMachine.Update();

        float hp = healthComponent.CurrentHealth / healthComponent.MaxHealth;
        if (hp < fleeHealthThreshold && StateMachine.CurrentState is not FleeState)
            StateMachine.ChangeState(new FleeState(Context));
    }

    public abstract void PerformAttack();

    protected override void HandleDeath()
    {
        base.HandleDeath();
        if (navAgent != null) navAgent.isStopped = true;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(ScoreValue);
            ScoreManager.Instance.OnEnemyKilled();
        }
        if (animator != null) animator.SetTrigger("Death");

        Destroy(gameObject, 2f);
    }
}