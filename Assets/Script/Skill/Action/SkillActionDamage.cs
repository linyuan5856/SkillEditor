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
            foreach (var actor in targets)
            {
                if (actor == null) continue;
                actor.Damage(skill,damage);
            }
        }
    }
}