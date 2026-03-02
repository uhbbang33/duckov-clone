
public abstract class EnemyStateBase
{
    protected Enemy _enemy;

    public EnemyStateBase(Enemy enemy) { _enemy = enemy; }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
}
