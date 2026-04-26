
public struct SceneName
{
    public const string TitleScene = "01TitleScene";
    public const string BunkerScene = "02BunkerScene";
    public const string FieldScene = "03FieldScene";
}

public enum SceneList
{
    TITLE,
    BUNKER,
    FIELD,
}

public struct ItemType
{
    public const string Gun = "Gun";
    public const string Ammo = "Ammo";
    public const string Medicine = "Medicine";
    public const string Food = "Food";
    public const string Etc = "Etc"; 
}

public struct BulletType
{
    public const string S = "S";
    public const string Sniping = "저격";
}

public enum SlotType
{
    INVENTORY,
    BOX,
    EQUIP,
    QUICKSLOT,
    MAINQUICKSLOT,
    STORAGE,
    SHOP
}

public enum InteractableType
{
    BOX,
    DROPPEDITEM,
    STORAGE,
    SHOP,
    DOOR
}

public struct RarityLoadingTime
{
    public const float Common = 0.8f;
    public const float Uncommon = 1.5f;
    public const float Rare = 3f;
    public const float Legendary = 4.8f;
}

public struct Durability
{
    public const int MaxDurability = 100;
}

public struct GunId
{
    public const int Mp7Id = 258;
    public const int M700Id = 780;
    public const int GlockId = 254;
}

public struct BulletId
{
    public const uint S = 595;
    public const uint Sniping = 622;
}

public struct PoolId
{
    public const uint Bullet = 1;
    public const uint Mp7 = GunId.Mp7Id;
    public const uint M700 = GunId.M700Id;
    public const uint Glock = GunId.GlockId;
    public const uint MuzzleFlash = 5;
    public const uint Smoke = 6;
    public const uint BloodSmoke = 7;
}

public struct PlayerAnimParm
{
    public const string Walk = "IsWalk";
    public const string Run = "IsRun";
    public const string Roll = "Roll";
    public const string Die = "Die";
}

public struct EnemyAnimParm
{
    public const string Walk = "IsWalk";
    public const string Run = "IsRun";
    public const string ArmRaised = "IsArmRaised";
    public const string Reload = "IsReload";
}

public struct Tag
{
    public const string Player = "Player";
}

public struct Layer
{
    public const string Player = "Player";
}

public struct SFXName
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

}