using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BluePro.Skill
{
    public interface ISkillAction
    {
        void Action(ISkill skill, SkillActionData data, CommonParam param);
    }
}