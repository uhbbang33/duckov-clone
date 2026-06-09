using UnityEngine;

public class IdleState : EnemyStateBase
{
    private float _timer;
    private float _healTimer;
    private const float _idleTime = 3f;
    private const float _healPerTickAmount = 1f;
    private const float _healDelay = 0.03f;

    public IdleState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        _enemy.Agent.isStopped = true;
        _enemy.SetAnimation(EnemyAnimParm.Run, false);
        _enemy.SetAnimation(EnemyAnimParm.Walk, false);
        _enemy.SetAnimation(EnemyAnimParm.ArmRaised, false);
        _timer = 0;
        _healTimer = 0;
        _enemy.StopFootStepSound();
    }

    public override void Update()
    {
        if (_enemy.IsPlayerInSight || _enemy.IsNoiseHeard)
        {
            _enemy.ChangeState(EnemyState.Chase);
            return;
        }

        if (_enemy.HP.CurrentHP < _enemy.HP.MaxHP)
        {
            _healTimer += Time.deltaTime;
            if (_healTimer >= _healDelay)
            {
                _enemy.HealHP(_healPerTickAmount);
                _healTimer = 0f;
            }
            return;
        }

        _timer += Time.deltaTime;
        if (_timer >= _idleTime)
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
