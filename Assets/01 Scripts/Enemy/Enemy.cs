using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemySound))]
[RequireComponent(typeof(EnemyDetection))]
[RequireComponent(typeof(EnemyUI))]
[RequireComponent(typeof(HealthPoint))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _detectPlayerDuration;
    [SerializeField] private Transform _handTransform;
    [SerializeField] private List<Transform> _destinationTransformList;
    
    // Component
    private Animator _anim;
    private HealthPoint _hp;
    private NavMeshAgent _agent;
    private EnemySound _enemySound;
    private EnemyDetection _enemyDetection;
    private EnemyUI _enemyUI;

    // Manager
    private DataManager _dataManager;
    private PoolManager _poolManager;
    private FieldManager _fieldManager;

    // Gun
    private Transform _playerTransform;
    private GameObject _gunObject;
    private Gun _gun;
    private uint _ammoCnt;

    // Destination
    private List<Vector3> _patrolDestinationList;
    private int _currentDestinationCount;
    private Vector3 _spawnPosition;

    // Data
    private EnemyData _enemyData;
    private GunData _gunData;
    private EnemyStateBase _currentState;
    private Dictionary<EnemyState, EnemyStateBase> _stateDictionary;


    #region Property 

    public HealthPoint HP => _hp;
    public NavMeshAgent Agent => _agent;
    public GameObject GunObject => _gunObject;
    public EnemyData Data => _enemyData;
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
    public Vector3 SpawnPosition => _spawnPosition;
    public EnemyDetection Detection => _enemyDetection;
    public EnemyUI UI => _enemyUI;

    #endregion Property

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _hp = GetComponent<HealthPoint>();
        _agent = GetComponent<NavMeshAgent>();
        _enemySound = GetComponent<EnemySound>();
        _enemyDetection = GetComponent<EnemyDetection>();
        _enemyUI = GetComponent<EnemyUI>();
    }

    private void Start()
    {
        _dataManager = DataManager.Instance;
        _poolManager = PoolManager.Instance;
        _fieldManager = FieldManager.Instance;

        _enemyData = _dataManager.GetEnemyData();
        _gunData = _dataManager.GetRandomGunData();
        _ammoCnt = _gunData.MagazineCapacity;
        _gunObject = _poolManager.GetObject(_gunData.Id, _handTransform, true);
        _gun = _gunObject.GetComponent<Gun>();
        _gun.SetRendererEnabled(false);

        DeactivateGun();
        SetPatrolDestination();

        _hp.MaxHP = _enemyData.MaxHP;
        _hp.OnHpChanged += HandleHpChanged;
        HealHP(_hp.MaxHP);

        _playerTransform = GameManager.Instance.PlayerObject.transform;
        _spawnPosition = transform.position;

        Transform muzzleTransform = _gunObject.GetComponent<Gun>().MuzzleTransform;
        _enemyDetection.Init(_playerTransform, _enemyData, _gunData, muzzleTransform);

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

        _agent.avoidancePriority = ++_fieldManager.EnemyAgentPriority;
    }

    private void Update()
    {
        if (Time.timeScale != 0f)
            _currentState.Update();
    }

    public void ChangeState(EnemyState newState)
    {
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
        StopFootStepSound();
        Destroy(gameObject);
    }

    private void MakeLootBox()
    {
        GameObject lootBox = _poolManager.GetObject(PoolId.LootBox);

        if (lootBox.GetComponent<LootBox>() == null)
        {
            Debug.LogError("LootBox Has not EnemyGunData Property");
            return;
        }

        lootBox.transform.position = transform.position;
        lootBox.transform.rotation = transform.rotation;

        lootBox.GetComponent<LootBox>().EnemyGunData = _gunData;
    }

    #endregion Health And Death

    #region Detect Player
    public void SoundDetectPlayer(bool isDetect) => _enemyDetection.SoundDetectPlayer(isDetect);
    public void DetectPlayer() => _enemyDetection.DetectPlayer();

    public bool IsPlayerInAttackRange(float offset = 0f) => _enemyDetection.IsPlayerInAttackRange(offset);

    public void LostPlayer()
    {
        _enemyDetection.LostPlayer();
        _enemyUI.ShowWarningIcon(false);
    }

    public void StartShowWarningIconRoutine()
    {
        _enemyDetection.StartShowWarningIconRoutine(_enemySound, _enemyUI, this);
    }

    public void SetVisible(bool show)
    {
        _enemyUI.SetVisible(show);
        _gun.SetRendererEnabled(show);
    }

    #endregion Detect Player

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

    #region Sound

    public void PlayFireSound() => _enemySound.PlayFire(_gunData.Id);
    public void PlayFootStepSound(bool isRun) => _enemySound.PlayFootStep(isRun);
    public void StopFootStepSound() => _enemySound.StopFootStep();
    public void PlayReloadSound(bool isStart) => _enemySound.PlayReload(isStart);

    #endregion Sound

    #region Event Handler

    private void HandleHpChanged()
    {
        if (_hp.CurrentHP <= 0)
        {
            ChangeState(EnemyState.Death);
            return;
        }

        if (_hp.CurrentHP / _hp.MaxHP <= 0.2f
             && _currentState != _stateDictionary[EnemyState.Idle])
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

    /*
#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Vector3 eyePosition = transform.position + new Vector3(0f, 0.7f, 0f);

        // 시야 거리 원
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyePosition, 30f);

        // 시야각 좌우 경계선
        Gizmos.color = Color.white;
        Vector3 leftBoundary = Quaternion.Euler(0, -120f / 2f, 0) * transform.forward * 30f;
        Vector3 rightBoundary = Quaternion.Euler(0, 120f / 2f, 0) * transform.forward * 30f;
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

            if (Physics.Raycast(eyePosition, dir, out RaycastHit hit, 30f, _obstacleLayer))
                hitPoints[i] = hit.point;
            else
                hitPoints[i] = eyePosition + dir * 30f;
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

#endif
    */
}
