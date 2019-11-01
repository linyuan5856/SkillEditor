using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using BluePro;


public interface IBaseTable
{
    void DeSerialize(string[] names, string[] types, string[] lines);

    IBaseRow GetRow(int id);
}

[Serializable]
public class BaseTable<T> : ScriptableObject, IBaseTable where T : class, IBaseRow
{
    private int initId = -1;
    [SerializeField] public List<T> Datas;
    
    public virtual IBaseRow GetRow(int id)
    {
        if (id < 0)
        {
            Debuger.LogError("id 为负数");
            return null;
        }

        if (Datas == null)
        {
            Debuger.LogError("表尚未初始化");
            return null;
        }

        if (initId == -1)
            initId = Datas[0].GetId();

        int index = id - initId;
        if (initId >= 0
            &&
            index < Datas.Count
            &&
            Datas[index].GetId() == id)
        {
            return Datas[index];
        }
        else
        {
            //乱序的 todo 待优化
            for (int i = 0; i < Datas.Count; i++)
            {
                if (Datas[i].GetId() == id)
                    return Datas[i];
            }
        }


        Debuger.LogError(string.Format("Can't find id {0}  InitId->{1}  Total Count->{2}",
            id, initId, Datas.Count));
        return null;
    }


    public void DeSerialize(string[] names, string[] types, string[] lines)
    {
        Datas = new List<T>();
        for (int i = 0; i < lines.Length; i++)
        {
            var config = Activator.CreateInstance<T>();
            string[] values = lines[i].Split(',');
            DeserializePerRow(config, names, types, values);
            Datas.Add(config);
        }
    }

    void DeserializePerRow(IBaseRow config, string[] names, string[] types, string[] values)
    {
        for (int i = 0; i < names.Length; i++)
        {
            var key = names[i];
            var targets = config.GetType().GetFields(BindingFlags.IgnoreCase |
                                                     BindingFlags.NonPublic | BindingFlags.Instance);

            if (targets.Length != values.Length)
            {
                Debuger.LogError(string.Format("脚本和配置表参数总数不匹配 脚本[{0}] 配置[{1}]",
                    targets.Length, values.Length));
            }

            FieldInfo fieldInfo = config.GetType().GetField(key,
                BindingFlags.IgnoreCase | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                Debuger.LogWarning("cannot find property:" + key);
                continue;
            }

            ConvertUtil.SetFieldValue(fieldInfo, config, values[i]);
        }
    }
}