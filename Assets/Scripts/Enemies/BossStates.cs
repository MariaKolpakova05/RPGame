using UnityEngine;
using UnityEngine.AI;

public class BossContext
{
    public BossEnemy Boss;
    public Transform Player;
    public NavMeshAgent Agent;
    public Animator Animator;
    public HealthComponent Health;

    public float HealthPercent => Health.CurrentHealth / Health.MaxHealth;
    public bool IsPhase2 => HealthPercent <= 0.5f;
}


//IDLE
public class BossIdleState : IState
{
    protected readonly BossContext _ctx;
    public BossIdleState(BossContext ctx) { _ctx = ctx; }

    public virtual void Enter()
    {
        if (_ctx.Agent) _ctx.Agent.isStopped = true;
        if (_ctx.Animator) _ctx.Animator.SetFloat("Speed", 0);
    }
    public virtual void Exit() { }

    public virtual void LogicUpdate()
    {
        if (_ctx.Player == null) return;
        if (BossEnemy.PeacefulMode && !_ctx.Boss.HasBeenHit) return;

        float dist = Vector3.Distance(_ctx.Boss.transform.position, _ctx.Player.position);
        if (dist < _ctx.Boss.ChaseRange)
            _ctx.Boss.StateMachine.ChangeState(_ctx.Boss.CreateChaseState());
    }
    public virtual void PhysicsUpdate() { }
}

//CHASE
public class BossChaseState : IState
{
    protected readonly BossContext _ctx;
    public BossChaseState(BossContext ctx) { _ctx = ctx; }

    public virtual void Enter()
    {
        if (_ctx.Agent) _ctx.Agent.isStopped = false;
        if (_ctx.Animator) _ctx.Animator.SetFloat("Speed", 1);
    }
    public virtual void Exit() { }

    public virtual void LogicUpdate()
    {
        if (_ctx.Player == null) return;
        if (_ctx.Agent) _ctx.Agent.SetDestination(_ctx.Player.position);

        float dist = Vector3.Distance(_ctx.Boss.transform.position, _ctx.Player.position);
        if (dist <= _ctx.Boss.AttackRange)
            _ctx.Boss.StateMachine.ChangeState(_ctx.Boss.CreateAttackState());
    }
    public virtual void PhysicsUpdate() { }
}

//ATTACK
public class BossAttackState : IState
{
    protected readonly BossContext _ctx;
    protected float _lastAttack;

    public BossAttackState(BossContext ctx) { _ctx = ctx; }

    public virtual void Enter() { if (_ctx.Agent) _ctx.Agent.isStopped = true; }
    public virtual void Exit() { }

    public virtual void LogicUpdate()
    {
        if (_ctx.Player == null) return;
        _ctx.Boss.FacePlayer();

        float dist = Vector3.Distance(_ctx.Boss.transform.position, _ctx.Player.position);
        if (dist > _ctx.Boss.AttackRange * 1.2f)
        {
            _ctx.Boss.StateMachine.ChangeState(_ctx.Boss.CreateChaseState());
            return;
        }

        if (Time.time > _lastAttack + _ctx.Boss.CurrentAttackCooldown)
        {
            _ctx.Boss.PerformNormalAttack();
            _lastAttack = Time.time;
        }

        if (Random.value < 0.3f && Time.time > _ctx.Boss.LastStrongAttackTime + _ctx.Boss.CurrentStrongCooldown)
            _ctx.Boss.StateMachine.ChangeState(_ctx.Boss.CreateStrongAttackState());
    }
    public virtual void PhysicsUpdate() { }
}

//STRONG ATTACK
public class BossStrongAttackState : IState
{
    protected readonly BossContext _ctx;
    public BossStrongAttackState(BossContext ctx) { _ctx = ctx; }

    public virtual void Enter()
    {
        if (_ctx.Agent) _ctx.Agent.isStopped = true;
        _ctx.Boss.FacePlayer();
        _ctx.Boss.PerformStrongAttack();
    }
    public virtual void Exit() { }

