namespace BluePro.Skill
{
    /// <summary>
    /// 弹射
    /// </summary>
    public class SkillActionBounceAction : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            //拿到弹射特效的Action ID
            var actionList = ConvertUtil.ConvertToIntList(data.Para3);
            if (actionList == null || actionList.Count == 0)
                return;

            if (!param.IsBounce)
            {
                param.SetBounceState(true, data.Id, actionList[0]); //进入弹射状态
            }

            SkillActionTrigger.Instance.TriggerMultiple(skill, actionList, param.IdentifyId);
            skill.RemoveSkillAction(data.Id, param);
        }
    }
}