using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExcelToJson))]
public class ExcelToJsonButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("½ÇÇà"))
        {
            ExcelToJson targetScript = (ExcelToJson)target;
            targetScript.ConvertExcelToJson();

            AssetDatabase.Refresh();
        }
    }
}
