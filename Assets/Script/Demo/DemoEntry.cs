using System.Collections.Generic;
using BluePro;
using BluePro.Skill;
using UnityEngine;

public class DemoEntry : BaseMain
{
    [SerializeField] private Ui_SkillPanel _ui;
    [SerializeField] private DemoActor source;
    [SerializeField] private DemoActor[] targets;
    [SerializeField] private DemoActor currentTarget;
    public List<int> SkillList = new() { 1001, 1002, 1003, 2001, 3001, 4001, 5001 };

    private static DemoEntry _instance;
    public static DemoEntry Instance => _instance;

    protected override void Init()
    {
        base.Init();
        Debuger.Init(null,new UnityDebugerConsole());
        _instance = this;
        targets = FindObjectsOfType<DemoActor>();
        _ui.gameObject.SetActive(true);
    }

    public void CastSkill(SkillData data)
    {
        if (!source || targets == null || targets[0] == null) return;
        currentTarget = GetRandomTarget();
        var param = new CommonParam(currentTarget);
        source.CastSkillById(data.Id, param);
    }

    public void ClearAllBuff()
    {
        foreach (var target in targets)
            target.GetSkillContext()?.ClearAllBuff();
    }

    DemoActor GetRandomTarget()
    {
        var target = targets[Random.Range(0, targets.Length)];
        if (target != source && target.targetTeamType != ESkillTargetTeamType.Friend) return target;
        return GetRandomTarget();
    }
}