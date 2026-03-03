[System.Serializable]
public class EnemyData
{
    public uint MaxHP;
    public float WalkSpeed;
    public float RunSpeed;
    public float PatrolRange;
    public float SoundDetectionLevel;
    public float ViewRange;
    public float ViewAngle;
}

[System.Serializable]
public class EnemyDataList
{
    public EnemyData[] EnemyDatas;
}
