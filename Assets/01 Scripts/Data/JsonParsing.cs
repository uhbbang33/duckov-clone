using UnityEngine;

public class JsonParsing : MonoBehaviour
{
    private WeaponDataList _weaponDataList;

    private void Awake()
    {
        LoadWeaponDataList();
    }

    private void LoadWeaponDataList()
    {
        TextAsset jsonText = Resources.Load<TextAsset>("JsonData/WeaponData");

        if(jsonText == null)
        {
            Debug.LogError("json Data not found");
            return;
        }

        _weaponDataList = JsonUtility.FromJson<WeaponDataList>(jsonText.text);
    }

    public WeaponData GetWeapon(int id)
    {
        foreach(var weapon in _weaponDataList.WeaponDatas)
        {
            if(weapon.id == id)
            {
                return weapon;
            }
        }

        return null;
    }
}
