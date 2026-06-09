using UnityEngine;

public class AttackState : EnemyStateBase
{
    private GunData _gunData;
    private PoolManager _poolManager;

    private bool _isReloading;
    private float _reloadTimer;
    private float _attackTimer;
    private float _burstCooldownTimer;
    private int _fireCount;

    private const float _turnSpeed = 3f;
    private const float _attackOffset = 3f;
    private const float _reloadWalkSpeed = 2.9f;
    private const float _burstCooldownDuration = 1.5f;
    

    public AttackState(Enemy enemy, GunData gunData) : base(enemy)
    {
        _gunData = gunData;
        _poolManager = PoolManager.Instance;
    }

    public override void Enter()
    {
        _enemy.Agent.isStopped = true;
        _fireCount = 0;
        _reloadTimer = 0f;
        _attackTimer = 0f;
        _burstCooldownTimer = 0f;
        _enemy.StopFootStepSound();
    }

    public override void Update()
    {
        if (!_enemy.GunObject.activeSelf)
            return;

        LookPlayer();

        if (ReloadGun())
            return;


        // 사거리
        if (_fireCount == _gunData.EnemyFireCount
            && !_enemy.IsPlayerInAttackRange(_attackOffset))
        {
            _enemy.ChangeState(EnemyState.Chase);
            return;
        }

        // 적 연발 수
        if (_fireCount >= _gunData.EnemyFireCount)
        {
            _burstCooldownTimer += Time.deltaTime;
            if (_burstCooldownTimer >= _burstCooldownDuration)
            {
                _burstCooldownTimer = 0f;
                _fireCount = 0;
            }

            // 공격을 끝낸 후, 플레이어가 시야에 없으면 chase state로 전환
            if (!_enemy.Detection.IsPlayerInSight)
            {
                _enemy.ChangeState(EnemyState.Chase);
            }

            return;
        }


        // 공격 쿨타임
        _attackTimer += Time.deltaTime;
        if (_attackTimer < (1 / _gunData.Rps) * _enemy.Data.FireIntervalMultiplier)
            return;

        FireGun();
        _attackTimer = 0f;
    }

    public override void Exit()
    {
        _enemy.SetAnimation(EnemyAnimParm.Reload, false);
        _enemy.SetAnimation(EnemyAnimParm.Walk, false);
        _enemy.Agent.isStopped = false;
    }

    private void FireGun()
    {
        // Bullet
        Vector3 dir = _enemy.Detection.GetMuzzleDirectionToPlayer();
        dir.y = 0f;

        GameObject bulletObject = _poolManager.GetObject(PoolId.Bullet, _enemy.Detection.MuzzleTransform, false);

        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.BulletDamage = _gunData.Damage;
        bullet.Fire(dir, _gunData.Range);


        // muzzle effect
        _poolManager.GetObject(PoolId.MuzzleFlash, _enemy.Detection.MuzzleTransform, false);


        // Sound
        _enemy.PlayFireSound();


        _enemy.AmmoCnt -= 1;
        _fireCount += 1;
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

                _enemy.Agent.isStopped = true;
                _enemy.SetAnimation(EnemyAnimParm.Walk, false);
            }

            if (_enemy.Agent.remainingDistance <= _enemy.Agent.stoppingDistance)
            {
                _enemy.Agent.isStopped = true;
                _enemy.SetAnimation(EnemyAnimParm.Walk, false);
            }

            return true;
        }

        if (_enemy.AmmoCnt <= 0)
        {
            _isReloading = true;
            _reloadTimer = 0f;
            _enemy.PlayReloadSound(true);
            _enemy.SetAnimation(EnemyAnimParm.Reload, true);

            _enemy.Agent.isStopped = false;
            _enemy.SetAnimation(EnemyAnimParm.Walk, true);
            _enemy.Agent.SetDestination(_enemy.Detection.LastSeenPlayerPosition);
            _enemy.Agent.speed = _reloadWalkSpeed;
            return true;
        }

        return false;
    }

    private void LookPlayer()
    {
        float dist = _enemy.Detection.GetDistanceMuzzleToPlayer();
        if (dist < 1f)
            return;

        Vector3 lookDir = _enemy.Detection.GetMuzzleDirectionToPlayer();
        lookDir.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(lookDir);
        _enemy.transform.rotation = Quaternion.Lerp(_enemy.transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);
    }

}