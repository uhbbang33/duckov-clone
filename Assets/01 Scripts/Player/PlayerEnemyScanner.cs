using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyScanner : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;

    private Player _player;
    private PlayerMove _playerMove;

    private Collider[] _scanResults;
    private Collider[] _soundScanResults;

    private HashSet<Enemy> _currentVisible = new();
    private HashSet<Enemy> _previousVisible = new();
    private HashSet<Enemy> _currentAudible = new();
    private HashSet<Enemy> _previousAudible = new();

    private PlayerSoundData _soundData;
    private float _soundLevel;

    private const float _scanRange = 60f;
    private const float _scanInterval = 0.1f;

    public float PlayerSoundLevel
    {
        get { return _soundLevel; }
        set { _soundLevel = value; }
    }


    private void Awake()
    {
        _player = GetComponent<Player>();
        _player.OnPlayerDataInitialized += OnPlayerSoundDataInitialized;

        _playerMove = GetComponent<PlayerMove>();

        _scanResults = new Collider[100];
        _soundScanResults = new Collider[100];
    }

    private void Start()
    {
        _playerMove.OnRun += OnPlayerRun;
        _playerMove.OnRunCancel += OnPlayerWalk;
        _playerMove.OnWalk += OnPlayerWalk;
        _playerMove.OnWalkCancel += OnPlayerIdle;

        InvokeRepeating(nameof(Scan), 0f, _scanInterval);
    }

    private void OnDisable()
    {
        _playerMove.OnRun -= OnPlayerRun;
        _playerMove.OnRunCancel -= OnPlayerWalk;
        _playerMove.OnWalk -= OnPlayerWalk;
        _playerMove.OnWalkCancel -= OnPlayerIdle;
    }

    private void OnPlayerSoundDataInitialized()
    {
        _soundData = _player.SoundData;
        _soundLevel = _soundData.DefaultSoundLevel;
    }

    private void Scan()
    {
        ScanBySight();
        ScanBySound();
    }

    private void ScanBySight()
    {
        OverlapToHashSet(_scanRange, _scanResults, _currentVisible);
        UpdateEnemyDetection(_currentVisible, _previousVisible, false);
        (_previousVisible, _currentVisible) = (_currentVisible, _previousVisible);
    }

    private void ScanBySound()
    {
        OverlapToHashSet(_soundLevel, _soundScanResults, _currentAudible);
        UpdateEnemyDetection(_currentAudible, _previousAudible, true);
        (_previousAudible, _currentAudible) = (_currentAudible, _previousAudible);
    }

    private void OverlapToHashSet(float radius, Collider[] overlapResults, HashSet<Enemy> result)
    {
        result.Clear();

        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, overlapResults, _enemyLayer);

        for (int i = 0; i < count; ++i)
        {
            if (overlapResults[i].TryGetComponent(out Enemy enemy))
                result.Add(enemy);
        }
    }

    private void UpdateEnemyDetection(HashSet<Enemy> current, HashSet<Enemy> previous, bool isSound)
    {
        foreach (Enemy enemy in previous)
        {
            if (enemy != null && !current.Contains(enemy))
            {
                if (isSound)
                    enemy.SoundDetectPlayer(false);
                else
                    enemy.LostPlayer();
            }
        }

        foreach (Enemy enemy in current)
        {
            if (isSound)
                enemy.SoundDetectPlayer(true);
            else
                enemy.DetectPlayer();
        }
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
