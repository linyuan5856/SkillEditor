namespace BluePro.Skill
{
    /// <summary>
    /// 特效
    /// </summary>
    public class SkillActionEffect : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            skill.RemoveSkillAction(data.Id, param);
        }
    }
}