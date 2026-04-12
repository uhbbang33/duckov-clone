using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _detectPlayerDuration;
    [SerializeField] private Transform _handTransform;
    [SerializeField] private AudioSource _enemyAudioSource;
    [SerializeField] private GameObject _targetingIcon;
    [SerializeField] private GameObject _warningIcon;
    [SerializeField] private List<Transform> _destinationTransformList;
    [SerializeField] private float _eyeOffset;
    
    private Animator _anim;
    private HealthPoint _hp;
    private NavMeshAgent _agent;
    private DataManager _dataManager;
    private PoolManager _poolManager;
    private SoundManager _soundManager;

    private Transform _playerTransform;
    private Transform _muzzleTransform;
    private GameObject _gunObject;
    private List<Vector3> _patrolDestinationList;

    private EnemyData _enemyData;
    private GunData _gunData;
    private EnemyStateBase _currentState;
    private Dictionary<EnemyState, EnemyStateBase> _stateDictionary;

    private bool _isPlayerInSight;
    private bool _isNoiseHeard;
    private bool _hasSeenPlayer;
    private uint _ammoCnt;
    private int _currentDestinationCount;

    private Vector3 _lootBoxOffset;
    private Vector3 _lastSeenPlayerPosition;
    private Vector3 _spawnPosition;

    private WaitForSeconds _waitForShowWarningIcon;

    private const float _attackRangeMultiplier = 0.8f;
    private const float _showWarningIconDuration = 1f;

    public HealthPoint HP { get { return _hp; } }
    public NavMeshAgent Agent { get { return _agent; } }
    public Transform MuzzleTransform { get { return _muzzleTransform; } }
    public GameObject GunObject { get { return _gunObject; } }
    public EnemyData Data {  get { return _enemyData; } }
    public bool IsPlayerInSight { get { return _isPlayerInSight; } }
    public bool IsNoiseHeard { get { return _isNoiseHeard; } }
    public bool HasSeenPlayer
    {
        get { return _hasSeenPlayer; }
        set { _hasSeenPlayer = value; }
    }
    public uint AmmoCnt
    {
        get { return _ammoCnt; }
        set { _ammoCnt = value; }
    }
    public int CurrentDestinationCount
    {
        get { return _currentDestinationCount; }
        set { _currentDestinationCount = value; }
    }
    public Vector3 LastSeenPlayerPosition { get { return _lastSeenPlayerPosition; } }
    public Vector3 SpawnPosition { get { return _spawnPosition; } }


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
        SetPatrolDestination();

        _hp.MaxHP = _enemyData.MaxHP;
        _hp.OnHpChanged += HandleHpChanged;
        HealHP(_hp.MaxHP);

        _playerTransform = GameManager.Instance.PlayerObject.transform;
        _spawnPosition = transform.position;
        _waitForShowWarningIcon = new WaitForSeconds(_showWarningIconDuration);

        _stateDictionary = new Dictionary<EnemyState, EnemyStateBase>
        {
            {EnemyState.Idle, new IdleState(this)},
            {EnemyState.Patrol, new PatrolState(this, _patrolDestinationList)},
            {EnemyState.Chase, new ChaseState(this)},
            {EnemyState.Return, new ReturnState(this)},
            {EnemyState.Attack, new AttackState(this, _gunData)},
            {EnemyState.Flee, new FleeState(this)},
            {EnemyState.Death, new DeathState(this)}
        };

        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        if (Time.timeScale != 0f)
            _currentState.Update();
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

    #region Health And Death
    public void HealHP(float healAmount)
    {
        _hp.Heal(healAmount);
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

    #endregion Health And Death

    #region Detect Player
    public void SoundDetectPlayer(bool isDetect)
    {
        _isNoiseHeard = isDetect;
        if (isDetect)
            UpdateLastSeenPlayerPosition();
    }

    public void DetectPlayer()
    {
        _isPlayerInSight = DetectPlayerBySight();
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
        Vector3 eyePosition = transform.position + Vector3.up * _eyeOffset;

        if (Physics.Raycast(eyePosition, dirToPlayer.normalized, distToPlayer, _obstacleLayer))
            return false;

        UpdateLastSeenPlayerPosition();
        return true;
    }

    public bool IsPlayerInAttackRange(float offset = 0f)
    {
        return GetDistanceToPlayer() <= _gunData.Range * _attackRangeMultiplier + offset;
    }

    public void LostPlayer()
    {
        _isPlayerInSight = false;
        ShowWarningIcon(false);
    }

    public void StartShowWarningIconRoutine()
    {
        if (_hasSeenPlayer) return;

        StartCoroutine(ShowWarningIconCoroutine());
        _hasSeenPlayer = true;
    }

    private void UpdateLastSeenPlayerPosition()
    {
        _lastSeenPlayerPosition = _playerTransform.position;

    }

    #endregion Detect Player

    #region Direction And Distance
    private Vector3 GetDirectionToPlayer()
    {
        return (_playerTransform.position - transform.position).normalized;
    }

    private float GetDistanceToPlayer()
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

    #endregion Direction And Distance

    #region Sound

    public void PlayFireSound()
    {
        _soundManager.PlayGunSFX(_gunData.Id, _enemyAudioSource);
    }

    public void PlayReloadSound(bool isStart)
    {
        _soundManager.PlayReloadSFX(isStart, _enemyAudioSource);
    }

    #endregion Sound

    #region Agent Destination
    private void SetPatrolDestination()
    {
        _patrolDestinationList = new List<Vector3>();

        TryAddDestination(transform);

        foreach (Transform point in _destinationTransformList)
            TryAddDestination(point);

        _currentDestinationCount = 1;
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

    #endregion Agent Destination

    #region Show Icon

    public void ShowTargetingIcon(bool show)
    {
        _targetingIcon.SetActive(show);

        if (_warningIcon.activeSelf && show)
            ShowWarningIcon(false);
    }

    private void ShowWarningIcon(bool show)
    {
        _warningIcon.SetActive(show);
    }

    #endregion Show Icon

    #region Event Handler

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

    #endregion Event Handler

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

    #region Coroutine

    private IEnumerator ShowWarningIconCoroutine()
    {
        ShowWarningIcon(true);
        yield return _waitForShowWarningIcon;
        ShowWarningIcon(false);
    }

    #endregion Coroutine

    private void OnDrawGizmos()
    {
        Vector3 eyePosition = transform.position + new Vector3(0f, _eyeOffset, 0f);

        // 시야 거리 원
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyePosition, 20f);

        // 시야각 좌우 경계선
        Gizmos.color = Color.white;
        Vector3 leftBoundary = Quaternion.Euler(0, -120f / 2f, 0) * transform.forward * 20f;
        Vector3 rightBoundary = Quaternion.Euler(0, 120f / 2f, 0) * transform.forward * 20f;
        Gizmos.DrawLine(eyePosition, eyePosition + leftBoundary);
        Gizmos.DrawLine(eyePosition, eyePosition + rightBoundary);

        // FOV 메시
        int rayCount = 50;
        float viewAngle = 120f;
        float stepAngle = viewAngle / rayCount;

        Vector3[] hitPoints = new Vector3[rayCount + 1];

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = -viewAngle / 2f + stepAngle * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

            if (Physics.Raycast(eyePosition, dir, out RaycastHit hit, 20f, _obstacleLayer))
                hitPoints[i] = hit.point;
            else
                hitPoints[i] = eyePosition + dir * 20f;
        }

        // 삼각형 단위로 면 채우기
        for (int i = 0; i < rayCount; i++)
        {
            // 빈 공간 초록 반투명
            Handles.color = new Color(0.2f, 0.9f, 0.4f, 0.15f);
            Handles.DrawAAConvexPolygon(eyePosition, hitPoints[i], hitPoints[i + 1]);

            // 삼각형 경계선
            Gizmos.color = new Color(0.2f, 0.9f, 0.4f, 0.4f);
            Gizmos.DrawLine(hitPoints[i], hitPoints[i + 1]);
        }

        // Player 탐지
        if (_playerTransform == null) return;

        Vector3 dirToPlayer = (_playerTransform.position - eyePosition).normalized;
        float distToPlayer = Vector3.Distance(eyePosition, _playerTransform.position);
        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

        if (angleToPlayer > viewAngle / 2f) return;

        if (Physics.Raycast(eyePosition, dirToPlayer, out RaycastHit playerHit,
                            distToPlayer, _obstacleLayer))
        {
            // 장애물에 막힘
            Gizmos.color = Color.red;
            Gizmos.DrawLine(eyePosition, playerHit.point);
            Gizmos.DrawWireSphere(playerHit.point, 0.1f);

            Gizmos.color = Color.gray;
            Gizmos.DrawLine(playerHit.point, _playerTransform.position);
        }
        else
        {
            // 플레이어 보임
            Gizmos.color = Color.green;
            Gizmos.DrawLine(eyePosition, _playerTransform.position);
            Gizmos.DrawWireSphere(_playerTransform.position, 0.3f);
        }
    }
}
