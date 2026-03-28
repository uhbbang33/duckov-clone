using UnityEngine;

public class ChaseState : EnemyStateBase
{
    private float _runSpeed;
    private float _setDestinationTimer;
    private float _findTimer;
    private bool _isFindingPlayer;

    private const float _findDuration = 5f;
    private const float _updateDestinationInterval = 1f;

    public ChaseState(Enemy enemy) : base(enemy)
    {
        _runSpeed = _enemy.Data.RunSpeed;
    }

    public override void Enter()
    {
        _findTimer = 0f;
        _setDestinationTimer = 0f;
        _isFindingPlayer = false;

        _enemy.Agent.isStopped = false;
        _enemy.Agent.speed = _runSpeed;
        MoveToLastSeenPlayerPosition();

        _enemy.SetAnimation(EnemyAnimParm.Run, true);
        _enemy.SetAnimation(EnemyAnimParm.ArmRaised, true);

        _enemy.ShowTargetingIcon(true);
    }

    public override void Update()
    {
        // (총 사거리 * 0.8) 안에 플레이어가 있다면 attack상태로 전환
        if (_enemy.IsPlayerInAttackRange(-2f))
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

        // 플레이어가 시야에 안보이다가 다시 보일 경우
        if (_enemy.IsDetectPlayer && _isFindingPlayer)
        {
            _findTimer = 0f;
            FindPlayer(false);
        }

        if (_isFindingPlayer)
        {
            // 플레이어가 find Duration 동안 시야에 안보일 경우 spawn position으로
            _findTimer += Time.deltaTime;
            if (_findTimer > _findDuration)
                ReturnSpawnPoint();

            if (_enemy.Agent.remainingDistance <= _enemy.Agent.stoppingDistance)
                _enemy.SetAnimation(EnemyAnimParm.Walk, false);

            return;
        }

        // 스폰 지점과의 거리가 탐색 범위 이상이면 스폰 지점으로 복귀
        float distanceToSpawn = Vector3.Distance(_enemy.SpawnPosition, _enemy.gameObject.transform.position);
        if (distanceToSpawn > _enemy.Data.ChaseRange)
        {
            Debug.Log("GO SPAWN");
            ReturnSpawnPoint();
        }


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

    private void ReturnSpawnPoint()
    {
        _enemy.CurrentDestinationCount = 0;
        _enemy.ChangeState(EnemyState.Patrol);
    }
}