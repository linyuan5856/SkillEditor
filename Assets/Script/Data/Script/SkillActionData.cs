using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[System.Serializable]
public class SkillActionData : BaseRow
{
    [SerializeField] private int id;
    [SerializeField] private int center;
    [SerializeField] private KVIntMap radius;
    [SerializeField] private string offsetpara;//todo
    [SerializeField] private int team;
    [SerializeField] private List<int> type;
    [SerializeField] private int flags;
    [SerializeField] private int actionType;
    [SerializeField] private string para1;
    [SerializeField] private string para2;
    [SerializeField] private string para3;
    [SerializeField] private string para4;
    [SerializeField] private string para5;
    [SerializeField] private string para6;
    [SerializeField] private string para7;

    public int Id => id;

    public int Center => center;

    public KVIntMap Radius => radius;

    public int Team => team;

    public List<int> Type => type;

    public int Flags => flags;

    public int ActionType => actionType;

    public string Para1 => para1;

    public string Para2 => para2;

    public string Para3 => para3;

    public string Para4 => para4;

    public string Para5 => para5;

    public string Para6 => para6;

    public string Para7 => para7;

    public override int GetId()
    {
        return id;
    }
}