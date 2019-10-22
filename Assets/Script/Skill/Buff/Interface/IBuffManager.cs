namespace BluePro.Skill
{
    public interface IBuffManager
    {
        void Init(ISkillContext context);
        bool AddBuff(ISkill skill, int buffId);

        bool TryRemoveBuff(int buffId, bool forceClean);

        void ClearAllBuff();
        void OnBuffEventDispatch(BuffActionType type);
    }
}