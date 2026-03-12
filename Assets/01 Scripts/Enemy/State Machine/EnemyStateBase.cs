
public abstract class EnemyStateBase
{
    protected Enemy _enemy;
    protected EnemyData _enemyData;

    public EnemyStateBase(Enemy enemy, EnemyData enemyData)
    {
        _enemy = enemy;
        _enemyData = enemyData;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
}
