namespace BluePro.Skill
{
    public interface ISkillManager
    {
        void Init(ISkillContext context);

        void CastSkill(int skillId, CommonParam param);

        void CastSkillByIndex(int skillIndex, CommonParam param);
    }
}