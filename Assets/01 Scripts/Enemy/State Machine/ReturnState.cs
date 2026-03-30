
using UnityEngine;

public class ReturnState : EnemyStateBase
{
    private float _stateLockTimer;

    private const float _stateLockDuration = 3f;

    public ReturnState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        _stateLockTimer = 0f;

        _enemy.CurrentDestinationCount = 0;

        _enemy.Agent.isStopped = false;
        _enemy.Agent.speed = _enemy.Data.WalkSpeed;
        _enemy.Agent.SetDestination(_enemy.SpawnPosition);

        _enemy.SetAnimation(EnemyAnimParm.ArmRaised, false);
        _enemy.SetAnimation(EnemyAnimParm.Walk, true);
    }

    public override void Update()
    {
        if (_enemy.Agent.remainingDistance <= _enemy.Agent.stoppingDistance)
            _enemy.ChangeState(EnemyState.Idle);

        _stateLockTimer += Time.deltaTime;
        if (_stateLockTimer <= _stateLockDuration)
            return;

        if (_enemy.IsPlayerInSight || _enemy.IsNoiseHeard)
        {
            _enemy.ChangeState(EnemyState.Chase);
        }
    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Walk, false);
    }
}
