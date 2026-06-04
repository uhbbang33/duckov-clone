
public static class SceneName
{
    public const string TitleScene = "01TitleScene";
    public const string BunkerScene = "02BunkerScene";
    public const string FieldScene = "03FieldScene";
}

public static class ItemType
{
    public const string Gun = "Gun";
    public const string Ammo = "Ammo";
    public const string Medicine = "Medicine";
    public const string Food = "Food";
    public const string Etc = "Etc"; 
}

public static class AmmoType
{
    public const string S = "S";
    public const string Sniping = "저격";
}

public static class RarityLoadingTime
{
    public const float Common = 0.8f;
    public const float Uncommon = 1.5f;
    public const float Rare = 3f;
    public const float Legendary = 4.8f;
}

public static class Durability
{
    public const int MaxDurability = 100;
}

public static class GunId
{
    public const int Mp7Id = 258;
    public const int M700Id = 780;
    public const int GlockId = 254;
}

public static class AmmoId
{
    public const uint S = 595;
    public const uint Sniping = 622;
}

public static class PoolId
{
    public const uint Bullet = 1;
    public const uint Mp7 = GunId.Mp7Id;
    public const uint M700 = GunId.M700Id;
    public const uint Glock = GunId.GlockId;
    public const uint MuzzleFlash = 5;
    public const uint Smoke = 6;
    public const uint BloodSmoke = 7;
    public const uint DroppedItem = 8;
    public const uint LootBox = 9;
}

public static class PlayerAnimParm
{
    public const string Walk = "IsWalk";
    public const string Run = "IsRun";
    public const string Roll = "Roll";
    public const string Die = "Die";
}

public static class EnemyAnimParm
{
    public const string Walk = "IsWalk";
    public const string Run = "IsRun";
    public const string ArmRaised = "IsArmRaised";
    public const string Reload = "IsReload";
}

public static class Tag
{
    public const string Player = "Player";
}

public static class Layer
{
    public const string Player = "Player";
}

public static class SFXName
{
    public const string GameOver = "GameOver";
    public const string MenuHover = "MenuHover";
    public const string Hit = "Hit";
    public const string OpenInventory = "OpenInventory";
    public const string PickItem = "PickItem";
    public const string SellBuy = "SellBuy";
    public const string FootStep = "FootStep";
    public const string Roll = "Roll";
    public const string Quack = "Quack";
    public const string EatFood = "EatFood";
    public const string UseMedicine = "UseMedicine";
}