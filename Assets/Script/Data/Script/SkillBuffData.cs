using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
[System.Serializable]
public class SkillBuffData : BaseRow
{
    [SerializeField] private int id;
    [SerializeField] private List<int> attributes;
    [SerializeField] private List<float> duration;
    [SerializeField] private bool isDeBuff;
    [SerializeField] private bool isHidden;
    [SerializeField] private bool isPurgable;
    [SerializeField] private bool passive;
    [SerializeField] private float thinkInterval;
    [SerializeField] private string icon;
    [SerializeField] private string effect;
    [SerializeField] private string attach;
    [SerializeField] private string overrideAnimation;
    [SerializeField] private List<KVIntMap> action;
    [SerializeField] private int state;
    [SerializeField] private string property_MoveSpeed;
    [SerializeField] private string property_BaseAttack;
    [SerializeField] private string property_armor;

    public override int GetId()
    {
        return id;
    }

    public int Id => id;

    public List<int> Attributes => attributes;

    public List<float> Duration => duration;

    public bool IsDeBuff => isDeBuff;

    public bool IsHidden => isHidden;

    public bool IsPurgable => isPurgable;

    public bool Passive => passive;

    public float ThinkInterval => thinkInterval;

    public string Icon => icon;

    public string Effect => effect;

    public string Attach => attach;

    public string OverrideAnimation => overrideAnimation;

    public List<KVIntMap> Action => action;

    public int State => state;

    public string PropertyMoveSpeed => property_MoveSpeed;

    public string PropertyBaseAttack => property_BaseAttack;

    public string PropertyArmor => property_armor;
}