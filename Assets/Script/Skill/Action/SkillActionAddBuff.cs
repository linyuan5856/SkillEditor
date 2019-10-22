namespace BluePro.Skill
{
    /// <summary>
    /// 添加BUFF
    /// </summary>
    public class SkillActionAddBuff : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            if (SkillUtil.TryParseWithDebug(data.Para1, out int addBuffId, "Add BuffID"))
                AddBuff(skill, param, addBuffId);
            skill.RemoveSkillAction(data.Id, param);
        }

        private void AddBuff(ISkill skill, CommonParam param, int buffId)
        {
            var targets = param.Targets;
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].GetSkillContext()?.AddBuff(skill, buffId);
            }
        }
    }
}