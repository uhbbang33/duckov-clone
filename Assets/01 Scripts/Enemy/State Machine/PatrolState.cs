using UnityEngine;
using UnityEngine.AI;

public class PatrolState : EnemyStateBase
{
    private Vector3 _spawnPosition;
    private float _patrolRange;
    private float _walkSpeed;
    private const float _minDistance = 2f;

    public PatrolState(Enemy enemy, Vector3 spawnPosition, float patrolRange, float walkSpeed) : base(enemy)
    {
        _spawnPosition = spawnPosition;
        _patrolRange = patrolRange;
        _walkSpeed = walkSpeed;
    }

    public override void Enter()
    {
        _enemy.Agent.speed = _walkSpeed;

        _enemy.SetAnimation(EnemyAnimParm.Walk, true);

        // 순찰 반경 내에서 랜덤한 목적지 설정
        SetRandomDestination();
    }

    public override void Update()
    {
        if (_enemy.IsDetectPlayer)
        {
            _enemy.ChangeState(EnemyState.Chase);
            return;
        }

        // 목적지에 도착하면 Idle 상태로 전환
        if(_enemy.Agent.remainingDistance <= _enemy.Agent.stoppingDistance)
            _enemy.ChangeState(EnemyState.Idle);
    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Walk, false);
    }

    private void SetRandomDestination()
    {
        Vector3 randomDestination = GetRandomPointOnNavMesh();
        _enemy.Agent.SetDestination(randomDestination);
    }

    private Vector3 GetRandomPointOnNavMesh()
    {
        int tryNum = 10;

        for (int i = 0; i < tryNum; ++i)
        {
            Vector3 randomPoint = _spawnPosition + Random.insideUnitSphere * _patrolRange;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                if (Vector3.Distance(_enemy.Agent.transform.position, hit.position) >= _minDistance)
                {
                    return hit.position;
                }
            }
        }

        return _spawnPosition;
    }
}