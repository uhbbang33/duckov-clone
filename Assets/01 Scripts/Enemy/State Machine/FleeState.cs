
public class FleeState : EnemyStateBase
{
    private float _runSpeed;

    public FleeState(Enemy enemy) : base(enemy)
    {
        _runSpeed = _enemy.Data.RunSpeed;
    }

    public override void Enter()
    {
        _enemy.Agent.isStopped = false;
        _enemy.Agent.speed = _runSpeed;
        _enemy.Agent.SetDestination(_enemy.SpawnPosition);

        _enemy.SetAnimation(EnemyAnimParm.ArmRaised, false);
        _enemy.SetAnimation(EnemyAnimParm.Run, true);

        _enemy.PlayFootStepSound(true);
    }

    public override void Update()
    {
        if (_enemy.Agent.remainingDistance <= _enemy.Agent.stoppingDistance)
            _enemy.ChangeState(EnemyState.Idle);
    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Run, false);
    }

}
