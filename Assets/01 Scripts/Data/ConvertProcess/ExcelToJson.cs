using System.IO;
using UnityEngine;

public class ExcelToJson : MonoBehaviour
{
    [SerializeField] private string _excelFileName;
    [SerializeField] private string _jsonFileName;
    [SerializeField] private string _dataName;

    public void ConvertExcelToJson()
    {
        string excelPath = Path.Combine(Application.dataPath + "/06 Data/ExcelData/" + _excelFileName + ".xlsx");

        string jsonPath = Path.Combine(Application.dataPath + "/Resources/JsonData/" + _jsonFileName + ".json");

        ExcelToJsonConverter.ConvertExcelToJson(excelPath, jsonPath, _dataName);
    }
}
