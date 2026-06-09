using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private LayerMask _obstacleLayer;
    [SerializeField] private float _eyeOffset;

    private Transform _playerTransform;
    private EnemyData _enemyData;
    private GunData _gunData;

    private bool _isPlayerInSight;
    private bool _isNoiseHeard;
    private Vector3 _lastSeenPlayerPosition;

    public bool IsPlayerInSight => _isPlayerInSight;
    public bool IsNoiseHeard => _isNoiseHeard;
    public Vector3 LastSeenPlayerPosition => _lastSeenPlayerPosition;

    public void Init(Transform player, EnemyData enemyData, GunData gunData)
    {
        _playerTransform = player;
        _enemyData = enemyData;
        _gunData = gunData;
    }

    public void DetectPlayer() => _isPlayerInSight = DetectPlayerBySight();
    public void LostPlayer() => _isPlayerInSight = false;

    public void SoundDetectPlayer(bool isDetect)
    {
        _isNoiseHeard = isDetect;
        if (isDetect)
            UpdateLastSeenPlayerPosition();
    }

    public bool IsPlayerInAttackRange(float offset = 0f) => GetDistanceToPlayer() <= _gunData.Range + offset;

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


    private void UpdateLastSeenPlayerPosition()
    {
        _lastSeenPlayerPosition = _playerTransform.position;
    }


    #region Direction And Distance
    private Vector3 GetDirectionToPlayer()
    {
        return (_playerTransform.position - transform.position).normalized;
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, _playerTransform.position);
    }
    #endregion Direction And Distance
}
