using System;
using System.Collections.Generic;
using BluePro.FrameWork;

namespace BluePro.Skill
{
    public class Skill : ISkill
    {
        /*
        * 使用字典原因:(比如王者鲁班七号2技能,释放可能会乱序--第一次打到地图外，第二次直接打面前目标，第二次比第一次先执行完毕)
        * 用IdentifyId作为Key匹配技能参数,整个技能释放各个阶段传递IdentifyId
        */
        private Dictionary<int, CommonParam> skillParamDic;

        private Dictionary<int, CommonParam> buffParamDic;

        //计算技能是否施法完毕的Action计数缓存 todo 考虑后期优化为 TreeNode
        //{[Key->IdentifyId] [Value->(key->ActionId value->Action被调用的次数)]}
        private Dictionary<int, Dictionary<int, int>> skillRecordDic;
        private static int identifyId; //使用技能释放次数作为标志位
        private static int buffIdentifyId = 100000; //使用Buff释放次数作为标志位
        private int id;
        private SkillData data;
        private ISkillContext context;
        private int level = 1;
        private bool isInit;
       

        public virtual void Init(SkillData skillData, ISkillContext skillContext)
        {
            data = skillData;
            context = skillContext;
            id = data?.Id ?? 0;
            skillParamDic = new Dictionary<int, CommonParam>();
            buffParamDic = new Dictionary<int, CommonParam>();
            skillRecordDic = new Dictionary<int, Dictionary<int, int>>();
            isInit = true;
            OnOwnedSkill(identifyId);
        }

        private void AddSkillParam(int id, CommonParam param, bool isSkill = true)
        {
            Dictionary<int, CommonParam> dic = isSkill ? skillParamDic : buffParamDic;
            if (dic == null) return;
            param.SetIdentifyId(id, !isSkill);
            dic.Add(id, param);
        }

        private void RemoveSkillParam(int id, bool isSkill = true)
        {
            Dictionary<int, CommonParam> dic = isSkill ? skillParamDic : buffParamDic;
            if (dic != null && dic.ContainsKey(id))
                skillParamDic.Remove(id);
            else
                SkillUtil.LogError($"Param ID->{id} 在字典中不存在");
        }


        public CommonParam CreateBuffParam(ISkillActor actor)
        {
            CommonParam param = new CommonParam(actor);
            buffIdentifyId++;
            AddSkillParam(buffIdentifyId, param, false);
            return param;
        }

        public void RemoveBuffParam(int id)
        {
            RemoveSkillParam(id, false);
        }

        public CommonParam GetParam(int identifyId, bool isBuff)
        {
            Dictionary<int, CommonParam> dic = isBuff ? buffParamDic : skillParamDic;
            if (dic == null)
            {
                SkillUtil.LogError("技能尚未初始化");
                return null;
            }

            if (dic.ContainsKey(identifyId))
                return dic[identifyId];

            var error =
                $"{SkillUtil.GetSkillDebugDes(this) + " " + (isBuff ? "Buff" : "Skill")} Param为空 IdentifyId->{identifyId} ";
            SkillUtil.LogError(error);
            return null;
        }


        public virtual bool IsSKillEnd(int identifyId)
        {
            return !skillParamDic.ContainsKey(identifyId);
        }

        public ISkillContext GetContext()
        {
            return context;
        }

        public SkillData GetData()
        {
            return data;
        }


        public int GetSkillId()
        {
            return id;
        }

        public int GetSkillLevel()
        {
            return level;
        }

        public virtual void UpdateSkillLevel(int newLv)
        {
            level = newLv;
        }

        public virtual void CastSkill(CommonParam commonParam)
        {
            if (!isInit)
            {
                SkillUtil.LogError("SKill is not Init");
                return;
            }

            if (context.CanCastSkill(id)) return;

            identifyId++;
            AddSkillParam(identifyId, commonParam);

            SkillUtil.LogWarning($"{SkillUtil.GetSkillDebugDes(this)} 开始释放");
            PlayAnimation();
            float castPointTime = SkillUtil.GetSkillCastPoint(GetSkillLevel(), data);

            void CastSkill(bool isEnd, object[] param)
            {
                CommonParam actionParam = (CommonParam)param[0]; 
                if (isEnd) OnCastSkillStart(actionParam.IdentifyId);
            }

            var timerService = ServiceLocate.Instance.GetService<TimerService>();
            timerService.CreateTimer(CastSkill, 0.1f, castPointTime, new object[] { commonParam });
        }

        public void AddSkillAction(int actionId, int identifyId)
        {
            //SkillUtil.LogWarning(string.Format("{0} SkillAddAction ActionID->{1}  IdentifyId->{2}",
            // SkillUtil.GetSkillDebugDes(this), actionId, identifyId));

            //todo  need a pool ,always create dictionary 
            Dictionary<int, int> record = null;
            if (skillRecordDic.ContainsKey(identifyId))
                record = skillRecordDic[identifyId];
            if (record == null)
            {
                record = new Dictionary<int, int>();
                skillRecordDic.Add(identifyId, record);
            }

            int time = 1;
            if (record.ContainsKey(actionId))
            {
                time = record[actionId];
                record[actionId] = time + 1;
            }
            else
                record.Add(actionId, time);
        }

        public void RemoveSkillAction(int actionId, CommonParam param)
        {
            if (param.IsBuff)
                return;

            if (Internal_RemoveSkillAction(actionId, param.IdentifyId))
                OnCastSkillEnd(param.IdentifyId);
        }

