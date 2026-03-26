using UnityEngine;

public class ChaseState : EnemyStateBase
{
    private Transform _playerTransform;

    private float _runSpeed;
    private float _gunRange;
    private float _setDestinationTimer;
    private float _findTimer;
    private bool _isFindingPlayer;

    private const float _findDuration = 3f;
    private const float _updateDestinationInterval = 1f;
    private const float _attackRangeMultiplier = 0.8f;

    public ChaseState(Enemy enemy, EnemyData enemyData, float gunRange) : base(enemy, enemyData)
    {
        _runSpeed = enemyData.RunSpeed;
        _gunRange = gunRange;
    }

    public override void Enter()
    {
        _findTimer = 0f;
        _setDestinationTimer = 0f;
        _playerTransform = _enemy.PlayerTransform;

        _enemy.Agent.isStopped = false;
        _enemy.Agent.speed = _runSpeed;
        MoveToLastSeenPlayerPosition();

        _enemy.SetAnimation(EnemyAnimParm.Run, true);
        _enemy.SetAnimation(EnemyAnimParm.ArmRaised, true);

        _enemy.ShowTargetingIcon(true);
    }

    public override void Update()
    {
        //float distanceToPlayer = Vector3.Distance(_playerTransform.position, _enemy.EnemyTransform.position);

        // (총 사거리 * 0.8) 안에 플레이어가 있다면 attack상태로 전환
        if (_enemy.GetDistanceToPlayer() <= _gunRange * _attackRangeMultiplier)
        {
            _enemy.ChangeState(EnemyState.Attack);
            return;
        }

        // 플레이어가 시야에 안보일 경우, 플레이어가 마지막 있던 자리로 목적지 설정
        if (!_enemy.IsDetectPlayer && !_isFindingPlayer)
        {
            FindPlayer(true);
            MoveToLastSeenPlayerPosition();
            return;
        }

        // 플레이어가 find Duration 동안 시야에 안보일 경우, Return상태로 전환
        if (_isFindingPlayer)
        {
            _findTimer += Time.deltaTime;
            if (_findTimer > _findDuration)
                _enemy.ChangeState(EnemyState.Return);

            return;
        }

        // 플레이어가 시야에 안보이다가 다시 보일 경우
        if(_enemy.IsDetectPlayer && _isFindingPlayer)
            FindPlayer(false);

        // interval 마다 플레이어가 있는 자리로 목적지 재설정
        _setDestinationTimer += Time.deltaTime;
        if(_setDestinationTimer >= _updateDestinationInterval)
        {
            MoveToLastSeenPlayerPosition();
            _setDestinationTimer = 0f;
        }
    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Walk, false);
        _enemy.SetAnimation(EnemyAnimParm.Run, false);
        _enemy.ShowTargetingIcon(false);
    }

    private void MoveToLastSeenPlayerPosition()
    {
        _enemy.Agent.SetDestination(_enemy.LastSeenPlayerPosition);
    }

    private void FindPlayer(bool isFinding)
    {
        _isFindingPlayer = isFinding;
        _enemy.SetAnimation(EnemyAnimParm.Walk, isFinding);
        _enemy.SetAnimation(EnemyAnimParm.Run, !isFinding);
    }
}