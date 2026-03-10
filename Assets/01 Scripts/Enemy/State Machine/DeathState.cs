
using UnityEngine;

public class DeathState : EnemyStateBase
{
    public DeathState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        _enemy.EnemyDeath();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }

}