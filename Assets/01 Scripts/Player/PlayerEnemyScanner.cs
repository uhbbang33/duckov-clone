using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyScanner : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;

    private PlayerMove _playerMove;
    private Collider[] _scanResults;
    private int _resultCnt;
    private float _soundLevel;
    private HashSet<Enemy> _current = new();
    private HashSet<Enemy> _previous = new();

    private PlayerSoundData _soundData;

    private const float _scanRange = 30f;

    public float PlayerSoundLevel
    {
        get { return _soundLevel; }
        set { _soundLevel = value; }
    }


    private void Awake()
    {
        _playerMove = GetComponent<PlayerMove>();

        _scanResults = new Collider[100];
    }

    private void Start()
    {
        _soundData = GetComponent<Player>().SoundData;
        _soundLevel = _soundData.DefaultSoundLevel;

        _playerMove.OnRun += OnPlayerRun;
        _playerMove.OnRunCancel += OnPlayerWalk;
        _playerMove.OnWalk += OnPlayerWalk;
        _playerMove.OnWalkCancel += OnPlayerIdle;

        StartScan();
    }

    //private void Update()
    //{
    //    Debug.Log(_soundLevel);
    //}

    private void OnDisable()
    {
        _playerMove.OnRun -= OnPlayerRun;
        _playerMove.OnRunCancel -= OnPlayerWalk;
        _playerMove.OnWalk -= OnPlayerWalk;
        _playerMove.OnWalkCancel -= OnPlayerIdle;
    }

    private void ScanEnemy()
    {
        _current.Clear();

        _resultCnt = Physics.OverlapSphereNonAlloc(
            transform.position,
            _scanRange,
            _scanResults,
            _enemyLayer);

        for(int i =0; i < _resultCnt; ++i)
        {
            Enemy enemy = _scanResults[i].gameObject.GetComponent<Enemy>();
            _current.Add(enemy);
        }

        DetectEnemy();

        (_previous, _current) = (_current, _previous);
    }

    private void DetectEnemy()
    {
        foreach (Enemy enemy in _previous)
        {
            if (enemy != null && !_current.Contains(enemy))
            {
                enemy.LostPlayer();
            }
        }

        foreach (Enemy enemy in _current)
        {
            enemy.DetectPlayerBySound(_soundLevel);
            enemy.DetectPlayerBySight();
        }
    }

    public void StartScan()
    {
        InvokeRepeating(nameof(ScanEnemy), 0f, 0.1f);
    }

    private void OnPlayerRun()
    {
        _soundLevel = _soundData.RunSoundLevel;
    }

    private void OnPlayerWalk()
    {
        _soundLevel = _soundData.WalkSoundLevel;
    }
    
    private void OnPlayerIdle()
    {
        _soundLevel = _soundData.DefaultSoundLevel;
    }
}
