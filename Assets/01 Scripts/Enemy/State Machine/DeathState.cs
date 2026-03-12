
using UnityEngine;

public class DeathState : EnemyStateBase
{
    public DeathState(Enemy enemy, EnemyData enemyData) : base(enemy, enemyData) { }

    public override void Enter()
    {
        _enemy.Agent.isStopped = true;
        _enemy.EnemyDeath();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }

}