        private bool Internal_RemoveSkillAction(int actionId, int identifyId)
        {
            //SkillUtil.LogWarning(string.Format("{0} SkillRemoveAction ActionID->{1}  IdentifyId->{2}",
            
            Dictionary<int, int> record = null;
            if (skillRecordDic.ContainsKey(identifyId))
                record = skillRecordDic[identifyId];
            if (record == null)
            {
                var error =
                    $" [Remove Action Failed,SkillId {SkillUtil.GetSkillDebugDes(this)} ActionID->{actionId}  IdentifyId->{identifyId}";
                SkillUtil.LogError(error);
                return false;
            }

            int time = 1;
            if (record.ContainsKey(actionId))
            {
                time = record[actionId];
                var newValue = time - 1;
                if (newValue <= 0)
                {
                    record.Remove(actionId);
                    if (record.Count == 0)
                        return true;
                }
                else
                    record[actionId] = newValue;
            }
            else
            {
                var error =
                    $" [Remove Action Failed,SkillId {SkillUtil.GetSkillDebugDes(this)} ActionID->{actionId}  IdentifyId->{identifyId}";
                SkillUtil.LogError(error);
                return false;
            }

            return false;
        }

        void PlayAnimation()
        {
            if (!string.IsNullOrEmpty(data.SkillCastAnimation))
                context.GetSelfActor().PlayAnimation(data.SkillCastAnimation);
        }

        /// <summary>
        /// 拥有技能
        /// </summary>
        /// <param name="identifyId"></param>
        protected virtual void OnOwnedSkill(int identifyId)
        {
            var time = ESkillActionTriggerTime.OwnedSkill;
            SkillActionTrigger.Instance.TryTriggerAction(this, data, time, identifyId, Internal_OnOwnedSkill);
        }

        void Internal_OnOwnedSkill()
        {
            SkillUtil.LogWarning(string.Format("{0} 被动技能初始化", SkillUtil.GetSkillDebugDes(this)));
            identifyId++;
            var param = new CommonParam();
            AddSkillParam(identifyId, param);
        }

        /// <summary>
        /// 开始释放技能
        /// </summary>
        /// <param name="identifyId"></param>
        void OnCastSkillStart(int identifyId)
        {
            SkillActionTrigger.Instance.TryTriggerAction(this, data, ESkillActionTriggerTime.StartCastSkill, identifyId);
        }

        /// <summary>
        /// 释放技能完毕回调
        /// </summary>
        void OnCastSkillEnd(int identifyId)
        {
            var param = GetParam(identifyId, false);
            if (param == null)
                return;
            if (param.IsBounce)
                param.SetBounceState(false, -1, -1);
            RemoveSkillParam(identifyId);
            if (skillRecordDic.ContainsKey(identifyId))
                skillRecordDic.Remove(identifyId);
            SkillUtil.LogWarning($"{SkillUtil.GetSkillDebugDes(this)} 技能释放结束 ");
        }

        /// <summary>
        /// 弹射单次完成
        /// </summary>
        /// <param name="identifyId"></param>
        public virtual void OnBounceNodeEnd(int identifyId, int activeId)
        {
            CommonParam param = GetParam(identifyId, false);
            if (param == null)
                return;

            if (param.IsBounce) //弹射
            {
                SkillActionData bounceData = SkillUtil.GetSkillActionData(param.BounceParam.BounceActionId);
                int maxBounceTime = int.Parse(bounceData.Para1); //最大允许的弹射次数
                float.TryParse(bounceData.Para2, out float interval); //弹射间隔
                if (param.BounceParam.BounceTime < maxBounceTime) //开始下一次弹射
                {
                    object[] actionParam = { activeId, bounceData.Id, identifyId };

                    if (interval > 0)
                    {
                        var timerService = ServiceLocate.Instance.GetService<TimerService>();
                        timerService.CreateTimer(TryDoNextBounce, 0.1f, interval, actionParam);
                    }
                    else
                        TryDoNextBounce(true, actionParam);
                }
                else //超过弹射次数
                {
                    SkillUtil.LogWarning("超过最大弹射次数 弹射取消 Max Time->" + maxBounceTime);
                    RemoveSkillAction(activeId, param);
                }
            }
        }

        /// <summary>
        /// 尝试发起下一次弹射（执行Action发现不到新目标会停止）
        /// </summary>
        /// <param name="isTickEnd"></param>
        /// <param name="params"></param>
        void TryDoNextBounce(bool isTickEnd, object[] @params)
        {
            int activeId = (int)@params[0];
            int bounceDataId = (int)@params[1];
            int identifyId = (int)@params[2];

            CommonParam param = GetParam(identifyId, false);
            if (param == null)
                return;

            param.BounceParam.AddBounceTime();
            SkillActionTrigger.Instance.Trigger(this, bounceDataId, identifyId);
            if (!IsSKillEnd(identifyId)) //触发下次弹射可能找不到目标而提前结束
                RemoveSkillAction(activeId, param);
        }

        /// <summary>
        /// 投射物Hit 目标
        /// </summary>
        /// <param name="identifyId"></param>
        public virtual void OnProjectileHit(int identifyId)
        {
            SkillActionTrigger.Instance.TryTriggerAction(this, data, ESkillActionTriggerTime.ProjectileHit,
                identifyId);
        }

        /// <summary>
        /// 投射物未命中目标
        /// </summary>
        /// <param name="identifyId"></param>
        public virtual void OnProjectileDisappear(int identifyId)
        {
            SkillActionTrigger.Instance.TryTriggerAction(this, data, ESkillActionTriggerTime.ProjectileDisappear,
                identifyId);
        }
    }
}