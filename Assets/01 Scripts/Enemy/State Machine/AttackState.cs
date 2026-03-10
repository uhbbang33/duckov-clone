using UnityEngine;

public class AttackState : EnemyStateBase
{
    private GunData _gunData;
    private PoolManager _poolManager;

    private bool _isReloading;
    private float _reloadTimer;
    private float _attackTimer;
    private const float _attackRangeOffset = 5f;

    public AttackState(Enemy enemy, GunData gunData) : base(enemy)
    {
        _gunData = gunData;
        _poolManager = PoolManager.Instance;
    }

    public override void Enter()
    {
        _enemy.SetAnimation(EnemyAnimParm.Attack, true);
    }

    public override void Update()
    {
        if (_enemy.HP.CurrentHP <= 0)
            _enemy.ChangeState(EnemyState.Death);

        float distToPlayer = _enemy.GetDistanceToPlayer();
        if (_gunData.Range < distToPlayer)
        {
            _enemy.ChangeState(EnemyState.Return);
            return;
        }

        LookPlayer();

        if (!_enemy.GunObject.activeSelf)
        {
            _enemy.Agent.isStopped = true;
            return;
        }

        WalkToPlayer(distToPlayer);

        if (ReloadGun())
            return;

        // Fire Gun
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= (1 / _gunData.Rps))
        {
            FireGun();
            _attackTimer = 0f;
        }
    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Reload, false);
        _enemy.SetAnimation(EnemyAnimParm.Attack, false);
        _enemy.SetAnimation(EnemyAnimParm.Walk, false);
        _enemy.Agent.isStopped = false;
    }

    private void FireGun()
    {
        // Bullet
        Vector3 dir = _enemy.GetDirectionToPlayer();
        dir.y = 0f;

        GameObject bulletObject = _poolManager.GetObject(PoolId.Bullet, _enemy.MuzzleTransform, false);


        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.BulletDamage = _gunData.Damage;
        bullet.Fire(dir, _gunData.Range);


        // muzzle effect
        GameObject muzzleFlash = _poolManager.GetObject(PoolId.MuzzleFlash, _enemy.MuzzleTransform, false);


        // Sound
        _enemy.PlayFireSound();


        _enemy.AmmoCnt -= 1;
    }

    private bool ReloadGun()
    {
        if (_isReloading)
        {
            _reloadTimer += Time.deltaTime;
            if (_reloadTimer >= _gunData.ReloadTime)
            {
                _enemy.AmmoCnt = _gunData.MagazineCapacity;
                _reloadTimer = 0f;
                _isReloading = false;
                _enemy.PlayReloadSound(false);
                _enemy.SetAnimation(EnemyAnimParm.Reload, false);
            }
            return true;
        }

        if (_enemy.AmmoCnt <= 0)
        {
            _isReloading = true;
            _reloadTimer = 0f;
            _enemy.PlayReloadSound(true);
            _enemy.SetAnimation(EnemyAnimParm.Reload, true);
            return true;
        }

        return false;
    }

    private void WalkToPlayer(float distToPlayer)
    {
        if (distToPlayer > _gunData.Range - _attackRangeOffset)
        {
            _enemy.SetAnimation(EnemyAnimParm.Walk, true);
            _enemy.Agent.isStopped = false;
            _enemy.Agent.SetDestination(_enemy.PlayerTransform.position);
        }
        else
        {
            _enemy.SetAnimation(EnemyAnimParm.Walk, false);
            _enemy.Agent.isStopped = true;
        }
    }

    private void LookPlayer()
    {
        Vector3 lookDir = _enemy.GetDirectionToPlayer().normalized;
        lookDir.y = 0f;
        _enemy.transform.rotation = Quaternion.LookRotation(lookDir);
    }

}