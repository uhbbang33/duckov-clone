
#region Player
public enum PlayerFireState
{
    Idle,
    Firing,
    Reloading,
    ChangeGun
}

public enum PlayerState
{
    Idle,
    Walking,
    Running,
    Rolling,
    Die
}
#endregion Player

#region Enemy
public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Return,
    Attack,
    Flee,
    Death
}
#endregion Enemy

public enum SceneList
{
    TITLE,
    BUNKER,
    FIELD,
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
public enum GunType
{
    Glock,
    Mp7,
    M700
}
