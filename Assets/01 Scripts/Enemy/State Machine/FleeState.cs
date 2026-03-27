using UnityEngine;

public class FleeState : EnemyStateBase
{
    private Vector3 _spawnPosition;
    private float _runSpeed;

    public FleeState(Enemy enemy, EnemyData enemyData, Vector3 spawnPosition ) : base(enemy, enemyData)
    {
        _spawnPosition = spawnPosition;
        _runSpeed = enemyData.RunSpeed;
    }

    public override void Enter()
    {
        _enemy.Agent.isStopped = false;
        _enemy.Agent.speed = _runSpeed;
        _enemy.Agent.SetDestination(_spawnPosition);

        _enemy.SetAnimation(EnemyAnimParm.ArmRaised, false);
        _enemy.SetAnimation(EnemyAnimParm.Run, true);
    }

    public override void Update()
    {
        if (_enemy.Agent.remainingDistance <= _enemy.Agent.stoppingDistance)
            _enemy.ChangeState(EnemyState.Idle);
    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Run, false);
    }

}