    public virtual void LogicUpdate()
    {
        _ctx.Boss.StateMachine.ChangeState(_ctx.Boss.CreateAttackState());
    }
    public virtual void PhysicsUpdate() { }
}

//ФАЗА 2: IDLE
public class BossPhase2IdleState : BossIdleState
{
    public BossPhase2IdleState(BossContext ctx) : base(ctx) { }

    public override void Enter()
    {
        base.Enter();
        _ctx.Boss.Announce("Ты ещё жив? Похвально...");
    }

    public override void LogicUpdate()
    {
        if (_ctx.Player == null) return;
        if (BossEnemy.PeacefulMode && !_ctx.Boss.HasBeenHit) return;

        // в фазе 2 радиус агрессии больше
        if (Vector3.Distance(_ctx.Boss.transform.position, _ctx.Player.position) < 25f)
            _ctx.Boss.StateMachine.ChangeState(_ctx.Boss.CreateChaseState());
    }
}

//ФАЗА 2: CHASE
public class BossPhase2ChaseState : BossChaseState
{
    public BossPhase2ChaseState(BossContext ctx) : base(ctx) { }

    public override void Enter()
    {
        base.Enter();
        if (_ctx.Agent) _ctx.Agent.speed *= 1.5f;  // ускоряется
    }

    public override void Exit()
    {
        if (_ctx.Agent) _ctx.Agent.speed /= 1.5f;
    }
}

//ФАЗА 2: ATTACK
public class BossPhase2AttackState : BossAttackState
{
    public BossPhase2AttackState(BossContext ctx) : base(ctx) { }

    public override void LogicUpdate()
    {
        if (_ctx.Player == null) return;
        _ctx.Boss.FacePlayer();

        float dist = Vector3.Distance(_ctx.Boss.transform.position, _ctx.Player.position);
        if (dist > _ctx.Boss.AttackRange * 1.2f)
        {
            _ctx.Boss.StateMachine.ChangeState(_ctx.Boss.CreateChaseState());
            return;
        }

        // в фазе 2 атакует быстрее
        if (Time.time > _lastAttack + _ctx.Boss.CurrentAttackCooldown)
        {
            _ctx.Boss.PerformNormalAttack();
            _lastAttack = Time.time;
        }

        // чаще использует сильную атаку
        if (Random.value < 0.5f && Time.time > _ctx.Boss.LastStrongAttackTime + _ctx.Boss.CurrentStrongCooldown)
            _ctx.Boss.StateMachine.ChangeState(_ctx.Boss.CreateStrongAttackState());
    }
}

//ФАЗА 2: STRONG ATTACK
public class BossPhase2StrongAttackState : BossStrongAttackState
{
    public BossPhase2StrongAttackState(BossContext ctx) : base(ctx) { }

    public override void Enter()
    {
        base.Enter();
        _ctx.Boss.Announce("Я покажу тебе настоящую силу!");
    }
}

//ENRAGE (переход в фазу 2)
public class BossEnrageState : IState
{
    private readonly BossContext _ctx;
    private float _startTime;

    public BossEnrageState(BossContext ctx) { _ctx = ctx; }

    public void Enter()
    {
        _startTime = Time.time;
        if (_ctx.Agent) _ctx.Agent.isStopped = true;
        if (_ctx.Animator) _ctx.Animator.SetTrigger("Enrage");
        _ctx.Boss.Announce("ХВАТИТ! Сейчас ты умрёшь!");
    }

    public void Exit() { }

    public void LogicUpdate()
    {
        // 2 секунды "ярости" — потом в фазу 2
        if (Time.time > _startTime + 2f)
            _ctx.Boss.StateMachine.ChangeState(new BossPhase2IdleState(_ctx));
    }

    public void PhysicsUpdate() { }
}