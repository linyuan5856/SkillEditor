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

            this.skillContext = context;
            this.skills = SkillWorldContext.CreateSKillList(context);
            this.isInit = true;
        }


        public void CastSkill(int skillId, CommonParam param)
        {
            if (!this.isInit)
            {
                SkillUtil.Log("Skill Manager not Init");
                return;
            }

            var skill = GetSkill(skillId);
            skill?.CastSkill(param);
        }

        public void CastSkillByIndex(int skillIndex, CommonParam param)
        {
            if (!this.isInit)
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

            for (int i = 0; i < skills.Count; i++)
            {
                if (skills[i] != null && skills[i].GetSkillId() == skillId)
                    return skills[i];
            }

#if UNITY_EDITOR
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < this.skills.Count; i++)
            {
                sb.Append(this.skills[i].GetSkillId());
                sb.Append("|");
            }

            SkillUtil.LogError($"已经装配的Skill {sb}");
#endif
            SkillUtil.LogError($"SkillId-> {skillId.ToString()} 不存在");
            return null;
        }

        private ISkill GetSkillByIndex(int skillIndex)
        {
            if (skills == null)
                return null;

            if (skillIndex >= 0 && skillIndex < skills.Count)
                return skills[skillIndex];

            return null;
        }
    }
}