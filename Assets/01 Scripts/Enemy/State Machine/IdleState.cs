using UnityEngine;

public class IdleState : EnemyStateBase
{
    private float _timer;
    private const float _idleTime = 3f;

    public IdleState(Enemy enemy, EnemyData enemyData) : base(enemy, enemyData) { }

    public override void Enter()
    {
        _enemy.Agent.isStopped = true;
        _timer = 0;
    }

    public override void Update()
    {
        if (_enemy.IsDetectPlayer)
        {
            _enemy.ChangeState(EnemyState.Chase);
            return;
        }

        _timer += Time.deltaTime;
        if (_timer > _idleTime)
        {
            _enemy.ChangeState(EnemyState.Patrol);
            return;
        }
    }

    public override void Exit()
    {
        _enemy.Agent.isStopped = false;
    }

}
