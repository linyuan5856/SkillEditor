using System;
using System.Collections;
using System.Collections.Generic;
using BluePro.Skill;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649
public class DemoActor : MonoBehaviour, ISkillActor
{
    private Animator _anim;
    private SkillComponent skillComponent;
    private Image img_hp;
    private Text txt_hp;

    [SerializeField] private int contextId = 1;
    [SerializeField] private bool isDead;
    [SerializeField] public ESkillTargetType targetType = ESkillTargetType.Hero;
    [SerializeField] public ESkillTargetTeamType targetTeamType = ESkillTargetTeamType.Enemy;

    private readonly Dictionary<ESkillProp, int> propDic = new()
    {
        { ESkillProp.Attack, 100 },
        { ESkillProp.Hp, 10000 },
        { ESkillProp.Speed, 30 },
        { ESkillProp.Armor, 10 }
    };

    void Start()
    {
        img_hp = transform.Find("Canvas/img_hp").GetComponent<Image>();
        txt_hp = img_hp.GetComponentInChildren<Text>();
        UpdateHp(0);
        img_hp.type = Image.Type.Filled;
        img_hp.fillMethod = Image.FillMethod.Horizontal;

        _anim = GetComponent<Animator>();
        if (!_anim)
            _anim = gameObject.AddComponent<Animator>();

        //实例化和Init要分开步骤
        skillComponent = new SkillComponent(this);
        skillComponent.Init();

        InitDebug();
    }

    public void CastSkill(int index, CommonParam param)
    {
        EnqeueLog("Cast Skill ");
        skillComponent?.CastSkill(index, param);
    }

    public void CastSkillById(int id, CommonParam param)
    {
        EnqeueLog("Cast Skill ");
        skillComponent?.CastSkillById(id, param);
    }

    public ISkillContext GetSkillContext()
    {
        return skillComponent;
    }

    public bool PlayAnimation(string anim)
    {
        if (_anim)
        {
            _anim.Play(anim);
            return true;
        }

        return false;
    }

    bool ISkillActor.IsDead()
    {
        return isDead;
    }

    bool ISkillActor.CheckManaValid(int skillCost)
    {
        return true;
    }


    int ISkillActor.GetIdentifyId()
    {
        return contextId;
    }

    Transform ISkillActor.GetTransform()
    {
        return transform;
    }

    void ISkillActor.ModifyProp(ESkillProp prop, ISkill skill, int change)
    {
        if (prop != ESkillProp.Hp) return;

        if (change < 0)
        {
            var data = skill.GetData();
            if (data != null && (ESkillType)data.SkillType == ESkillType.NormalAttack)
            {
                GetSkillContext().OtherNormalAttackActor();
                skill.GetContext().ActorNormalAttackOther();
            }

            GetSkillContext().OtherHurtActor();
            skill.GetContext().ActorHurtOther();
        }

        var newValue = UpdateHp(change);
        if (newValue <= 0) PlayerDead(skill);
    }


    bool ISkillActor.AddBuffEffect(string effectName, string dummyPoint)
    {
        return true;
    }

    bool ISkillActor.RemoveBuffEffect(string effectName, string dummyPoint)
    {
        return true;
    }

    void ISkillActor.ModifyProp(ESkillProp prop, int value)
    {
        ModifyProp(prop, value);
    }

    private int ModifyProp(ESkillProp prop, int change)
    {
        if (change == 0 || !propDic.TryGetValue(prop, out var cur)) return -1;
        var newValue = cur + change;
        if (newValue < 0) newValue = 0;
        propDic[prop] = newValue;
        EnqeueLog($"{prop}:   {cur}->{newValue}");
        SkillUtil.Log($" {GetBaseDes()}   Speed -> {change} ");
        return newValue;
    }

    private int UpdateHp(int change)
    {
        int cur = ModifyProp(ESkillProp.Hp, change);
        txt_hp.text = cur.ToString();
        img_hp.fillAmount = (float)cur / 10000;
        return cur;
    }

    public void AddState(EActorSkillState state)
    {
        EnqeueLog($"Add State -> {state}  ");
        SkillUtil.LogWarning(GetBaseDes() + "  状态添加 ->" + state);
    }

    public void RemoveState(EActorSkillState state)
    {
        EnqeueLog($"remove State->{state} ");
        SkillUtil.LogWarning(GetBaseDes() + "  状态移除 ->" + state);
    }


    void PlayerDead(ISkill skill)
    {
        isDead = true;
        GetSkillContext().ActorBeKilled();
        skill.GetContext().ActorKilledOther();
        GetSkillContext()?.ClearAllBuff();
    }


    private string GetBaseDes()
    {
        return $"[ContextId {contextId} Name {gameObject.name}  ] ";
    }

    public ESkillTargetTeamType GetTargetTeamType()
    {
        return targetTeamType;
    }

    public ESkillTargetType GetTargetType()
    {
        return targetType;
    }

    public ESkillTargetFlag GetTargetFlag()
    {
        return ESkillTargetFlag.MagicImmune;
    }

    #region Debug

    private TextMeshPro text;
    private Queue<string> logQueue;

    void InitDebug()
    {
        logQueue = new Queue<string>();
        text = GetComponentInChildren<TextMeshPro>();
        text.color = Color.black;
        text.text = String.Empty;
        var rd = GetComponent<Renderer>();
        if (rd == null)
            return;
        var color = targetTeamType == ESkillTargetTeamType.Enemy ? Color.red : Color.green;
        rd.material.color = color;

        StartCoroutine(UpdateLog());
    }

    void EnqeueLog(string log)
    {
        logQueue?.Enqueue(log);
    }

    IEnumerator UpdateLog()
    {
        var waitAppear = new WaitForSecondsRealtime(0.15f);
        var waitDisappear = new WaitForSecondsRealtime(0.5f);
        while (true)
        {
            if (logQueue.Count > 0)
            {
                text.text = logQueue.Dequeue();
                yield return waitDisappear;
                text.text = string.Empty;
                yield return waitAppear;
            }
            else
                yield return null;
        }
    }

    #endregion
}