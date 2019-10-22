namespace BluePro.Skill
{
    /// <summary>
    /// 治疗
    /// </summary>
    public class SkillActionHeal : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            if (SkillUtil.GetSkillActionHeal(skill.GetSkillLevel(), data, out int heal))
                Heal(skill, param, heal);
            skill.RemoveSkillAction(data.Id, param);
        }

        private void Heal(ISkill skill, CommonParam param, int heal)
        {
            var targets = param.Targets;
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                    continue;
                targets[i].Heal(skill, heal);
            }
        }
    }
}