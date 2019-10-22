namespace BluePro.Skill
{
    /// <summary>
    /// 动画
    /// </summary>
    public class SkillActionAnimation  : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            skill.RemoveSkillAction(data.Id, param);
        }

       
    }
}