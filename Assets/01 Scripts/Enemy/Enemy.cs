using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Animator _anim;
    private HealthPoint _hp;
    private EnemyData _enemyData;
    private NavMeshAgent _agent;

    private EnemyStateBase _currentState;
    private Dictionary<EnemyState, EnemyStateBase> _stateDictionary;


    public NavMeshAgent Agent { get { return _agent; } }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _hp = GetComponent<HealthPoint>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _enemyData = DataManager.Instance.GetEnemyData();

        _stateDictionary = new Dictionary<EnemyState, EnemyStateBase>
        {
            {EnemyState.Idle, new IdleState(this)},
            {EnemyState.Patrol, new PatrolState(this, transform.position, _enemyData.PatrolRange)},
            {EnemyState.Chase, new ChaseState(this)},
            {EnemyState.Return, new ReturnState(this)},
            {EnemyState.Attack, new AttackState(this)},
            {EnemyState.Death, new DeathState(this)}
        };

        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        _currentState.Update();
    }

    public void ChangeState(EnemyState newState)
    {
        _currentState?.Exit();
        _currentState = _stateDictionary[newState];
        _currentState.Enter();
    }

    public void SetAnimation(string param, bool value)
    {
        _anim.SetBool(param, value);
    }

    public bool DetectPlayer()
    {
        return false;
    }
}
