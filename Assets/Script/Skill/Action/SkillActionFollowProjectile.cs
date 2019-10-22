namespace BluePro.Skill
{
    /// <summary>
    /// 跟踪投射物
    /// </summary>
    public class SkillActionFollowProjectile : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            SkillUtil.CreatProjectile(skill, param, data);
        }
    }
}