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
    STORAGE
}

public enum InteractableType
{
    BOX,
    DROPPEDITEM,

}

public struct RarityLoadingTime
{
    public const float Common = 0.5f;
    public const float Uncommon = 1f;
    public const float Rare = 2f;
    public const float Legendary = 3f;
}