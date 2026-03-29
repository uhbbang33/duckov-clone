
public class ReturnState : EnemyStateBase
{
    public ReturnState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        _enemy.CurrentDestinationCount = 0;

        _enemy.Agent.isStopped = false;
        _enemy.Agent.speed = _enemy.Data.WalkSpeed;
        _enemy.Agent.SetDestination(_enemy.SpawnPosition);

        _enemy.SetAnimation(EnemyAnimParm.ArmRaised, false);
        _enemy.SetAnimation(EnemyAnimParm.Walk, true);
    }

    public override void Update()
    {
        if (_enemy.Agent.remainingDistance <= _enemy.Agent.stoppingDistance)
            _enemy.ChangeState(EnemyState.Idle);
    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Walk, false);
    }
}
