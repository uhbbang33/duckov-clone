using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyStateBase
{
    private float _walkSpeed;
    private List<Vector3> _destinationList;

    public PatrolState(Enemy enemy, List<Vector3> destinationList) : base(enemy)
    {
        _walkSpeed = _enemy.Data.WalkSpeed;
        _destinationList = new List<Vector3>();
        _destinationList = destinationList;
    }

    public override void Enter()
    {
        _enemy.Agent.isStopped = false;
        _enemy.Agent.speed = _walkSpeed;

        _enemy.SetAnimation(EnemyAnimParm.Walk, true);
        _enemy.SetAnimation(EnemyAnimParm.ArmRaised, false);

        SetDestination();

        ++_enemy.CurrentDestinationCount;
        if (_enemy.CurrentDestinationCount >= _destinationList.Count)
            _enemy.CurrentDestinationCount = 0;
    }

    public override void Update()
    {
        if (_enemy.IsDetectPlayer)
        {
            _enemy.ChangeState(EnemyState.Chase);
            return;
        }

        // 목적지에 도착하면 Idle 상태로 전환
        if (_enemy.Agent.remainingDistance <= _enemy.Agent.stoppingDistance)
        {
            _enemy.ChangeState(EnemyState.Idle);
            return;
        }
    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Walk, false);
    }

    private void SetDestination()
    {
        _enemy.Agent.SetDestination(_destinationList[_enemy.CurrentDestinationCount]);
    }
}