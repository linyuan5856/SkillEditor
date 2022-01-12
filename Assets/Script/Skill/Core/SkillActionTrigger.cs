using System;
using System.Collections.Generic;

namespace BluePro.Skill
{
    public class SkillActionTrigger : Singleton<SkillActionTrigger>
    {
        private Dictionary<int, ISkillAction> map;

        protected override void InitSingleton()
        {
            base.InitSingleton();
            map = new Dictionary<int, ISkillAction>
            {
                { (int)ESkillActionType.AddBuff, new SkillActionAddBuff() },
                { (int)ESkillActionType.RemoveBuff, new SkillActionRemoveBuff() },
                { (int)ESkillActionType.Damage, new SkillActionDamage() },
                { (int)ESkillActionType.Heal, new SkillActionHeal() },
                { (int)ESkillActionType.Dizzy, new SkillActionDizzy() },
                { (int)ESkillActionType.LineProjectile, new SkillActionLineProjectile() },
                { (int)ESkillActionType.FollowProjectile, new SkillActionFollowProjectile() },
                { (int)ESkillActionType.Blink, new SkillActionBlink() },
                { (int)ESkillActionType.Summon, new SkillActionSummon() },
                { (int)ESkillActionType.Knockback, new SkillActionKnockback() },
                { (int)ESkillActionType.RunScript, new SkillActionRunScript() },
                { (int)ESkillActionType.ProbabilityAction, new SkillActionProbabilityAction() },
                { (int)ESkillActionType.DelayAction, new SkillActionDelayAction() },
                { (int)ESkillActionType.BounceAction, new SkillActionBounceAction() },
                { (int)ESkillActionType.Effect, new SkillActionEffect() },
                { (int)ESkillActionType.Audio, new SkillActionAudio() },
                { (int)ESkillActionType.Action, new SkillActionAnimation() },
            };
        }

        private List<int> cacheActions;

        private bool HasAction(SkillData data, ESkillActionTriggerTime triggerTime, ref List<int> actions)
        {
            actions ??= new List<int>();
            actions.Clear();

            bool bFind = false;
            foreach (var ac in data.Action)
            {
                var acType = (ESkillActionTriggerTime)ac.Key;
                if (acType != triggerTime) continue;
                actions.Add(ac.Value);
                bFind = true;
            }

            return bFind;
        }

        public void TryTriggerAction(ISkill skill, SkillData data, ESkillActionTriggerTime triggerTime, int identifyId,
            Action confirmCb = null)
        {
            SkillUtil.Log($"  TriggerTime-> {triggerTime}");
            if (!HasAction(data, triggerTime, ref cacheActions)) return;
            confirmCb?.Invoke();
            TriggerMultiple(skill, cacheActions, identifyId);
        }

        public bool Trigger(ISkill skill, int actionId, int identifyId)
        {
            return Internal_TriggerAction(skill, actionId, identifyId, false);
        }

        public bool TriggerBuff(ISkill skill, int actionId, int identifyId)
        {
            return Internal_TriggerAction(skill, actionId, identifyId, true);
        }

        //todo 这个触发结构不好 后期优化重构
        public bool TriggerMultiple(ISkill skill, List<int> actionIds, int identifyId)
        {
            if (skill == null)
            {
                SkillUtil.LogError("skill is nullptr");
                return false;
            }

            var result = true;
            foreach (var actionId in actionIds)
                skill.AddSkillAction(actionId, identifyId);

            foreach (var actionId in actionIds)
            {
                if (!Internal_TriggerAction(skill, actionId, identifyId, false, false))
                    result = false;
            }

            return result;
        }

        private bool Internal_TriggerAction(ISkill skill, int actionId, int identifyId, bool isBuff,
            bool addSkillAction = true)
        {
            SkillActionData data = null;
            if (!IsValid(skill, actionId, identifyId, isBuff, ref data)) return false;
            
            var param = skill.GetParam(identifyId, isBuff);
            if (map.ContainsKey(data.ActionType))
            {
                //  SkillUtil.Log(string.Format("  Trigger Type-> {0}", (SkillActionType) data.ActionType));
                if (addSkillAction && !isBuff)
                    skill.AddSkillAction(actionId, identifyId);
                map[data.ActionType].Action(skill, data, param);
            }
            else
                SkillUtil.LogError(SkillUtil.GetSkillDebugDes(skill) + "  Ation Type" + data.ActionType + " 未注册");
            return true;
        }

        private bool IsValid(ISkill skill, int id, int identifyId, bool isBuff, ref SkillActionData data)
        {
            if (skill == null)
            {
                SkillUtil.LogError("skill is nullptr");
                return false;
            }

            data = SkillUtil.GetSkillActionData(id);
            ISkillContext skillContext = skill.GetContext();

            if (data == null)
            {
                SkillUtil.LogError(SkillUtil.GetSkillDebugDes(skill) + "技能数据尚未初始化");
                return false;
            }

            if (skillContext == null)
            {
                SkillUtil.LogError(SkillUtil.GetSkillDebugDes(skill) + "技能数据尚未初始化");
                return false;
            }

            var param = skill.GetParam(identifyId, isBuff);
            if (param == null) return false;

            SkillTargetSearch.Instance.GetTargets(skillContext, data, param);

            if (param.HasTargets()) return true;
            if (param.IsBounce)
            {
                SkillUtil.Log(SkillUtil.GetSkillDebugDes(skill) + "技能没有目标 弹射结束");
                skill.RemoveSkillAction(data.Id, param);
            }
            else
                SkillUtil.LogError(SkillUtil.GetSkillDebugDes(skill) + "ActionID->" + id + " 技能没有目标");

            return false;
        }
    }
}