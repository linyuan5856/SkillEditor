using System.Collections.Generic;
using UnityEngine;

namespace BluePro.Skill
{
    public class SkillTargetSearch : Singleton<SkillTargetSearch>
    {
        public void GetTargets(ISkillContext context,
            SkillActionData data, CommonParam param)
        {
            if (param.IsBounce && param.BounceParam.BounceTime > 1) //弹射
                BounceSearch(context, data, param);
            else
                NormalSearch(context, data, param);
        }

        /// <summary>
        /// 弹射模式下的搜索
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="param"></param>
        private void BounceSearch(ISkillContext context,
            SkillActionData data, CommonParam param)
        {
            if (data.Id != param.BounceParam.SearchTargetActionId)
                return;

            param.ResetTarget();
            var bounceData = SkillUtil.GetSkillActionData(param.BounceParam.BounceActionId);
            //弹射检查范围
            SkillUtil.TryParseWithDebug(bounceData.Para4, out int range, "弹射范围");
            bool isIgnoreRepeat = string.Equals("1", data.Para5);

            //todo  fake data,need real check 
            var targetArray = GameObject.FindObjectsOfType<TestSkillActor>();
            param.AddTargets(targetArray);
            FilterTarget(context, data, param);
            var targets = param.Targets;

            //1.排除自己
            //2.排除已经弹射过的
            //3.找距离最近的
            //4.已经死亡的排除
            float minDistance = float.MaxValue;
            ISkillActor nearestActor = null;
            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];
                if (target.IsDead()) continue;
                //todo TestSkillActor remove
                if ((TestSkillActor) target == (TestSkillActor) context.GetSelfActor()) continue;
                if (!isIgnoreRepeat && param.BounceParam.HasBounced(target)) continue;

                var distance = Vector3.Distance(target.GetTransform().position,
                    param.BounceParam.LastBouncedActor.GetTransform().position);
                if (!(distance < minDistance) || !(distance <= range)) continue;
                minDistance = distance;
                nearestActor = target;
            }

            param.ResetTarget();
            if (nearestActor != null)
                param.AddTarget(nearestActor);
        }

        /// <summary>
        /// 普通搜索
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="param"></param>
        private void NormalSearch(ISkillContext context,
            SkillActionData data, CommonParam param)
        {
            NormalGetTargets(context, data, param);

            FilterTarget(context, data, param);
        }

        private void NormalGetTargets(ISkillContext context,
            SkillActionData data, CommonParam param)
        {
            var rangeType = (SkillTargetRangeType) data.Radius.Key;
            var centerType = (SkillTargetCenter) data.Center;

            //线性投射会把Hit的目标动态AddTarget进来
            //跟踪投射 策划会配置Center为2
            if (centerType != SkillTargetCenter.ProjectileHitPoint)
                param.ResetTarget();

            if (rangeType == SkillTargetRangeType.None) //不需要检测距离
            {
                switch (centerType)
                {
                    case SkillTargetCenter.Caster:
                        //添加技能施放者
                        param.AddTarget(context.GetSelfActor());
                        break;
                    case SkillTargetCenter.Target:
                        //添加被施法目标
                        param.AddTarget(param.ActorClicked);
                        break;
                    default: //todo delete this later 
                        param.AddTarget(param.ActorClicked);
                        break;
                }
            }
            else
            {
                Vector3 center = Vector3.zero;
                center = GetCenterTarget(context, data, param);
                var range = data.Radius.Value;

                switch (rangeType)
                {
                    //todo
                    case SkillTargetRangeType.Circle:
                    case SkillTargetRangeType.Rect:
                    case SkillTargetRangeType.Sector:
                        //todo fake 暂时都按照 Radius处理
                        var targetArrary = GameObject.FindObjectsOfType<TestSkillActor>();
                        for (int i = 0; i < targetArrary.Length; i++)
                        {
                            var target = targetArrary[i];
                            if (target.IsDead())
                                continue;
                            var distance = Vector3.Distance(target.GetTransform().position, center);
                            if (distance <= range)
                                param.AddTarget(target);
                        }

                        break;
                }
            }
        }

        private Vector3 GetCenterTarget(ISkillContext context,
            SkillActionData data, CommonParam param)
        {
            var centerType = (SkillTargetCenter) data.Center;
            Vector3 pos = Vector3.zero;

            switch (centerType)
            {
                case SkillTargetCenter.Caster:
                    pos = context.GetSelfActor().GetTransform().position;
                    break;
                case SkillTargetCenter.Target:
                    pos = param.ActorClicked?.GetTransform().position ?? pos;
                    break;
                case SkillTargetCenter.Point:
                    pos = param.Postion;
                    break;
                case SkillTargetCenter.Attacker:
                    //todo
                    break;
                case SkillTargetCenter.Projectile:
                    //todo
                    break;
                case SkillTargetCenter.ProjectileHitPoint:
                    //todo
                    break;
            }

            return pos;
        }


        private List<ISkillActor> removelist = new List<ISkillActor>();

        private void FilterTarget(ISkillContext context, SkillActionData data, CommonParam param)
        {
            removelist.Clear();

            var targets = param.Targets;
            var needTeamType = (SkillTargetTeamType) data.Team;

            for (int i = 0; i < targets.Count; i++)
            {
                var target = targets[i];

                if (needTeamType != SkillTargetTeamType.ALl) //阵营
                {
                    var targetTeamType = target.GetTargetTeamType();
                    if (target == context.GetSelfActor()) //todo 待数据层成型 删除
                        targetTeamType = SkillTargetTeamType.Friend;
                    if (targetTeamType != needTeamType)
                        removelist.Add(target);
                }

                if (!data.Type.Contains((int) SkillTargetType.All)) //类型
                {
                    for (int j = 0; j < data.Type.Count; j++)
                    {
                        var needType = (SkillTargetType) data.Type[j];
                        if (needType == target.GetTargetType())
                            break;
                        if (j == data.Type.Count - 1)
                            removelist.Add(target);
                    }
                }
            }


            for (int i = 0; i < removelist.Count; i++)
            {
                targets.Remove(removelist[i]);
            }
        }
    }
}