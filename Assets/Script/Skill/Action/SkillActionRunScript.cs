namespace BluePro.Skill
{
    /// <summary>
    /// 执行脚本
    /// </summary>
    public class SkillActionRunScript  : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            skill.RemoveSkillAction(data.Id, param);
        }

       
    }
}