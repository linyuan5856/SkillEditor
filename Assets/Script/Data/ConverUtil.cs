using System;
using System.Collections.Generic;
using System.Reflection;
using BluePro;
using UnityEngine;

public class ConvertUtil
{
    private const string NONE = "|";

    public static List<int> ConvertToIntList(string value, char splite = '|')
    {
        if (string.IsNullOrEmpty(value) || value.Equals(NONE)) return null;

        string[] arr = value.Split(splite);

        List<int> list = new List<int>();
        for (int i = 0; i < arr.Length; i++)
        {
            list.Add(int.Parse(arr[i]));
        }

        return list;
    }

    public static List<float> ConvertToFloatList(string value, char splite = '|')
    {
        if (string.IsNullOrEmpty(value) || value.Equals(NONE)) return null;

        string[] arr = value.Split(splite);

        List<float> list = new List<float>();
        for (int i = 0; i < arr.Length; i++)
        {
            list.Add(float.Parse(arr[i]));
        }

        return list;
    }


    public static string[] ConvertToArrary(string value, char splite)
    {
        string[] arr = value.Split(splite);
        return arr;
    }

    public static KVIntMap ConvertToMap(string value, char splite = '-')
    {
        if (string.IsNullOrEmpty(value) || value.Equals(NONE)) return default;

        string[] arr = value.Split(splite);
        var value0 = int.Parse(arr[0]);
        var value1 = arr.Length == 1 ? 0 : int.Parse(arr[1]);
        KVIntMap map = new KVIntMap(value0, value1);
        return map;
    }

    public static List<KVIntMap> ConvertToMapList(string value, char splite = '|')
    {
        if (string.IsNullOrEmpty(value) || value.Equals(NONE)) return default;

        List<KVIntMap> list = new List<KVIntMap>();
        string[] arr = value.Split(splite);
        for (int i = 0; i < arr.Length; i++)
        {
            var map = ConvertToMap(arr[i]);
            list.Add(map);
        }

        return list;
    }

    public static bool SetFieldValue(FieldInfo fieldInfo, System.Object target, string value)
    {
        try
        {
            if (fieldInfo.FieldType == typeof(int))
            {
                if (string.IsNullOrEmpty(value))
                {
                    fieldInfo.SetValue(target, 0);
                }
                else
                {
                    try
                    {
                        fieldInfo.SetValue(target, int.Parse(value));
                    }
                    catch (Exception)
                    {
                        float tValue = float.Parse(value);
                        fieldInfo.SetValue(target, Mathf.RoundToInt(tValue));
                    }
                }
            }
            else if (fieldInfo.FieldType == typeof(string))
            {
                fieldInfo.SetValue(target, value);
            }
            else if (fieldInfo.FieldType == typeof(float))
            {
                fieldInfo.SetValue(target, float.Parse(value));
            }
            else if (fieldInfo.FieldType == typeof(bool))
            {
                fieldInfo.SetValue(target, (value == "true" || value == "1") ? true : false);
            }
            else if (fieldInfo.FieldType == typeof(long))
            {
                fieldInfo.SetValue(target, long.Parse(value));
            }
            else if (fieldInfo.FieldType == typeof(List<int>))
            {
                fieldInfo.SetValue(target, ConvertToIntList(value));
            }
            else if (fieldInfo.FieldType == typeof(List<float>))
            {
                fieldInfo.SetValue(target, ConvertToFloatList(value));
            }
            else if (fieldInfo.FieldType == typeof(KVIntMap))
            {
                fieldInfo.SetValue(target, ConvertToMap(value));
            }
            else if (fieldInfo.FieldType == typeof(List<KVIntMap>))
            {
                fieldInfo.SetValue(target, ConvertToMapList(value));
            }
            else
            {
                Debuger.LogError("field type not supported:" + target.GetType().ToString() + " " + fieldInfo.FieldType);
                return false;
            }
        }
        catch (Exception e)
        {
            Debuger.LogError(string.Format("field setValue failed:{0},{1},{2},{3},", target.GetType(),
                fieldInfo.FieldType, fieldInfo.Name, value));
            Debuger.LogError(e.Message);
            return false;
        }

        return true;
    }
}