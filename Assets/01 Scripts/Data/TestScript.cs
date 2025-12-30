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
        WeaponData weaponData = _load.GetWeapon(258);
        Debug.Log(weaponData.itemType);
        Debug.Log(weaponData.name);
        Debug.Log(weaponData.bulletType);

    }
}
