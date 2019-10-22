namespace BluePro.Skill
{
    /// <summary>
    /// 线性投射物
    /// </summary>
    public class SkillActionLineProjectile : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            SkillUtil.CreatProjectile(skill, param, data);
        }
    }
}