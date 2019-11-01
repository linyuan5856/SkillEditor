using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GenerateDefine
{
    private static readonly string PROJECT_NAME = "Client2019.2";
    private static readonly string CSV_PATH = "Share/Config/csv/client/";
    private static string mCsvPath;


    public static readonly string ScriptObjectOutPutPath = "Assets/Resources/Config/";
    public static readonly string ConfigScriptOutPutPath = "Assets/Script/Data/Script/";
    public static readonly string ConfigTableScriptOutPutPath = "Assets/Script/Data/Script/Table/";


    public static readonly List<string> csvList = new List<string>
    {
        //csv Name    
        "Skill",
        "SkillAction",
        "SkillBuff",
        "sundry",
        "npc",
        "model",
        "matchmode",
        "effect",
        "motion",
    };


    public static string GetCsvPath()
    {
        if (string.IsNullOrEmpty(mCsvPath))
        {
            var endIndex = Application.dataPath.IndexOf(PROJECT_NAME, StringComparison.Ordinal);
            var headPath = Application.dataPath.Substring(0, endIndex);
            mCsvPath = headPath + CSV_PATH;
        }

        return mCsvPath;
    }

    public static string GetDesByFieldType(string fieldType)
    {
        var type = fieldType.ToLower();
        string result = string.Empty;
        switch (type)
        {
            case "string":
                result = "string";
                break;
            case "int":
                result = "int";
                break;
            case "float":
                result = "float";
                break;
            case "intkv":
                result = "KVIntMap";
                break;
            case "intkvlist":
                result = "List<KVIntMap>";
                break;
            case "intlist":
                result = "List<int>";
                break;
            case "floatlist":
                result = "List<float>";
                break;
            case "vector3":
                result = "Vector3";
                break;
            case "long":
                result = "long";
                break;
            case "bool":
                result = "bool";
                break;
        }

        return result;
    }

    public static string GetDataScriptNameByCsv(string csvName)
    {
        return "_" + ToFirstLetterUpper(csvName) + "Data";
    }

    public static string GetTableScriptNameByCsv(string csvName)
    {
        return GetDataScriptNameByCsv(csvName) + "Table";
    }

    public static string ToFirstLetterUpper(string str)
    {
        return str.First().ToString().ToUpper() + str.Substring(1);
    }
}