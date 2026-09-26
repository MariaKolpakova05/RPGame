using UnityEngine;
using UnityEngine.AI;

public class EnemyContext
{
    public EnemyBase Enemy;
    public Transform Player;
    public NavMeshAgent Agent;
    public Animator Animator;
    public HealthComponent Health;
    public bool IsPeaceful;

    public bool IsPlayerInRange(float range)
    {
        if (Player == null) return false;
        return Vector3.Distance(Enemy.transform.position, Player.position) <= range;
    }

    public float HealthPercent => Health.CurrentHealth / Health.MaxHealth;
}

public class IdleState : IState
{
    private readonly EnemyContext _ctx;
    private readonly float _chaseRange;

    public IdleState(EnemyContext ctx, float chaseRange)
    {
        _ctx = ctx;
        _chaseRange = chaseRange;
    }

    public void Enter()
    {
        if (_ctx.Agent != null) _ctx.Agent.isStopped = true;
        if (_ctx.Animator != null) _ctx.Animator.SetFloat("Speed", 0);
    }

    public void Exit() { }

    public void LogicUpdate()
    {
        if (_ctx.IsPeaceful) return;
        if (_ctx.IsPlayerInRange(_chaseRange))
            _ctx.Enemy.StateMachine.ChangeState(new ChaseState(_ctx, _chaseRange));
    }

    public void PhysicsUpdate() { }
}

public class ChaseState : IState
{
    private readonly EnemyContext _ctx;
    private readonly float _chaseRange;

    public ChaseState(EnemyContext ctx, float chaseRange)
    {
        _ctx = ctx;
        _chaseRange = chaseRange;
    }

    public void Enter()
    {
        if (_ctx.Agent != null) _ctx.Agent.isStopped = false;
        if (_ctx.Animator != null) _ctx.Animator.SetFloat("Speed", 1);
    }

    public void Exit() { }

    public void LogicUpdate()
    {
        if (_ctx.Player == null) return;

        if (_ctx.Agent != null)
            _ctx.Agent.SetDestination(_ctx.Player.position);

        float dist = Vector3.Distance(_ctx.Enemy.transform.position, _ctx.Player.position);

        if (dist > _chaseRange)
        {
            _ctx.Enemy.StateMachine.ChangeState(new IdleState(_ctx, _chaseRange));
            return;
        }

        if (dist <= _ctx.Enemy.AttackRange)
            _ctx.Enemy.StateMachine.ChangeState(new AttackState(_ctx));
    }

    public void PhysicsUpdate() { }
}

public class AttackState : IState
{
    private readonly EnemyContext _ctx;
    private float _lastAttackTime;

    public AttackState(EnemyContext ctx) { _ctx = ctx; }

    public void Enter()
    {
        if (_ctx.Agent != null) _ctx.Agent.isStopped = true;
    }

    public void Exit() { }

    public void LogicUpdate()
    {
        if (_ctx.Player == null) return;

        Vector3 dir = (_ctx.Player.position - _ctx.Enemy.transform.position).normalized;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
            var targetRot = Quaternion.LookRotation(dir);
            _ctx.Enemy.transform.rotation = Quaternion.Slerp(
                _ctx.Enemy.transform.rotation, targetRot, 10f * Time.deltaTime);
        }

        float dist = Vector3.Distance(_ctx.Enemy.transform.position, _ctx.Player.position);

        if (dist > _ctx.Enemy.AttackRange * 1.1f)
        {
            _ctx.Enemy.StateMachine.ChangeState(new ChaseState(_ctx, _ctx.Enemy.ChaseRange));
            return;
        }

        if (Time.time > _lastAttackTime + _ctx.Enemy.AttackCooldown)
        {
            _ctx.Enemy.PerformAttack();
            _lastAttackTime = Time.time;
        }
    }

    public void PhysicsUpdate() { }
}

public class FleeState : IState
{
    private readonly EnemyContext _ctx;
    private float _lastFleeDecision;

    public FleeState(EnemyContext ctx) { _ctx = ctx; }

    public void Enter()
    {
        if (_ctx.Agent != null) _ctx.Agent.isStopped = false;
    }

    public void Exit() { }

    public void LogicUpdate()
    {
        if (_ctx.Player == null) return;

        if (Time.time > _lastFleeDecision + 0.5f)
        {
            Vector3 away = (_ctx.Enemy.transform.position - _ctx.Player.position).normalized;
            Vector3 target = _ctx.Enemy.transform.position + away * 10f;
            if (_ctx.Agent != null) _ctx.Agent.SetDestination(target);
            _lastFleeDecision = Time.time;
        }

        if (_ctx.HealthPercent > 0.5f)
            _ctx.Enemy.StateMachine.ChangeState(new IdleState(_ctx, _ctx.Enemy.ChaseRange));
    }

    public void PhysicsUpdate() { }
}