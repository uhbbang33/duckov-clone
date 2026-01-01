using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private JsonParsing _load;

    private void Start()
    {
        // Check Converter (excel to json)
        //ExcelToJsonConverter.ConvertExcelToJson(
        //    "Assets/06 Data/ExcelData/Weapon_Item_Table.xlsx",
        //    "Assets/Resources/JsonData/WeaponData.json");

        // Check parsing
        GunData gunData = _load.GetGun(258);
        Debug.Log(gunData.itemType);
        Debug.Log(gunData.name);
        Debug.Log(gunData.bulletType);

    }
}
