using UnityEngine;

public class TestScript : MonoBehaviour
{
    private void Start()
    {
        ExcelToJsonConverter.ConvertExcelToJson(
            "Assets/06 Data/ExcelData/Weapon_Item_Table.xlsx",
            "Assets/06 Data/JsonData/WeaponData.json");
    }
}
