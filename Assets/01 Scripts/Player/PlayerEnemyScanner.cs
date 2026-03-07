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

    private const float _scanRange = 30f;
    private const float _idleSoundLevel = 2f;
    private const float _walkSoundLevel = 6f;
    private const float _runSoundLevel = 11f;

    public float PlayerSoundLevel
    {
        get { return _soundLevel; }
        set { _soundLevel = value; }
    }


    private void Awake()
    {
        _scanResults = new Collider[100];
        _soundLevel = _idleSoundLevel;
    }

    private void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerMove.OnRun += OnPlayerRun;
        _playerMove.OnRunCancel += OnPlayerWalk;
        _playerMove.OnWalk += OnPlayerWalk;
        _playerMove.OnWalkCancel += OnPlayerIdle;

        StartScan();
    }

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

        SendSoundLevelToEnemy();

        (_previous, _current) = (_current, _previous);
    }

    private void SendSoundLevelToEnemy()
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
            enemy.DetectPlayer(_soundLevel);
        }
    }

    public void StartScan()
    {
        InvokeRepeating(nameof(ScanEnemy), 0f, 0.1f);
    }

    private void OnPlayerRun()
    {
        _soundLevel = _runSoundLevel;
    }

    private void OnPlayerWalk()
    {
        _soundLevel = _walkSoundLevel;
    }
    
    private void OnPlayerIdle()
    {
        _soundLevel = _idleSoundLevel;
    }
}
