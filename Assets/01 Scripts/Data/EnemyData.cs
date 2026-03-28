[System.Serializable]
public class EnemyData
{
    public uint MaxHP;
    public float WalkSpeed;
    public float RunSpeed;
    public float SightDistance;
    public float SightAngle;
    public float FireIntervalMultiplier;
    public float SearchDuration;
    public float ChaseRange;
}

[System.Serializable]
public class EnemyDataList
{
    public EnemyData[] EnemyBaseStatsDatas;
}
