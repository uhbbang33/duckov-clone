
public class AttackState : EnemyStateBase
{
    public AttackState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        _enemy.Agent.enabled = false;
        _enemy.SetAnimation(EnemyAnimParm.Attack, true);
    }

    public override void Update()
    {
        // 플레이어 쳐다보기

        // 플레이어가 사거리보다 멀리 가면 Return State

    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Attack, false);
        _enemy.Agent.enabled = true;
    }

}