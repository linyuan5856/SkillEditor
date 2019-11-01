#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public class GenerateConfig
{
    [MenuItem("Tools/Config/GenerateConfig", false, 102)]
    static void GenerateProjectConfig()
    {
        if (EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("Warning", "Wait for compiling end",
                "Confirm");
            return;
        }

        foreach (var csvName in GenerateDefine.csvList)
        {
            var path = GenerateDefine.GetCsvPath() + csvName + ".csv";

            if (!File.Exists(path))
            {
                Debug.LogError(path + "   is not exist");
                return;
            }

            CreateScriptObject(csvName, GenerateDefine.GetTableScriptNameByCsv(csvName), File.ReadAllLines(path));
        }
    }

    static void CreateScriptObject(string csvName, string scriptObjectName, string[] rows, bool isAll = true)
    {
        var outPutPath = GenerateDefine.ScriptObjectOutPutPath;
        if (!Directory.Exists(outPutPath))
            Directory.CreateDirectory(outPutPath);

        string path = outPutPath + scriptObjectName + ".asset";
        if (File.Exists(path))
        {
            AssetDatabase.DeleteAsset(path);
            Debug.Log("has delete old asset  ->" + csvName);
        }

        var asset = ScriptableObject.CreateInstance(scriptObjectName);
        var success = DeSerializeAsset(rows, ref asset);

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.Refresh();
        if (!isAll)
            Selection.activeObject = asset;
        var des = success ? "Done " : "Failed ";
        Debug.Log(string.Format("GenerateConfig {0}  [ScriptName->{1}]", csvName + " " + des,
            scriptObjectName));
    }

    static bool DeSerializeAsset(string[] rows, ref ScriptableObject obj)
    {
        IBaseTable table = (IBaseTable) obj;
        if (table == null)
        {
            Debug.LogError("it's not a BaseTable Type");
            return false;
        }

        string[] lines = new string[rows.Length - 2];

        int index = 0;
        for (int i = 2; i < rows.Length; i++)
        {
            lines[index] = rows[i];
            index++;
        }

        string[] name = rows[0].Split(',');
        string[] type = rows[1].Split(',');
        table.DeSerialize(name, type, lines);
        return true;
    }
}
#endif