namespace BluePro.Skill
{
    /// <summary>
    /// 击退
    /// </summary>
    public class SkillActionKnockback : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            skill.RemoveSkillAction(data.Id, param);
        }
    }
}