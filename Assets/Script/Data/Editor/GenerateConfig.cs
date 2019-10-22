#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GenerateConfig : MonoBehaviour
{
    private static readonly string PROJECT_NAME = "Client2019.2";
    private static readonly string CSV_PATH = "Share/Config/csv/client/";
    private static readonly string OutPutPath = "Assets/Resources/Config/";

    private static readonly Dictionary<string, string> configDic = new Dictionary<string, string>
    {
        //csv Name    ScriptObject Name
        {"Skill", "SkillDataTable"},
        {"SkillAction", "SkillActionDataTable"},
        {"SkillBuff", "SkillBuffDataTable"},
        {"SkillState", "SkillStateTable"},
        {"sundry", "SundryDataTable"},
        {"npc", "NpcDataTable"},
        {"model", "ModelDataTable"},
    };

    private static string mCsvPath;

    static string GetCsvPath()
    {
        if (string.IsNullOrEmpty(mCsvPath))
        {
            var endIndex = Application.dataPath.IndexOf(PROJECT_NAME, StringComparison.Ordinal);
            var headPath = Application.dataPath.Substring(0, endIndex);
            mCsvPath = headPath + CSV_PATH;
        }

        return mCsvPath;
    }

    [MenuItem("BluePlanetPROTools/Config/GenerateConfig")]
    static void GenerateProjectConfig()
    {
        if (EditorApplication.isCompiling)
        {
            EditorUtility.DisplayDialog("Warning", "Wait for compiling end",
                "Confirm");
            return;
        }

        foreach (var kv in configDic)
        {
            var csvName = kv.Key;
            var path = GetCsvPath() + csvName + ".csv";

            if (!File.Exists(path))
            {
                Debug.LogError(path + "   is not exist");
                return;
            }

            CreateScriptObject(csvName, kv.Value, File.ReadAllLines(path));
        }
    }

    static void CreateScriptObject(string csvName, string scriptName, string[] rows, bool isAll = true)
    {
        if (!Directory.Exists(OutPutPath))
            Directory.CreateDirectory(OutPutPath);

        string path = OutPutPath + scriptName + ".asset";
        if (File.Exists(path))
        {
            AssetDatabase.DeleteAsset(path);
            Debug.Log("has delete old asset  ->" + csvName);
        }

        var asset = ScriptableObject.CreateInstance(scriptName);
        var success = DeSerializeAsset(rows, ref asset);

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.Refresh();
        if (!isAll)
            Selection.activeObject = asset;
        var des = success ? "Done " : "Failed ";
        Debug.Log(string.Format("GenerateConfig {0}  [ScriptName->{1}]", csvName + " " + des,
            scriptName));
    }

    static bool DeSerializeAsset(string[] rows, ref ScriptableObject obj)
    {
        BaseTable table = (BaseTable) obj;
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