using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _detectPlayerDuration;
    [SerializeField] private Transform _handTransform;
    [SerializeField] private AudioSource _enemyAudioSource;
    [SerializeField] private GameObject _targetingIcon;
    [SerializeField] private GameObject _warningIcon;
    [SerializeField] private List<Transform> _destinationTransformList;
    
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
    private EnemyStateBase _currentState;
    private Dictionary<EnemyState, EnemyStateBase> _stateDictionary;
    private List<Vector3> _patrolDestinationList;

    private bool _isDetectPlayer;
    private uint _ammoCnt;
    private Vector3 _lootBoxOffset;
    private Vector3 _lastSeenPlayerPosition;
    private const float _attackRangeMultiplier = 0.8f;

    public NavMeshAgent Agent { get { return _agent; } }
    public Transform MuzzleTransform { get { return _muzzleTransform; } }
    public bool IsDetectPlayer { get { return _isDetectPlayer; } }
    public GameObject GunObject { get { return _gunObject; } }
    public uint AmmoCnt
    {
        get { return _ammoCnt; }
        set { _ammoCnt = value; }
    }
    public HealthPoint HP { get { return _hp; } }
    public Vector3 LastSeenPlayerPosition { get { return _lastSeenPlayerPosition; } }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _hp = GetComponent<HealthPoint>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _dataManager = DataManager.Instance;
        _poolManager = PoolManager.Instance;
        _soundManager = SoundManager.Instance;

        _enemyData = _dataManager.GetEnemyData();
        _gunData = _dataManager.GetRandomGunData();
        //_gunData = _dataManager.GetGun(GunType.Glock);
        _ammoCnt = _gunData.MagazineCapacity;
        _gunObject = _poolManager.GetObject(_gunData.Id, _handTransform, true);
        _muzzleTransform = _gunObject.GetComponent<Gun>().MuzzleTransform;
        
        //Temp
        _lootBoxOffset = new Vector3(0, 0.5f, 0f);

        DeactivateGun();

        _patrolDestinationList = new List<Vector3>();
        SetPatrolDestination();

        _hp.MaxHP = _enemyData.MaxHP;
        _hp.OnHpChanged += HandleHpChanged;
        HealHP(_hp.MaxHP);

        _playerTransform = GameManager.Instance.PlayerObject.transform;
        Vector3 spawnPosition = transform.position;

        _stateDictionary = new Dictionary<EnemyState, EnemyStateBase>
        {
            {EnemyState.Idle, new IdleState(this, _enemyData)},
            {EnemyState.Patrol, new PatrolState(this, _enemyData, _patrolDestinationList)},
            {EnemyState.Chase, new ChaseState(this, _enemyData)},
            {EnemyState.Return, new ReturnState(this, _enemyData, spawnPosition)},
            {EnemyState.Attack, new AttackState(this, _enemyData, _gunData)},
            {EnemyState.Flee, new FleeState(this, _enemyData, spawnPosition)},
            {EnemyState.Death, new DeathState(this, _enemyData)}
        };

        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        _currentState.Update();
    }

    private void HandleHpChanged()
    {
        if (_hp.CurrentHP <= 0)
        {
            ChangeState(EnemyState.Death);
            return;
        }

        if (_hp.CurrentHP / _hp.MaxHP < 0.1f)
        {
            ChangeState(EnemyState.Flee);
            return;
        }
    }

    public void ChangeState(EnemyState newState)
    {
        Debug.Log(newState);

        _currentState?.Exit();
        _currentState = _stateDictionary[newState];
        _currentState.Enter();
    }

    public void SetAnimation(string param, bool value)
    {
        _anim.SetBool(param, value);
    }

    public void DetectPlayer(float playerSoundLevel)
    {
        if (DetectPlayerBySight() || DetectPlayerBySound(playerSoundLevel))
            _isDetectPlayer = true;
        else 
            _isDetectPlayer = false;
    }

    private bool DetectPlayerBySight()
    {
        Vector3 dirToPlayer = GetDirectionToPlayer();
        float distToPlayer = GetDistanceToPlayer();

        // 거리
        if (distToPlayer > _enemyData.SightDistance)
            return false;

        // 각도
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > _enemyData.SightAngle / 2f)
            return false;

        // 장애물
        Vector3 eyePosition = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(eyePosition, dirToPlayer.normalized, distToPlayer, _obstacleLayer))
            return false;

        _lastSeenPlayerPosition = _playerTransform.position;
        return true;
    }

    private bool DetectPlayerBySound(float playerSoundLevel)
    {
        float distToPlayer = GetDistanceToPlayer();
        float soundLevelByDist = playerSoundLevel / distToPlayer;

        if (soundLevelByDist >= _enemyData.SoundDetectionLevel)
        {
            _lastSeenPlayerPosition = _playerTransform.position;
            return true;
        }
        else
            return false;
    }

    public void LostPlayer()
    {
        _isDetectPlayer = false;
    }

    public Vector3 GetDirectionToPlayer()
    {
        return (_playerTransform.position - transform.position).normalized;
    }

    public float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _playerTransform.position);
    }

    public Vector3 GetMuzzleDirectionToPlayer()
    {
        return (_playerTransform.position - _muzzleTransform.position).normalized;
    }

    public float GetDistanceMuzzleToPlayer()
    {
        return Vector3.Distance(_muzzleTransform.position, _playerTransform.position);
    }

    public void HealHP(float healAmount)
    {
        _hp.Heal(healAmount);
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

    public bool IsPlayerInAttackRange(float offset = 0f)
    {
        if (GetDistanceToPlayer() <= _gunData.Range * _attackRangeMultiplier + offset)
            return true;

        return false;
    }

    private void MakeLootBox()
    {
        GameObject lootBox = Instantiate(GameResources.Instance.LootBoxPrefab, transform.position + _lootBoxOffset, transform.rotation);

        if (lootBox.GetComponent<LootBox>() == null)
            Debug.LogError("LootBox Has not EnemyGunData Property");
        
        lootBox.GetComponent<LootBox>().EnemyGunData = _gunData;
    }

    public void ShowTargetingIcon(bool show)
    {
        _targetingIcon.SetActive(show);
    }

    public void ShowWarningIcon(bool show)
    {
        _warningIcon.SetActive(show);
    }

    private void SetPatrolDestination()
    {
        TryAddDestination(transform);

        foreach (Transform point in _destinationTransformList)
            TryAddDestination(point);
    }

    private void TryAddDestination(Transform point)
    {
        float raycastHeight = 10f;
        float range = 2f;

        Vector3 rayOrigin = new Vector3(point.position.x, point.position.y + raycastHeight, point.position.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, _groundLayer))
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, range, NavMesh.AllAreas))
                _patrolDestinationList.Add(navHit.position);
    }


    #region Coroutine


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
