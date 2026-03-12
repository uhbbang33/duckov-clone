using UnityEngine;

public class ReturnState : EnemyStateBase
{
    private Vector3 _spawnPosition;
    private float _walkSpeed;

    public ReturnState(Enemy enemy, EnemyData enemyData, Vector3 spawnPosition, float walkSpeed) : base(enemy, enemyData)
    {
        _spawnPosition = spawnPosition;
        _walkSpeed = walkSpeed;
    }

    public override void Enter()
    {
        _enemy.Agent.isStopped = false;
        _enemy.Agent.speed = _walkSpeed;
        _enemy.SetAnimation(EnemyAnimParm.Walk, true);
        _enemy.Agent.SetDestination(_spawnPosition);
    }

    public override void Update()
    {
        if (_enemy.IsDetectPlayer)
        {
            _enemy.ChangeState(EnemyState.Chase);
            return;
        }

        if (_enemy.Agent.remainingDistance <= _enemy.Agent.stoppingDistance)
        {
            _enemy.HealMaxHP();
            _enemy.ChangeState(EnemyState.Idle);
            return;
        }
    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Walk, false);
    }

}