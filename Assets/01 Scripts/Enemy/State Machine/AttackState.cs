
using UnityEngine;

public class AttackState : EnemyStateBase
{
    private float _gunRange;

    public AttackState(Enemy enemy, float gunRange) : base(enemy)
    {
        _gunRange = gunRange;
    }

    public override void Enter()
    {
        _enemy.Agent.enabled = false;
        _enemy.SetAnimation(EnemyAnimParm.Attack, true);
        // »ìÂ¦ ±â´Ù·È´Ù°¡ Attack  (Animation Event)
    }

    public override void Update()
    {
        if (_gunRange < _enemy.GetDistanceToPlayer())
            _enemy.ChangeState(EnemyState.Return);

        Vector3 lookDir = _enemy.GetDirectionToPlayer().normalized;
        _enemy.transform.rotation = Quaternion.LookRotation(lookDir);
    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Attack, false);
        _enemy.Agent.enabled = true;
    }

}