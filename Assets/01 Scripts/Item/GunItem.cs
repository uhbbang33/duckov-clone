
public class GunItem : Item
{
    // TODO : bullet과 Ammo 명칭 구분
    private string _bulletType; // ammoType으로 바꿔야함
    private float _damage;
    private float _rps;
    private uint _magazineCapacity;
    private float _range;
    private float _reloadTime;
    private float _adsTime;
    private int _currentAmmoCount;
    private uint _bulletId;
    private AmmoItem _ammo;
    private string _ammoName;

    public string GunItemType { get { return _itemType; } }
    public string GunBulletType { get { return _bulletType; } }
    public float Damage { get { return _damage; } }
    public float Rps { get { return _rps; } }
    public uint MagazineCapacity { get { return _magazineCapacity; } }
    public float Range { get { return _range; } }
    public float ReloadTime { get { return _reloadTime; } }
    public float AdsTime { get { return _adsTime; } }
    public int CurrentAmmoCount
    {
        get { return _currentAmmoCount; }
        set { _currentAmmoCount = value; }
    }
    public uint BulletId { get { return _bulletId; } }
    public AmmoItem Ammo
    {
        get { return _ammo; }
        set { _ammo = value; }
    }
    public string AmmoName { get { return _ammoName; } }

    public GunItem(uint id, string rarity, string name, uint value, float weight, uint weightValue, string bulletType, float damage, float rps, uint magazineCapacity, float range, float reloadTime, float adsTime, uint maxStackSize) : base(id, rarity, name, value, weight, weightValue, maxStackSize)
    {
        _currentAmmoCount = 0;
        _itemType = ItemType.Gun;
        _bulletType = bulletType;
        _damage = damage;
        _rps = rps;
        _magazineCapacity = magazineCapacity;
        _range = range;
        _reloadTime = reloadTime;
        _adsTime = adsTime;

        _bulletId = DataManager.Instance.GetBulletId(bulletType);
        _ammoName = DataManager.Instance.GetAmmo((int)_bulletId).Name;
    }
}
