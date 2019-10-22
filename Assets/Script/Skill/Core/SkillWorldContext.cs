using System.Collections.Generic;
using BluePro.FrameWork;

namespace BluePro.Skill
{
    public class SkillWorldContext
    {
        public static List<ISkill> CreateSKillList(ISkillContext skillContext)
        {
            //todo  Load skillList index From Role Data

            return CreateFakeSkillDatas(skillContext);
        }

        static Dictionary<int, List<int>> dic = new Dictionary<int, List<int>>
        {
            //1003 反击螺旋
            {1, new List<int> {1001, 1002, 5001, 2001}},
            {2, new List<int> {3001, 4001, 5001, 2001}},
            {3, new List<int> {6001, 7001, 8001, 9001}},
        };

        static List<ISkill> CreateFakeSkillDatas(ISkillContext skillContext)
        {
            if (skillContext == null)
            {
                SkillUtil.LogError("skillContext is Null");
                return null;
            }

            List<ISkill> datas = new List<ISkill>();
            int skillContextId = skillContext.GetSkillContextId();

            if (dic.ContainsKey(skillContextId))
            {
                var lists = dic[skillContextId];
                for (int i = 0; i < lists.Count; i++)
                {
                    Skill skill = new Skill();
                    skill.Init(CreateFakeSkillData(lists[i]), skillContext);
                    datas.Add(skill);
                }
            }

            return datas;
        }


        static SkillData CreateFakeSkillData(int skillId)
        {
            var loaderService = ServiceLocate.Instance.GetService<LoaderService>();
            var data = loaderService.GetRowFromTable(SkillDefine.SKILLTABLE, skillId);
            return data as SkillData;
        }
    }
}