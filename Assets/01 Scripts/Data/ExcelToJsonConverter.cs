using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Data;
using ExcelDataReader;
using Newtonsoft.Json;

public static class ExcelToJsonConverter
{
    public static void ConvertExcelToJson(string excelPath, string jsonPath)
    {
        // read excel file
        using FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read);
        using IExcelDataReader reader = CreateReader(stream, excelPath);
        if (reader == null)
        {
            Debug.LogError("only '.xlsx' file available");
            return;
        }

        string json = Convert(reader);

        File.WriteAllText(jsonPath, json);

        Debug.Log("Create json file From " + stream.Name);
    }

    private static IExcelDataReader CreateReader(FileStream stream, string path)
    {
        string extension = Path.GetExtension(path).ToLower();
        
        if (extension == ".xlsx")
            return ExcelReaderFactory.CreateOpenXmlReader(stream);

        return null;
    }

    private static string Convert(IExcelDataReader reader)
    {
        // Read excel file data
        DataSet dataSet = reader.AsDataSet();
        reader.Close();

        // Convert excel to json
        List<Dictionary<string, object>> jsonData = new();
        DataTable table = dataSet.Tables[0];

        for (int i = 3; i < table.Rows.Count; i++)
        {
            DataRow row = table.Rows[i];
            Dictionary<string, object> rowData = new();

            for (int j = 0; j < table.Columns.Count; ++j)
            {
                string key = table.Rows[1][j].ToString();
                object value = row[j];
                rowData[key] = value;
            }
            jsonData.Add(rowData);
        }
        string json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

        return json;
    }
}