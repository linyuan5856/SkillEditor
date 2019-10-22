namespace BluePro.Skill
{
    /// <summary>
    /// 召唤
    /// </summary>
    public class SkillActionSummon  : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            skill.RemoveSkillAction(data.Id, param);
        }

       
    }
}