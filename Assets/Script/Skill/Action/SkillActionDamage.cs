namespace BluePro.Skill
{
    public class SkillActionDamage : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            if (SkillUtil.GetSkillActionDamage(skill.GetSkillLevel(), data, out int damage))
                Damage(skill, param, damage);
            skill.RemoveSkillAction(data.Id, param);
        }

        private void Damage(ISkill skill, CommonParam param, int damage)
        {
            var targets = param.Targets;
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i] == null)
                    continue;
                targets[i].Damage(skill,damage);
            }
        }
    }
}