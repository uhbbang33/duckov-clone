using UnityEngine;

public class ChaseState : EnemyStateBase
{
    private Transform _playerTransform;

    private float _runSpeed;
    private float _gunRange;
    private float _timer;

    private const float _updateInterval = 0.1f;

    public ChaseState(Enemy enemy, float runSpeed) : base(enemy)
    {
        _runSpeed = runSpeed;
    }

    public override void Enter()
    {
        _enemy.Agent.speed = _runSpeed;
        _enemy.SetAnimation(EnemyAnimParm.Run, true);
        _playerTransform = _enemy.PlayerTransform;
    }

    public override void Update()
    {
        float distanceToPlayer = Vector3.Distance(_playerTransform.position, _enemy.EnemyTransform.position);

        if (distanceToPlayer < _gunRange) {
            _enemy.ChangeState(EnemyState.Attack);
            return;
        }

        if (!_enemy.DetectPlayer())
        {
            _enemy.ChangeState(EnemyState.Return);
            return;
        }

        _timer += Time.deltaTime;
        if(_timer >= _updateInterval)
        {
            _timer = 0f;
            _enemy.Agent.SetDestination(_playerTransform.position);
        }
    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Run, false);
    }

}