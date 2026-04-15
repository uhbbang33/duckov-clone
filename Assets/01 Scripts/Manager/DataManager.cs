using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    private GunDataList _gunDataList;
    private AmmoDataList _ammoDataList;
    private UsableItemDataList _usableItemDataList;
    private EtcItemDataList _etcItemDataList;
    private EnemyDataList _enemyDataList;
    private PlayerBaseDataList _playerBaseDataList;
    private PlayerMoveDataList _playerMoveDataList;
    private PlayerSoundDataList _playerSoundDataList;
    private ShopItemDataList _shopItemDataList;

    private List<UsableItemData> _foodDatas = new();
    private List<UsableItemData> _medicineDatas = new();

    private Dictionary<uint, ItemData> _itemDict = new();

    private SaveAndLoadManager _saveAndLoadManager;

    private bool _isParsed = false;

    #region Property
    public GunDataList GunDatas
    {
        get { return _gunDataList; }
        set { _gunDataList = value; }
    }

    public AmmoDataList AmmoDatas
    {
        get { return _ammoDataList; }
        set { _ammoDataList = value; }
    }

    public UsableItemDataList UsableItemDatas
    {
        get { return _usableItemDataList; }
        set
        {
            _usableItemDataList = value;

            foreach (var data in value.UsableItemDatas)
            {
                if (data.ItemType == ItemType.Food)
                    _foodDatas.Add(data);
                else if (data.ItemType == ItemType.Medicine)
                    _medicineDatas.Add(data);
            }
        }
    }

    public EtcItemDataList EtcItemDatas
    {
        get { return _etcItemDataList; }
        set { _etcItemDataList = value; }
    }

    public EnemyDataList EnemyDatas
    {
        get { return _enemyDataList; }
        set { _enemyDataList = value; }
    }

    public PlayerBaseDataList PlayerBaseList
    {
        get { return _playerBaseDataList; }
        set { _playerBaseDataList = value; }
    }

    public PlayerMoveDataList PlayerMoveList
    {
        get { return _playerMoveDataList; }
        set { _playerMoveDataList = value; }
    }

    public PlayerSoundDataList PlayerSoundList
    {
        get { return _playerSoundDataList; }
        set { _playerSoundDataList = value; }
    }

    public ShopItemDataList ShopItemList
    {
        get { return _shopItemDataList; }
        set { _shopItemDataList = value; }
    }

    public bool IsParsed
    {
        get { return _isParsed; }
        set { _isParsed = value; }
    }

    #endregion Property


    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    #region Save

    public void SaveDataByScene()
    {
        string current = GameManager.Instance.CurrentSceneName;

        if (current == SceneName.BunkerScene)
        {
            SaveAllData();
        }
        else if (current == SceneName.FieldScene)
        {
            SavePlayerData();
        }
    }

    public void SaveAllData()
    {
        SavePlayerData();
        _saveAndLoadManager.SaveStorage();
    }

    public void SavePlayerData()
    {
        _saveAndLoadManager.SavePlayerStats();
        _saveAndLoadManager.SavePlayerInventory();
    }

    public void SetDataForScene(string sceneName)
    {
        if (_saveAndLoadManager == null)
            _saveAndLoadManager = SaveAndLoadManager.Instance;

        if (sceneName == SceneName.FieldScene)
        {
            _saveAndLoadManager.LoadPlayerStats();
            _saveAndLoadManager.LoadPlayerInventory();
        }
        else if (sceneName == SceneName.BunkerScene)
        {
            _saveAndLoadManager.LoadPlayerStats();
            _saveAndLoadManager.LoadPlayerInventory();
            _saveAndLoadManager.LoadStorage();
        }
    }

    #endregion Save


    public void FillItemDictionary()
    {
        foreach (var data in _gunDataList.GunItemDatas)
            _itemDict.Add(data.Id, data);
        foreach (var data in _ammoDataList.AmmoItemDatas)
            _itemDict.Add(data.Id, data);
        foreach (var data in _usableItemDataList.UsableItemDatas)
            _itemDict.Add(data.Id, data);
        foreach (var data in _etcItemDataList.EtcItemDatas)
            _itemDict.Add(data.Id, data);
    }


    #region Get 

    public ItemData GetItemDataByID(int id)
    {
        return _itemDict.GetValueOrDefault((uint)id);
    }

    public GunData GetGunByType(GunType type)
    {
        int id = 0;
        if (type == GunType.Glock)
            id = GunId.GlockId;
        else if (type == GunType.Mp7)
            id = GunId.Mp7Id;
        else if (type == GunType.M700)
            id = GunId.M700Id;

        return GetItemDataByID(id) as GunData;
    }

    public AmmoData GetAmmo(string type)
    {
        uint id = 0;

        if (type == BulletType.S)
            id = BulletId.S;
        else if (type == BulletType.Sniping)
            id = BulletId.Sniping;

        return GetItemDataByID((int)id) as AmmoData;
    }

    public EnemyData GetEnemyData()
    {
        return _enemyDataList.EnemyBaseStatsDatas[0];
    }

    public uint GetBulletId(string bulletType)
    {
        if (bulletType == BulletType.S)
            return BulletId.S;
        if (bulletType == BulletType.Sniping)
            return BulletId.Sniping;

        return 0;
    }

    #endregion Get


    public Item GetRandomItem(string type)
    {
        if (type == ItemType.Gun)
            return GetRandomGunData().ToItem();
        else if (type == ItemType.Ammo)
            return GetRandomAmmoData().ToItem();
        else if (type == ItemType.Food)
            return GetRandomFoodData().ToItem();
        else if (type == ItemType.Medicine)
            return GetRandomMedicineData().ToItem();
        else if (type == ItemType.Etc)
            return GetRandomEtcData().ToItem();

        return null;
    }


    #region Get Random Data

    private T GetRandomItemData<T>(IEnumerable<T> dataList) where T : ItemData
    {
        float totalWeightValue = 0;
        foreach (var data in dataList)
            totalWeightValue += data.WeightValue;

        float random = Random.Range(0, totalWeightValue);
        float current = 0;

        foreach (var data in dataList)
        {
            current += data.WeightValue;
            if (random < current)
                return data;
        }

        return null;
    }

    public GunData GetRandomGunData()
    {
        return GetRandomItemData(_gunDataList.GunItemDatas);
    }

    private AmmoData GetRandomAmmoData()
    {
        return GetRandomItemData(_ammoDataList.AmmoItemDatas);
    }

    private UsableItemData GetRandomFoodData()
    {
        return GetRandomItemData(_foodDatas);
    }

    private UsableItemData GetRandomMedicineData()
    {
        return GetRandomItemData(_medicineDatas);
    }

    private ItemData GetRandomEtcData()
    {
        return GetRandomItemData(_etcItemDataList.EtcItemDatas);
    }

    #endregion
}
