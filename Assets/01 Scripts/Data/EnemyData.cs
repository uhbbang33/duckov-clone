[System.Serializable]
public class EnemyData
{
    public uint MaxHP;
    public float WalkSpeed;
    public float RunSpeed;
    public float PatrolRange;
    public float SightDistance;
    public float SightAngle;
    public float SoundDetectionLevel;
    public float FireIntervalMultiplier;
}

[System.Serializable]
public class EnemyDataList
{
    public EnemyData[] EnemyDatas;
}
