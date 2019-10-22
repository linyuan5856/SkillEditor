namespace BluePro.Skill
{
    /// <summary>
    /// 闪现
    /// </summary>
    public class SkillActionBlink : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            skill.RemoveSkillAction(data.Id, param);
        }
    }
}