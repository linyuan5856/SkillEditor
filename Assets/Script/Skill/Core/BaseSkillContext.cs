namespace BluePro.Skill
{
    public class BaseSkillContext : ISkillContext
    {
        private SkillManager mSkillManager;
        private BuffManager mBuffManager;
        private ISkillActor mSkillActor;

        protected BaseSkillContext(ISkillActor actor)
        {
            this.mSkillActor = actor;
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
            GetBuffManager()?.OnBuffEventDispatch(BuffActionType.OwnerAttackOther);
        }

        public void OtherNormalAttackActor()
        {
            GetBuffManager()?.OnBuffEventDispatch(BuffActionType.OwnerBeAttacked);
        }

        public void ActorHurtOther()
        {
            GetBuffManager()?.OnBuffEventDispatch(BuffActionType.OwnerHurtOther);
        }

        public void OtherHurtActor()
        {
            GetBuffManager()?.OnBuffEventDispatch(BuffActionType.OwnerBeHurt);
        }

        public void ActorKilledOther()
        {
            GetBuffManager()?.OnBuffEventDispatch(BuffActionType.OwnerKillOther);
        }

        public void ActorBeKilled()
        {
            GetBuffManager()?.OnBuffEventDispatch(BuffActionType.OwnerBeKilled);
        }
    }
}