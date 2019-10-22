using BluePro.FrameWork;

namespace BluePro.Skill
{
    /// <summary>
    /// 几率性延迟
    /// </summary>
    public class SkillActionDelayAction : ISkillAction
    {
        public void Action(ISkill skill, SkillActionData data, CommonParam param)
        {
            SkillUtil.TryParseWithDebug(data.Para1, out float delayTime, "Delay Time");
            SkillUtil.TryParseWithDebug(data.Para1, out int actionId, "Action Id");

            object[] actionParam = {skill, data, param, actionId};
            var timer = ServiceLocate.Instance.GetService<TimerService>();
            timer?.CreateTimer(DelayedCall, 0.1f, delayTime, actionParam);
        }


        void DelayedCall(bool isTickEnd, object[] @params)
        {
            if (isTickEnd)
            {
                var skill = (ISkill) @params[0];
                var data = (SkillData) @params[1];
                var param = (CommonParam) @params[2];
                var newActionId = (int) @params[3];

                SkillActionTrigger.Instance.Trigger(skill, newActionId, param.IdentifyId);
                skill.RemoveSkillAction(data.Id, param);
            }
        }
    }
}