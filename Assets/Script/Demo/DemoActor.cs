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
    [SerializeField] private int hp = 10000;
    [SerializeField] private int speed = 30;
    [SerializeField] private int attack = 100;
    [SerializeField] private int armor = 10;
    [SerializeField] private bool isDead;

    [SerializeField] public SkillTargetType targetType = SkillTargetType.Hero;
    [SerializeField] public SkillTargetTeamType targetTeamType = SkillTargetTeamType.Enemy;

    private int maxHp;
    void Start()
    {
        maxHp = hp;
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

    public bool IsDead()
    {
        return isDead;
    }

    public bool CheckManaValid(int skillCost)
    {
        return true;
    }

    public int GetIdentifyId()
    {
        return contextId;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool Damage(ISkill skill, int value)
    {
        SkillUtil.Log($" {GetBaseDes()}   damage -> {value}");
        Internal_Damage(skill, value);
        return true;
    }

    public bool Heal(ISkill skill, int value)
    {
        SkillUtil.Log($" {GetBaseDes()}   Heal -> {value} ");
        Internal_Heal(skill, value);
        return true;
    }

    public bool AddBuffEffect(string effectName, string dummyPoint)
    {
        return true;
    }

    public bool RemoveBuffEffect(string effectName, string dummyPoint)
    {
        return true;
    }

    public void ModifySpeed(int value)
    {
        if (value == 0) return;
        var old = speed;
        speed += value;
        EnqeueLog($"Speed From {old}->{speed} <color=\"red\">[Change]->{Math.Abs(value)}  ");
        SkillUtil.Log($" {GetBaseDes()}   Speed -> {value} ");
    }

    public void ModifyAttack(int value)
    {
        if (value == 0) return;
        var old = attack;
        attack += value;
        EnqeueLog($"Speed From {old}->{attack} [Change]->{Math.Abs(value)}  ");
        SkillUtil.Log($" {GetBaseDes()}   Attack -> {value} ");
    }

    public void ModifyArmor(int value)
    {
        if (value == 0) return;
        var old = armor;
        armor += value;
        EnqeueLog($"Speed From {old}->{armor} [Change]->{Math.Abs(value)}  ");
        SkillUtil.Log($" {GetBaseDes()}   Armor -> {value} ");
    }

    public void AddState(ActorSkillState state)
    {
        EnqeueLog($"Add State -> {state}  ");
        SkillUtil.LogWarning(GetBaseDes() + "  状态添加 ->" + state);
    }

    public void RemoveState(ActorSkillState state)
    {
        EnqeueLog($"remove State->{state} ");
        SkillUtil.LogWarning(GetBaseDes() + "  状态移除 ->" + state);
    }

    void Internal_Damage(ISkill skill, int value)
    {
        var data = skill.GetData();
        if (data != null && (SkillType)data.SkillType == SkillType.NormalAttack)
        {
            GetSkillContext().OtherNormalAttackActor();
            skill.GetContext().ActorNormalAttackOther();
        }

        GetSkillContext().OtherHurtActor();
        skill.GetContext().ActorHurtOther();

        UpdateHp(-value);
        if (hp <= 0)
            PlayerDead(skill);
    }

    void Internal_Heal(ISkill skill, int value)
    {
        UpdateHp(value);
    }

    private void UpdateHp(int value)
    {
        hp += value;
        if (hp <= 0)hp = 0;
        txt_hp.text = hp.ToString();
        img_hp.fillAmount = (float)hp / maxHp;
        var color = value > 0 ? "<color=\"green\">" : "<color=\"red\">";
        EnqeueLog($"{color}hp {value}");
      
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

    public SkillTargetTeamType GetTargetTeamType()
    {
        return targetTeamType;
    }

    public SkillTargetType GetTargetType()
    {
        return targetType;
    }

    public SkillTargetFlag GetTargetFlag()
    {
        return SkillTargetFlag.MagicImmune;
    }
    
    #region Debug
    private TextMeshPro text;
    private Queue<string> logQueue;

    void InitDebug()
    {
        logQueue = new Queue<string>();
        text = GetComponentInChildren<TextMeshPro>();
        text.color = Color.black;
        text.text=String.Empty;
        var rd = GetComponent<Renderer>();                                     
        if (rd == null)
            return;
        var color = targetTeamType == SkillTargetTeamType.Enemy ? Color.red : Color.green;
        rd.material.color = color;

        StartCoroutine(UpdateLog());
    }

    void EnqeueLog(string log)
    {
        logQueue?.Enqueue(log);
    }

    IEnumerator UpdateLog()
    {
        var waitAppear=new WaitForSecondsRealtime(0.15f);
        var waitDisappear=new WaitForSecondsRealtime(0.5f);
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