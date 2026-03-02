using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyStateBase _currentState;
    private Dictionary<EnemyState, EnemyStateBase> _stateDictionary;

    private void Start()
    {
        _stateDictionary = new Dictionary<EnemyState, EnemyStateBase>
        {
            {EnemyState.Idle, new IdleState(this)},
            {EnemyState.Patrol, new PatrolState(this)},
            {EnemyState.Chase, new ChaseState(this)},
            {EnemyState.Return, new ReturnState(this)},
            {EnemyState.Attack, new AttackState(this)},
            {EnemyState.Dead, new DeadState(this)}
        };
    }

    private void Update()
    {
        _currentState.Update();
    }
   
    private void ChangeState(EnemyState newState)
    {
        _currentState?.Exit();
        _currentState = _stateDictionary[newState];
        _currentState.Enter();
    }
}
