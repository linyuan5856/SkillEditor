namespace BluePro.Skill
{
    /// <summary>
    /// 移除BUFF
    /// </summary>
    public class SkillActionRemoveBuff : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            if (SkillUtil.TryParseWithDebug(data.Para1, out int removeBuffId, "remove BuffID"))
                RemoveBuff(param, removeBuffId);
            skill.RemoveSkillAction(data.Id, param);
        }

        private void RemoveBuff(CommonParam param, int buffId)
        {
            var targets = param.Targets;
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].GetSkillContext()?.TryRemoveBuff(buffId, false);
            }
        }
    }
}