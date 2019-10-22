using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using BluePro;

[Serializable]
public class BaseTable : ScriptableObject //, ITable
{
    private int initId = -1;
    protected List<BaseRow> m_BaseRowList;

    public virtual List<BaseRow> GetList()
    {
        return null;
    }

    public virtual BaseRow GetRow(int id)
    {
        if (id < 0)
        {
            Debuger.LogError("id 为负数");
            return null;
        }

        var datas = GetList();

        if (datas == null)
        {
            Debuger.LogError("表尚未初始化");
            return null;
        }

        if (initId == -1)
            initId = datas[0].GetId();

        int index = id - initId;
        if (initId >= 0
            &&
            index < datas.Count
            &&
            datas[index].GetId() == id)
        {
            return datas[index];
        }
        else
        {
            //乱序的 todo 待优化
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].GetId() == id)
                    return datas[i];
            }
        }


        Debuger.LogError(string.Format("Can't find id {0}  InitId->{1}  Total Count->{2}",
            id, initId, datas.Count));
        return null;
    }

    protected virtual void AfterDeserialize()
    {
    }

    protected virtual BaseRow CreateRow()
    {
        return null;
    }


    protected List<BaseRow> datas;


    public virtual void DeSerialize(string[] names, string[] types, string[] lines)
    {
        datas = new List<BaseRow>();
        for (int i = 0; i < lines.Length; i++)
        {
            var config = CreateRow();
            string[] values = lines[i].Split(',');
            DeserializePerRow(config, names, types, values);
            datas.Add(config);
        }

        AfterDeserialize();
    }

    void DeserializePerRow(BaseRow config, string[] names, string[] types, string[] values)
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