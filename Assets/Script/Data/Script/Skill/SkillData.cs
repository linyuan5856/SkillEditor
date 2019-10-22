using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[System.Serializable]
public class SkillData : BaseRow
{
    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] private string icon;
    [SerializeField] private string description;
    [SerializeField] private int type;
    [SerializeField] private List<int> behavior;
    [SerializeField] private int castRange;
    [SerializeField] private KVIntMap AOERange;
    [SerializeField] private string offsetpara;//todo
    [SerializeField] private int damageType;
    [SerializeField] private List<int> skillCost;
    [SerializeField] private List<float> coolDown;
    [SerializeField] private string castAnimation;
    [SerializeField] private List<float> castPoint;
    [SerializeField] private float channelTime;
    [SerializeField] private int targetTeam;
    [SerializeField] private List<int> targetType;
    [SerializeField] private int targetFlags;
    [SerializeField] private List<KVIntMap> action;


    public int Id => id;

    public string Name => name;

    public string Icon => icon;

    public string Description => description;

    public int SkillType => type;

    public List<int> SkillBehavior => behavior;

    public int CastRange => castRange;

    public KVIntMap AoeRange => AOERange;

    public int DamageType => damageType;

    public List<int> SkillCost => skillCost;

    public List<float> CoolDown => coolDown;

    public string SkillCastAnimation => castAnimation;

    public List<float> SkillCastPoint => castPoint;

    public float SkillChannelTime => channelTime;

    public int TargetTeam => targetTeam;

    public List<int> TargetType => targetType;

    public int TargetFlags => targetFlags;

    public List<KVIntMap> Action => action;

    public override int GetId()
    {
        return this.id;
    }
}