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
            foreach (var t in targets)
            {
                if (t == null) continue;
                t.Heal(skill, heal);
            }
        }
    }
}