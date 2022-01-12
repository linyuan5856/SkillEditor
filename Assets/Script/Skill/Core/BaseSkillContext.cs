namespace BluePro.Skill
{
    public class BaseSkillContext : ISkillContext
    {
        private ISkillManager mSkillManager;
        private BuffManager mBuffManager;
        private ISkillActor mSkillActor;

        protected BaseSkillContext(ISkillActor actor)
        {
            mSkillActor = actor;
            mSkillManager = new SkillManager();
            mBuffManager = new BuffManager();
        }

        public void Init()
        {
            mBuffManager.Init(this);
            mSkillManager.Init(this);
        }

        public ISkillManager GetSkillManager()
        {
            return mSkillManager;
        }

        public IBuffManager GetBuffManager()
        {
            return mBuffManager;
        }

        public ISkillActor GetSelfActor()
        {
            return mSkillActor;
        }

        public virtual int GetSkillContextId()
        {
            return GetSelfActor().GetIdentifyId();
        }

        public virtual bool CheckManaValid(int skillCost)
        {
            return GetSelfActor().CheckManaValid(skillCost);
        }

        public virtual bool AddBuff(ISkill skill, int buffId)
        {
            return GetBuffManager().AddBuff(skill, buffId);
        }

        public virtual bool TryRemoveBuff(int buffId, bool forceClean)
        {
            return GetBuffManager().TryRemoveBuff(buffId, forceClean);
        }

        public void ClearAllBuff()
        {
            GetBuffManager()?.ClearAllBuff();
        }

        public void ActorNormalAttackOther()
        {
            GetBuffManager()?.OnBuffEventDispatch(EBuffActionType.OwnerAttackOther);
        }

        public void OtherNormalAttackActor()
        {
            GetBuffManager()?.OnBuffEventDispatch(EBuffActionType.OwnerBeAttacked);
        }

        public void ActorHurtOther()
        {
            GetBuffManager()?.OnBuffEventDispatch(EBuffActionType.OwnerHurtOther);
        }

        public void OtherHurtActor()
        {
            GetBuffManager()?.OnBuffEventDispatch(EBuffActionType.OwnerBeHurt);
        }

        public void ActorKilledOther()
        {
            GetBuffManager()?.OnBuffEventDispatch(EBuffActionType.OwnerKillOther);
        }

        public void ActorBeKilled()
        {
            GetBuffManager()?.OnBuffEventDispatch(EBuffActionType.OwnerBeKilled);
        }
    }
}