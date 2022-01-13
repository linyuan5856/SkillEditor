namespace BluePro.Skill
{
    public interface ISkillContext
    {
        ISkillManager GetSkillManager();
        IBuffManager GetBuffManager();
        ISkillActor GetSelfActor();

        int GetSkillContextId();
        bool CanCastSkill(int skillId);
        bool CheckManaValid(int skillCost);

        bool AddBuff(ISkill skill, int buffId);

        bool TryRemoveBuff( int buffId, bool forceClean);

        void ClearAllBuff();

        void ActorNormalAttackOther();

        void OtherNormalAttackActor();

        void ActorHurtOther();

        void OtherHurtActor();

        void ActorKilledOther();

        void ActorBeKilled();
    }
}