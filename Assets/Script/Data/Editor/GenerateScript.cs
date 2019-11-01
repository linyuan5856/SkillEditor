using System.IO;
using System.Text;
using BluePro.Utils;
using UnityEditor;
using UnityEngine;

public class GenerateScript
{
    private const string ScriptTemplate = @"//This is Generate Auto By GenerateScript.cs
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[System.Serializable]
public class {0} : BaseRow
{
    {1}
    {2}

    public override int GetId()
    {
        return this.id;
    }
}

";

    private const string TableTemplate = @"//This is Generate Auto By GenerateScript.cs
public partial class {0} : BaseTable<{1}>
{
}";


    [MenuItem("Tools/Config/GenerateScript", false, 101)]
    static void GenerateConfigScript()
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

            var csvs = File.ReadAllLines(path);
            string[] fieldNames = csvs[0].Split(',');
            string[] fieldTypes = csvs[1].Split(',');

            var scriptName = GenerateDefine.GetDataScriptNameByCsv(csvName);
            var str = GenerateDataScript(scriptName, fieldNames, fieldTypes);
            FileUtils.SaveFile(GenerateDefine.ConfigScriptOutPutPath + scriptName + ".cs", str);

            var tableName = GenerateDefine.GetTableScriptNameByCsv(csvName);
            str = GenerateTableScript(scriptName, tableName);
            FileUtils.SaveFile(GenerateDefine.ConfigTableScriptOutPutPath + tableName + ".cs", str);

            break;
        }

        AssetDatabase.Refresh();
    }

    static string GenerateDataScript(string scriptName, string[] fieldNames, string[] fieldTypes)
    {
        StringBuilder privateType = new StringBuilder();
        privateType.AppendLine();
        StringBuilder publicProperty = new StringBuilder();
        publicProperty.AppendLine();

        for (int i = 0; i < fieldNames.Length; i++)
        {
            var typeName = GenerateDefine.GetDesByFieldType(fieldTypes[i]);
            privateType.AppendFormat($"    [SerializeField] private {typeName} {fieldNames[i]};");
            privateType.AppendLine();

            var upperName = GenerateDefine.ToFirstLetterUpper(fieldNames[i]);
            publicProperty.AppendFormat($"    public {typeName} {upperName} => {fieldNames[i]};");
            publicProperty.AppendLine();
        }

        string str = ScriptTemplate;
        str = str.Replace("{0}", scriptName);
        str = str.Replace("{1}", privateType.ToString());
        str = str.Replace("{2}", publicProperty.ToString());
        return str;
    }

    static string GenerateTableScript(string scriptName, string tableName)
    {
        string str = TableTemplate;
        str = str.Replace("{0}", tableName);
        str = str.Replace("{1}", scriptName);
        return str;
    }
}