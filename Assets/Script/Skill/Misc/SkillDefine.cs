namespace BluePro.Skill
{
    public class SkillDefine
    {
        public const int NONE = -9999;
        public const string SKILLTABLE = "SkillDataTable";
        public const string SKILL_ACTION_TABLE = "SkillActionDataTable";
        public const string SKILL_BUFF_TABLE = "SkillBuffDataTable";
        public const string SKILL_STATE_TABLE = "SkillStateDataTable";
    }

    public enum SkillType
    {
        NormalAttack,
        NormalSkill,
        FinalSkill,
        ItemSkill,
        BuildSkill,
    }

    public enum SkillBehaviourType
    {
        Point = 1,
        Direction,
        NoTarget,
        Passivity,
        SwitchState,
        Continuous,
        AreaOfEffect, //AOE
    }

    public enum SkillDamageType
    {
        Magic = 1,
        Physic,
        Holy
    }


    public enum SkillActionTriggerTime
    {
        StartCastSkill = 1,
        ContinueSkillSuccess,
        ContinueSkillBreak,
        ProjectileHit,
        ProjectileDisappear,
        StateOpen,
        StateClosed,
        ContextDead,
        ContextBorn,
        OwnedSkill
    }

    public enum SkillActionType
    {
        /*
          1-给目标添加buff
          2-删除目标的buff
          3-造成伤害
          4-回复生命
          5-造成眩晕
          6-线性投射物
          7-跟踪投射物
          8-闪烁
          9-召唤单位
          10-击退
          100-运行脚本
          101-概率执行Action
          102-延迟执行Action
          103-弹射多种Action
          201-创建特效
          202-创建声音
          203-执行动作
         */
        AddBuff = 1,
        RemoveBuff,
        Damage,
        Heal,
        Dizzy,
        LineProjectile,
        FollowProjectile,
        Blink,
        Summon, //召唤
        Knockback, //击退
        RunScript = 100,
        ProbabilityAction = 101,
        DelayAction = 102,
        BounceAction = 103,
        Effect = 201,
        Audio = 202,
        Action = 203,
    }

    public enum SkillTargetTeamType
    {
        Enemy = 1,
        Friend,
        ALl
    }

    public enum SkillTargetType
    {
        None = 1,
        All,
        Hero,
        Build,
        Monster,
        Boss
    }

    public enum SkillTargetFlag
    {
        MagicImmune = 1,
        Invincible,
    }

    public enum SkillTargetRangeType
    {
        None,
        Circle,
        Rect,
        Sector,
    }

    public enum SkillTargetCenter
    {
        Caster = 1,
        Target,
        Point,
        Attacker,
        Projectile,
        ProjectileHitPoint,
    }

    public enum BuffActionType
    {
        /*
         30-循环执行定时器
         31-拥有者普攻别人
         32-拥有者受到普攻
         33-拥有者受到伤害
         34-拥有者伤害别人
         35-当本buff被移除
         36-当本buff被创建
         37-当拥有者死亡时
         38-拥有者杀死别人
    */
        LoopTimer = 30,
        OwnerAttackOther = 31,
        OwnerBeAttacked = 32,
        OwnerBeHurt = 33,
        OwnerHurtOther = 34,
        RemoveBuff = 35,
        AddBuff = 36,
        OwnerBeKilled = 37,
        OwnerKillOther = 38,
    }

    public enum BuffAttributes
    {
        Normal=1,//正常
        CanAdd=2,//可叠加
        IgnoreInvincible=3,//忽视无敌
    }

    public enum ActorSkillState
    {
        None=0,
        PhysicalImmune, //物理免疫
        MagicImmune, //魔法免疫
        Invincible, //无敌
        Silence, //沉默
        Dizzy, //眩晕
        Disarm, //缴械
        Sleep, //沉睡
        Blind, //致盲
        DisablePassive, //禁用被动
        Taunt, //嘲讽
    }
}