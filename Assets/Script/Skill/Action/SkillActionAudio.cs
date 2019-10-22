namespace BluePro.Skill
{
    /// <summary>
    /// 音频
    /// </summary>
    public class SkillActionAudio : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            skill.RemoveSkillAction(data.Id, param);
        }
    }
}