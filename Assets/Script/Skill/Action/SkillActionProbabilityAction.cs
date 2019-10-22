namespace BluePro.Skill
{
    /// <summary>
    /// 几率性触发行为
    /// </summary>
    public class SkillActionProbabilityAction : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            SkillUtil.TryParseWithDebug(data.Para1, out int probability, "概率");
            SkillUtil.TryParseWithDebug(data.Para3, out int successActionId, "成功ActionId");
            SkillUtil.TryParseWithDebug(data.Para3, out int failActionId, "失败ActionId");

            float prob = probability / 100000.0f;
            int actionId = SkillUtil.GetRandomValue() <= prob ? successActionId : failActionId;
            SkillActionTrigger.Instance.Trigger(skill, actionId, param.IdentifyId);
            skill.RemoveSkillAction(data.Id, param);
        }
    }
}