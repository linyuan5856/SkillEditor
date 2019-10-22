namespace BluePro.Skill
{
    /// <summary>
    /// 眩晕
    /// </summary>
    public class SkillActionDizzy : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            
            skill.RemoveSkillAction(data.Id, param);
        }
    }
}