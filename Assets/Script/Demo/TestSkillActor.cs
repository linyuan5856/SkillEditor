using System;
using System.Collections;
using System.Collections.Generic;
using BluePro.Skill;
using TMPro;
using UnityEngine;

#pragma warning disable 0649
public class TestSkillActor : MonoBehaviour, ISkillActor
{
    private Animator _anim;
    private SkillComponent skillComponent;

    [SerializeField] private int contextId = 1;

    [SerializeField] private int hp = 10000;

    // [SerializeField] private int mana = 120;
    [SerializeField] private int speed = 30;
    [SerializeField] private int attack = 100;
    [SerializeField] private int armor = 10;
    [SerializeField] private bool isDead;

    [SerializeField] public SkillTargetType targetType = SkillTargetType.Hero;
    [SerializeField] public SkillTargetTeamType targetTeamType = SkillTargetTeamType.Enemy;

    #region Debug

    private TextMeshPro text;
    private Queue<string> logQueue;
    private string oldLog;

    private float idleTime;
    //private float showTime;
    //private bool canDequeue = true;

    void InitDebug()
    {
        this.logQueue = new Queue<string>();
        this.text = this.GetComponentInChildren<TextMeshPro>();
        this.text.color = Color.black;
        var rd = this.GetComponent<Renderer>();
        if (rd == null)
            return;
        var color = this.targetTeamType == SkillTargetTeamType.Enemy ? Color.red : Color.green;
        rd.material.color = color;

        StartCoroutine(UpdateLog());
    }

    void EnqeueLog(string log)
    {
        this.logQueue?.Enqueue(log);
    }

    IEnumerator UpdateLog()
    {
        while (true)
        {
            string log = oldLog;
            if (logQueue.Count > 0)
                log = this.logQueue.Dequeue();
            if (!string.Equals(log, oldLog))
            {
                idleTime = 0;
            }

            idleTime += Time.deltaTime;
            this.text.text = log;
            oldLog = log;
            if (idleTime > 0.08)
                this.text.text = string.Empty;
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    #endregion

    void Start()
    {
        this._anim = this.GetComponent<Animator>();
        if (!this._anim)
            this._anim = this.gameObject.AddComponent<Animator>();

        //实例化和Init要分开步骤
        this.skillComponent = new SkillComponent(this);
        this.skillComponent.Init();

        // this.InitDebug();
    }


    public void CastSkill(int index, CommonParam param)
    {
        EnqeueLog("Cast Skill ");
        skillComponent?.CastSkill(index, param);
    }

    public void CastSkillById(int id,CommonParam param)
    {
        EnqeueLog("Cast Skill ");
        skillComponent?.CastSkillById(id,param);
    }

    public ISkillContext GetSkillContext()
    {
        return this.skillComponent;
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
        return this.transform;
    }

    public bool Damage(ISkill skill, int value)
    {
        SkillUtil.Log(string.Format(" {0}   -damage -> {1}", GetBaseDes(), value));
        this.Internal_Damage(skill, value);
        return true;
    }

    public bool Heal(ISkill skill, int value)
    {
        SkillUtil.Log(string.Format(" {0}   Heal -> {1} ", GetBaseDes(), value));
        this.Internal_Heal(skill, value);
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
        if (value == 0)
            return;
        var old = this.speed;
        this.speed += value;
        EnqeueLog(string.Format("Speed From {0}->{1} [Change]->{2}  ", old, this.speed, Math.Abs(value)));
        SkillUtil.Log(string.Format(" {0}   Speed -> {1} ", GetBaseDes(), value));
    }

    public void ModifyAttack(int value)
    {
        if (value == 0)
            return;
        var old = this.attack;
        this.attack += value;
        EnqeueLog(string.Format("Speed From {0}->{1} [Change]->{2}  ", old, this.attack, Math.Abs(value)));
        SkillUtil.Log(string.Format(" {0}   Attack -> {1} ", GetBaseDes(), value));
    }

    public void ModifyArmor(int value)
    {
        if (value == 0)
            return;
        var old = this.armor;
        this.armor += value;
        EnqeueLog(string.Format("Speed From {0}->{1} [Change]->{2}  ", old, this.armor, Math.Abs(value)));
        SkillUtil.Log(string.Format(" {0}   Armor -> {1} ", GetBaseDes(), value));
    }

    public void AddState(ActorSkillState state)
    {
        EnqeueLog(string.Format("Add State -> {0}  ", state));
        SkillUtil.LogWarning(GetBaseDes() + "  状态添加 ->" + state);
    }

    public void RemoveState(ActorSkillState state)
    {
        EnqeueLog(string.Format("remove State->{0} ", state));
        SkillUtil.LogWarning(GetBaseDes() + "  状态移除 ->" + state);
    }

    void Internal_Damage(ISkill skill, int value)
    {
        var data = skill.GetData();
        if (data != null && (SkillType) data.SkillType == SkillType.NormalAttack)
        {
            this.GetSkillContext().OtherNormalAttackActor();
            skill.GetContext().ActorNormalAttackOther();
        }

        this.GetSkillContext().OtherHurtActor();
        skill.GetContext().ActorHurtOther();

        this.hp -= value;
        if (this.hp <= 0)
        {
            EnqeueLog(string.Format(" damage -> {0} Player Dead", value));
            this.PlayerDead(skill);
        }
        else
            EnqeueLog(string.Format("damage -> {0} Hp->{1} ", value, hp));
    }

    void Internal_Heal(ISkill skill, int value)
    {
        this.hp += value;
    }

    void PlayerDead(ISkill skill)
    {
        isDead = true;
        this.GetSkillContext().ActorBeKilled();
        skill.GetContext().ActorKilledOther();
        this.GetSkillContext()?.ClearAllBuff();
    }


    private string GetBaseDes()
    {
        return string.Format("[ContextId {0} Name {1}  ] ", contextId, this.gameObject.name);
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
}