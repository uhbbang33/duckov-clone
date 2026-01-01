using UnityEngine;

public class JsonParsing : MonoBehaviour
{
    private GunDataList _gunDataList;
    private AmmoDataList _ammoDataList;
    private UsableItemDataList _usableItemDataList;
    private EtcItemDataList _etcItemDataList;

    private void Awake()
    {
        TextAsset jsonText = LoadDataList("JsonData/GunItemData");
        _gunDataList = JsonUtility.FromJson<GunDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/AmmoItemData");
        _ammoDataList = JsonUtility.FromJson<AmmoDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/UsableItemData");
        _usableItemDataList = JsonUtility.FromJson<UsableItemDataList>(jsonText.text);

        jsonText = LoadDataList("JsonData/EtcItemData");
        _etcItemDataList = JsonUtility.FromJson<EtcItemDataList>(jsonText.text);
    }

    private TextAsset LoadDataList(string dataAddress)
    {
        TextAsset jsonText = Resources.Load<TextAsset>(dataAddress);

        if(jsonText == null)
        {
            Debug.LogError("json Data not found");
        }

        return jsonText;
    }

    public GunData GetGun(int id)
    {
        foreach(var gun in _gunDataList.GunItemDatas)
            if (gun.id == id)
                return gun;
        return null;
    }

    public AmmoData GetAmmo(int id)
    {
        foreach (var ammo in _ammoDataList.AmmoItemDatas)
            if (ammo.id == id)
                return ammo;
        return null;
    }

    public UsableItemData GetUsableItem(int id)
    {
        foreach (var usableItem in _usableItemDataList.UsableItemDatas)
            if (usableItem.id == id)
                return usableItem;
        return null;
    }

    public EtcItemData GetEtcItem(int id)
    {
        foreach (var etcItem in _etcItemDataList.EtcItemDatas)
            if (etcItem.id == id)
                return etcItem;
        return null;
    }
}
