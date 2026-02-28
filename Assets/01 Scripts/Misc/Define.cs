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
    public const string Sniping = "РњАн";
}

public enum SlotType
{
    INVENTORY,
    BOX,
    EQUIP,
    QUICKSLOT,
    MAINQUICKSLOT,
    STORAGE
}

public enum InteractableType
{
    BOX,
    DROPPEDITEM,

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
    public const uint Mp7 = 2;
    public const uint M700 = 3;
    public const uint Glock = 4;
}