
[System.Serializable]
public class PlayerBaseData
{
    public uint MaxHP;
    public uint MaxSP;
    public uint MaxHydration;
    public uint MaxHunger;
    public float SPRegenInterval;
    public float SPRegenAmount;
    public float HydrationLossPerSec;
    public float HungerLossPerSec;
}

public class PlayerBaseDataList
{
    public PlayerBaseData[] PlayerBaseStatsDatas;
}


[System.Serializable]
public class PlayerMoveData
{
    public float WalkSpeed;
    public float RunSpeed;
    public float RunSPConstInterval;
    public float RunSPCost;
    public float RunHydrationLossRate;
    public float RunHungerLossRate;
    public float RollMoveSpeed;
    public float RollDuration;
    public float RollDistance;
    public float RollSPCost;
    public float RollCooldown;
}

public class PlayerMoveDataList
{
    public PlayerMoveData[] PlayerMoveStatsDatas;
}


[System.Serializable]
public class PlayerSoundData
{
    public float DefaultSoundLevel;
    public float WalkSoundLevel;
    public float RunSoundLevel;
}


public class PlayerSoundDataList
{
    public PlayerSoundData[] PlayerSoundDatas;
}