using System.Collections.Generic;

namespace BluePro.Skill
{
    public class SkillManager : ISkillManager
    {
        private bool isInit;
        private List<ISkill> skills;
        private ISkillContext skillContext;

        public void Init(ISkillContext context)
        {
            if (context == null)
                return;

            skillContext = context;
            skills = SkillWorldContext.CreateSKillList(context);
            isInit = true;
        }


        public void CastSkill(int skillId, CommonParam param)
        {
            if (!isInit)
            {
                SkillUtil.Log("Skill Manager not Init");
                return;
            }

            var skill = GetSkill(skillId);
            skill?.CastSkill(param);
        }

        public void CastSkillByIndex(int skillIndex, CommonParam param)
        {
            if (!isInit)
            {
                SkillUtil.Log("Skill Manager not Init");
                return;
            }

            var skill = GetSkillByIndex(skillIndex);
            skill?.CastSkill(param);
        }


        private ISkill GetSkill(int skillId)
        {
            if (skills == null)
                return null;

            foreach (var t in skills)
            {
                if (t != null && t.GetSkillId() == skillId)
                    return t;
            }

#if UNITY_EDITOR
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var t in skills)
            {
                sb.Append(t.GetSkillId());
                sb.Append("|");
            }
            SkillUtil.LogError($"已经装配的Skill {sb}");
#endif
            SkillUtil.LogError($"SkillId-> {skillId.ToString()} 不存在");
            return null;
        }

        private ISkill GetSkillByIndex(int skillIndex)
        {
            if (skills == null) return null;
            if (skillIndex >= 0 && skillIndex < skills.Count)
                return skills[skillIndex];
            return null;
        }
    }
}