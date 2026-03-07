using UnityEngine;

public class IdleState : EnemyStateBase
{
    private float _timer;
    private const float _idleTime = 3f;

    public IdleState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        _enemy.Agent.enabled = false;
        _timer = 0;
    }

    public override void Update()
    {
        if (_enemy.IsDetectPlayer)
        {
            _enemy.ChangeState(EnemyState.Chase);
        }

        _timer += Time.deltaTime;
        if (_timer > _idleTime)
        {
            _enemy.ChangeState(EnemyState.Patrol);
        }
    }

    public override void Exit()
    {
        _enemy.Agent.enabled = true;
    }

}
