using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private float _detectPlayerDuration;
    [SerializeField] private Transform _handTransform;
    [SerializeField] private AudioSource _enemyAudioSource;
    [SerializeField] private float _dividendRTS = 1f;
    
    private Animator _anim;
    private HealthPoint _hp;
    private NavMeshAgent _agent;
    private Transform _playerTransform;
    private Transform _muzzleTransform;
    private DataManager _dataManager;
    private PoolManager _poolManager;
    private SoundManager _soundManager;

    private EnemyData _enemyData;
    private GunData _gunData;
    private GameObject _gunObject;

    private Coroutine _detectPlayerCoroutine;
    private WaitForSeconds _waitForDetectPlayerDuration;

    private EnemyStateBase _currentState;
    private Dictionary<EnemyState, EnemyStateBase> _stateDictionary;

    private bool _isDetectPlayer;
    private uint _ammoCnt;
    private Vector3 _lootBoxOffset;

    public NavMeshAgent Agent { get { return _agent; } }
    public Transform PlayerTransform { get { return _playerTransform; } }
    public Transform EnemyTransform { get { return transform; } }
    public Transform MuzzleTransform { get { return _muzzleTransform; } }
    public bool IsDetectPlayer { get { return _isDetectPlayer; } }
    public GameObject GunObject { get { return _gunObject; } }
    public uint AmmoCnt
    {
        get { return _ammoCnt; }
        set { _ammoCnt = value; }
    }
    public HealthPoint HP { get { return _hp; } }
    public float DividendRTS
    {
        get { return _dividendRTS; }
        set { _dividendRTS = value; }
    }


    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _hp = GetComponent<HealthPoint>();
        _agent = GetComponent<NavMeshAgent>();

        _waitForDetectPlayerDuration = new WaitForSeconds(_detectPlayerDuration);
    }

    private void Start()
    {
        _dataManager = DataManager.Instance;
        _poolManager = PoolManager.Instance;
        _soundManager = SoundManager.Instance;

        _enemyData = _dataManager.GetEnemyData();
        _gunData = _dataManager.GetRandomGunData();
       // _gunData = _dataManager.GetGun(GunType.M700);
        _ammoCnt = _gunData.MagazineCapacity;
        _gunObject = _poolManager.GetObject(_gunData.Id, _handTransform, true);
        _muzzleTransform = _gunObject.GetComponent<Gun>().MuzzleTransform;
        
        //Temp
        _lootBoxOffset = new Vector3(0, 0.5f, 0f);

        DeactivateGun();

        _hp.MaxHP = _enemyData.MaxHP;
        HealMaxHP();

        _playerTransform = GameManager.Instance.PlayerObject.transform;
        Vector3 spawnPosition = transform.position;

        _stateDictionary = new Dictionary<EnemyState, EnemyStateBase>
        {
            {EnemyState.Idle, new IdleState(this, _enemyData)},
            {EnemyState.Patrol, new PatrolState(this, _enemyData, spawnPosition, _enemyData.PatrolRange, _enemyData.WalkSpeed)},
            {EnemyState.Chase, new ChaseState(this, _enemyData, _enemyData.RunSpeed, _gunData.Range)},
            {EnemyState.Return, new ReturnState(this, _enemyData, spawnPosition, _enemyData.WalkSpeed)},
            {EnemyState.Attack, new AttackState(this, _enemyData, _gunData)},
            {EnemyState.Death, new DeathState(this, _enemyData)}
        };

        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        if (_hp.CurrentHP <= 0)
        {
            ChangeState(EnemyState.Death);
            return;
        }

        _currentState.Update();
    }

    public void ChangeState(EnemyState newState)
    {
        //Debug.Log(newState);

        _currentState?.Exit();
        _currentState = _stateDictionary[newState];
        _currentState.Enter();
    }

    public void SetAnimation(string param, bool value)
    {
        _anim.SetBool(param, value);
    }

    public void DetectPlayerBySight()
    {
        Vector3 dirToPlayer = GetDirectionToPlayer();
        float distToPlayer = GetDistanceToPlayer();

        // 거리
        if (distToPlayer > _enemyData.SightDistance)
            return;

        // 각도
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > _enemyData.SightAngle / 2f)
            return;

        // 장애물
        if (Physics.Raycast(transform.position, dirToPlayer.normalized, distToPlayer, _obstacleLayer))
            return;

        if (_detectPlayerCoroutine != null)
            StopCoroutine(_detectPlayerCoroutine);

        _detectPlayerCoroutine = StartCoroutine(DetectPlayerRoutine());
    }

    public void DetectPlayerBySound(float playerSoundLevel)
    {
        float distToPlayer = GetDistanceToPlayer();
        float soundLevelByDist = playerSoundLevel / distToPlayer;
        //Debug.Log("Sound Level By Dist : " + soundLevelByDist);

        if (soundLevelByDist >= _enemyData.SoundDetectionLevel)
        {
            if (_detectPlayerCoroutine != null)
                StopCoroutine(_detectPlayerCoroutine);

            _detectPlayerCoroutine = StartCoroutine(DetectPlayerRoutine());
        }
    }

    public void LostPlayer()
    {
        _isDetectPlayer = false;
    }

    public float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _playerTransform.position);
    }

    public float GetDistanceMuzzleToPlayer()
    {
        return Vector3.Distance(_muzzleTransform.position, _playerTransform.position);
    }

    public Vector3 GetDirectionToPlayer()
    {
        return (_playerTransform.position - _muzzleTransform.position).normalized;
    }

    public void HealMaxHP()
    {
        _hp.Heal(_hp.MaxHP);
    }

    public void PlayFireSound()
    {
        _soundManager.PlayGunSFX(_gunData.Id, _enemyAudioSource);
    }

    public void PlayReloadSound(bool isStart)
    {
        _soundManager.PlayReloadSFX(isStart, _enemyAudioSource);
    }

    public void EnemyDeath()
    {
        MakeLootBox();
        Destroy(gameObject);
    }

    private void MakeLootBox()
    {
        GameObject lootBox = Instantiate(GameResources.Instance.LootBoxPrefab, transform.position + _lootBoxOffset, transform.rotation);

        if (lootBox.GetComponent<LootBox>() == null)
            Debug.LogError("LootBox Has not EnemyGunData Property");
        
        lootBox.GetComponent<LootBox>().EnemyGunData = _gunData;
    }

    #region Coroutine

    private IEnumerator DetectPlayerRoutine()
    {
        _isDetectPlayer = true;

        yield return _waitForDetectPlayerDuration;

        _isDetectPlayer = false;
    }

    #endregion

    #region Animation Event

    private void ActivateGun()
    {
        _gunObject.SetActive(true);
    }

    private void DeactivateGun()
    {
        _gunObject.SetActive(false);
    }

    #endregion

    private void OnDrawGizmos()
    {
        // 시야 거리 원
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 20f);

        // 시야각 좌우 경계선
        Gizmos.color = Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, -120f / 2f, 0) * transform.forward * 20f;
        Vector3 rightBoundary = Quaternion.Euler(0, 120f / 2f, 0) * transform.forward * 20f;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
