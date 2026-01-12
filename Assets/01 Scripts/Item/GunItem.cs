
public class GunItem : Item
{
    private string _bulletType;
    private float _damage;
    private float _rps;
    private uint _magazineCapacity;
    private float _range;
    private float _reloadTime;
    private float _adsTime;

    public string GunItemType { get { return _itemType; } }
    public string GunBulletType { get { return _bulletType; } }
    public float Damage { get { return _damage; } }
    public float Rps { get { return _rps; } }
    public uint MagazineCapacity { get { return _magazineCapacity; } }
    public float Range { get { return _range; } }
    public float ReloadTime { get { return _reloadTime; } }
    public float AdsTime { get { return _adsTime; } }

    public GunItem(uint id, string name, uint value, float weight, float weightValue, string bulletType, float damage, float rps, uint magazineCapacity, float range, float reloadTime, float adsTime) : base(id, name, value, weight, weightValue)
    {
        _itemType = ItemType.Gun;
        _bulletType = bulletType;
        _damage = damage;
        _rps = rps;
        _magazineCapacity = magazineCapacity;
        _range = range;
        _reloadTime = reloadTime;
        _adsTime = adsTime;
    }
}
