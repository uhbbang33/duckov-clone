
public class GunItem : Item
{
    private string _ammoType;
    private float _damage;
    private float _rps;
    private uint _magazineCapacity;
    private float _range;
    private float _reloadTime;
    private float _adsTime;
    private float _soundRange;

    private int _currentAmmoCount;
    private uint _ammoId;
    private AmmoItem _ammo;
    private float _originGunWeight;
    private int _originGunValue;

    public string GunItemType { get { return _itemType; } }
    public string GunAmmoType { get { return _ammoType; } }
    public float Damage { get { return _damage; } }
    public float Rps { get { return _rps; } }
    public uint MagazineCapacity { get { return _magazineCapacity; } }
    public float Range { get { return _range; } }
    public float ReloadTime { get { return _reloadTime; } }
    public float AdsTime { get { return _adsTime; } }
    public float SoundRange {  get { return _soundRange; } }

    public int CurrentAmmoCount
    {
        get { return _currentAmmoCount; }
        set
        {
            _currentAmmoCount = value;

            _weight = _originGunWeight + _currentAmmoCount * _ammo.Weight;
            _value = (uint)(_originGunValue + _ammo.Value * _currentAmmoCount);
        }
    }
    public uint AmmoId { get { return _ammoId; } }
    public AmmoItem Ammo
    {
        get { return _ammo; }
        set { _ammo = value; }
    }

    public GunItem(uint id, string rarity, string name, uint value, float weight, uint weightValue, string ammoType, float damage, float rps, uint magazineCapacity, float range, float reloadTime, float adsTime, float soundRange, uint maxStackSize) : base(id, rarity, name, value, weight, weightValue, maxStackSize)
    {
        _currentAmmoCount = 0;
        _itemType = ItemType.Gun;
        _ammoType = ammoType;
        _damage = damage;
        _rps = rps;
        _magazineCapacity = magazineCapacity;
        _range = range;
        _reloadTime = reloadTime;
        _adsTime = adsTime;
        _soundRange = soundRange;

        _originGunWeight = weight;
        _originGunValue = (int)value;

        DataManager dataManager = DataManager.Instance;
        _ammoId = dataManager.GetAmmoId(ammoType);
        _ammo = dataManager.GetItemDataByID((int)_ammoId).ToItem() as AmmoItem;
    }
}
