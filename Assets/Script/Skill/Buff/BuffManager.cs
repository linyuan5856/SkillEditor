using System;
using System.Collections.Generic;
using BluePro.FrameWork;

namespace BluePro.Skill
{
    public class BuffManager : IBuffManager
    {
        private ISkillContext mContext;
        private MapList<int, IBuff> buffMap;
        private Dictionary<int, int> stateMap; //key->类型 value->叠加次数
        private Dictionary<int, Queue<int>> buffTimerMap;
        private TimerService timerService;

        public void Init(ISkillContext context)
        {
            mContext = context;
            buffMap = new MapList<int, IBuff>();
            stateMap = new Dictionary<int, int>();
            buffTimerMap = new Dictionary<int, Queue<int>>();
            timerService = ServiceLocate.Instance.GetService<TimerService>();
        }

        public bool AddBuff(ISkill skill, int buffId)
        {
            SkillUtil.LogWarning(string.Format(" [BUFFMANAGER]  [NAME]->{0}  AddBuff-> {1}",
                mContext.GetSelfActor().GetTransform().name, buffId));

            IBuff buff = null;
            bool canModify = true;
            float life = 0;
            if (buffMap.ContainsKey(buffId))
            {
                buff = buffMap[buffId];
                canModify = buff is IOverLayBuff;
                /*Buff
                  1.已经拥有 2.有持续时间 3.不可叠加
                  -》直接刷新Buff的持续时间（Life）
                 */
                if (!canModify)
                    RefreshTimerLife(buff);
            }
            else
            {
                var data = SkillUtil.GetSkillBuffData(buffId);
                bool isOverLay = data.Attributes.Contains((int)BuffAttributes.CanAdd);
                buff = isOverLay ? new OverlayBuff() : new Buff();
                buff.Create(skill, mContext.GetSelfActor(), buffId);
                buffMap.Add(buffId, buff);
            }

            if (canModify) //只有新加的和可叠加的Buff才能触发Modify
            {
                BuffModify(buffId);

                //1.先判定这个Buff 是不是一个循环定时器
                if (!TryAddLoopTimer(buffId))
                {
                    life = buff.GetDuringTime();
                    if (life > 0)
                    {
                        /* 可以执行到此处满足的条件：
                         * 1.不是循环定时器 2.有效的Buff生命周期life 3.可以叠加效果的老BUFF或者刚刚添加的新BUFF
                        * =》生成一个新的计时器
                        */
                        CreateNormalTimer(life, buffId);
                    }
                }
            }

            OnBuffEventDispatch(BuffActionType.AddBuff);
            return true;
        }

        /// <summary>
        ///  尝试移除Buff 1.移除Buff效果 2.移除定时器（如果Buff有） 3.从Buff Manager移除出字典
        /// A.普通Buff 直接走完1，2，3
        /// B.可以叠加的Buff 走完1，2 每次层数-1，如果层数==0,走3
        /// </summary>
        /// <param name="skill">拥有这个Buff的Skill载体</param>
        /// <param name="buffId">BuffId</param>
        /// <param name="forceClean">彻底清除</param>
        /// <returns></returns>
        public bool TryRemoveBuff(int buffId, bool forceClean)
        {
            SkillUtil.LogWarning(string.Format(" [BUFFMANAGER] [Target]->{0}  TryRemoveBuff -> {1}",
                mContext.GetSelfActor().GetTransform().name, buffId));
            if (buffMap.ContainsKey(buffId))
            {
                var buff = buffMap[buffId];
                if (buff.GetDuringTime() > 0)
                    RemoveTimer(buffId, forceClean);


                bool isOverLayBuff = buff is IOverLayBuff;
                if (isOverLayBuff) //可以叠加Buff
                {
                    IOverLayBuff overlayBuff = buff as IOverLayBuff;
                    int time = forceClean ? overlayBuff.GetOverlayTime() : 1;
                    for (int i = 0; i < time; i++)
                    {
                        EndBuffModify(buffId);
                    }

                    var result = overlayBuff.TryRemove();
                    if (result || forceClean)
                        RemoveBuff(buff);
                }
                else //普通Buff
                {
                    EndBuffModify(buffId);
                    RemoveBuff(buff);
                }
            }

            return true;
        }

        private void RemoveBuff(IBuff buff)
        {
            int buffId = buff.GetBuffId();
            OnBuffEventDispatch(BuffActionType.RemoveBuff);
            buff.Release();
            buffMap.Remove(buffId);
        }

        /// <summary>
        /// 清除身上所有的BUFF
        /// </summary>
        public void ClearAllBuff()
        {
            SkillUtil.LogWarning(string.Format(" [BUFFMANAGER] [Target]->{0}  Clear All Buff",
                mContext.GetSelfActor().GetTransform().name));
            var list = buffMap.AsList();
            for (int i = 0; i < list.Count; i++)
            {
                TryRemoveBuff(list[i].GetBuffId(), true);
            }
        }

        /// <summary>
        /// Buff作用于Actor,改变属性
        /// </summary>
        private void BuffModify(int buffId)
        {
            var buffData = SkillUtil.GetSkillBuffData(buffId);
            var buff = buffMap[buffId];
            if (buffData == null || buff == null)
                return;
            AddActorState(buff, buffData);
            buff.GetTarget().ModifySpeed(int.Parse(buffData.PropertyMoveSpeed));
            buff.GetTarget().ModifyAttack(int.Parse(buffData.PropertyBaseAttack));
            buff.GetTarget().ModifyArmor(int.Parse(buffData.PropertyArmor));
        }

        /// <summary>
        /// Buff还原对于Actor的作用
        /// </summary>
        private void EndBuffModify(int buffId)
        {
            var buffData = SkillUtil.GetSkillBuffData(buffId);
            var buff = buffMap[buffId];
            if (buffData == null || buff == null)
                return;

            RemoveActorState(buff, buffData);
            buff.GetTarget().ModifySpeed(-int.Parse(buffData.PropertyMoveSpeed));
            buff.GetTarget().ModifyAttack(-int.Parse(buffData.PropertyBaseAttack));
            buff.GetTarget().ModifyArmor(-int.Parse(buffData.PropertyArmor));
        }

        private void AddActorState(IBuff buff, SkillBuffData buffData)
        {
            if (buffData.State != (int)ActorSkillState.None)
            {
                var stateData = SkillUtil.GetSkillStateData(buffData.State);
                int key = stateData.StateType;
                if (stateMap.ContainsKey(key))
                {
                    var number = stateMap[key];
                    stateMap[key] = number + 1;
                }
                else
                {
                    stateMap.Add(key, 1);
                    buff.GetTarget()?.AddState((ActorSkillState)key);
                }
            }
        }

        private void RemoveActorState(IBuff buff, SkillBuffData buffData)
        {
            if (buffData.State != (int)ActorSkillState.None)
            {
                var stateData = SkillUtil.GetSkillStateData(buffData.State);
                int key = stateData.StateType;
                if (stateMap.ContainsKey(key))
                {
                    var number = stateMap[key];
                    var newValue = number - 1;
                    if (newValue <= 0)
                    {
                        stateMap.Remove(key);
                        buff.GetTarget()?.RemoveState((ActorSkillState)key);
                    }
                    else
                        stateMap[key] = newValue;
                }
            }
        }

        #region 定时器

        private void SaveTimerId(int buffId, int timerId)
        {
            if (!buffTimerMap.ContainsKey(buffId))
                buffTimerMap.Add(buffId, new Queue<int>());
            var queue = buffTimerMap[buffId];
            queue.Enqueue(timerId);
            // SkillUtil.Log(string.Format("[BUFFMANAGER] Save Time Id  [BUffId->{0}] [TimeId->{1}] ])"
            //   , buffId, timerId));
        }

        private int GetTimerId(int buffId, bool isPeek)
        {
            if (buffTimerMap.ContainsKey(buffId))
            {
                var queue = buffTimerMap[buffId];
                if (queue != null && queue.Count > 0)
                {
                    var timerId = 0;
                    timerId = isPeek ? queue.Peek() : queue.Dequeue();
                    if (queue.Count == 0)
                        buffTimerMap.Remove(buffId);
                    return timerId;
                }
            }

            SkillUtil.LogError(string.Format("[BUFFMANAGER] GetTimerId Failed [BUffId]->[{0}] ])"
                , buffId));
            return SkillDefine.NONE;
        }

        /// <summary>
        /// 给Clear All Buff时拿到全部Buff的TimerId,全部Delete
        /// </summary>
        /// <param name="buffId"></param>
        /// <param name="timerIdList"></param>
        private void GetAllTimerId(int buffId, ref List<int> timerIdList)
        {
            if (buffTimerMap.ContainsKey(buffId))
            {
                var queue = buffTimerMap[buffId];
                if (queue != null && queue.Count > 0)
                {
                    var timerId = 0;
                    while (queue.Count > 0)
                    {
                        timerId = queue.Dequeue();
                        timerIdList.Add(timerId);
                        if (queue.Count == 0)
                            buffTimerMap.Remove(buffId);
                    }
                }
            }
        }

        /// <summary>
        /// 尝试启动循环定时器
        /// </summary>
        private bool TryAddLoopTimer(int buffId)
        {
            if (!IsBuffActionType(buffId, BuffActionType.LoopTimer, out int actionId)) return false;
            SkillBuffData data = SkillUtil.GetSkillBuffData(buffId);
            if (data == null) return false;
            SkillUtil.LogWarning($"[BUFFMANAGER] LoopTimer is Start [BUffId]->[{buffId}] ActionID->[{actionId}])");
            CreateLoopTimer(buffId, actionId, data);
            return true;
        }

        private void CreateNormalTimer(float life, int buffId)
        {
            object[] @params = { buffId };
            BaseCreateTimer(buffId, NormalTimerEndCallBack, 0.1f, life, @params);
        }

        private void CreateLoopTimer(int buffId, int actionId, SkillBuffData data)
        {
            object[] @params = { buffId, actionId, buffId };
            BaseCreateTimer(buffId, LoopTimerEndCallBack, data.ThinkInterval,
                buffMap[buffId].GetDuringTime(), @params);
        }

        private void BaseCreateTimer(int buffId, Action<bool, object[]> ac, float interval, float life,
            object[] @params)
        {
            int timeId = timerService.CreateTimer(ac, interval, life, @params);
            SaveTimerId(buffId, timeId);
        }

        private void NormalTimerEndCallBack(bool isTickEnd, object[] objs)
        {
            if (isTickEnd)
            {
                int id = (int)objs[0];
                TryRemoveBuff(id, false);
            }
        }

        private void LoopTimerEndCallBack(bool isEnd, object[] obj)
        {
            int id = (int)obj[0];
            int triggerId = (int)obj[1];

            if (!isEnd) //Tick 定时间间隔回调
            {
                if (buffMap.ContainsKey(id))
                    Trigger(buffMap[id], BuffActionType.LoopTimer, triggerId);
            }

            if (isEnd) //定时器已经结束
            {
                int buffId = (int)obj[2];
                TryRemoveBuff(buffId, false);
            }
        }

        private void RefreshTimerLife(IBuff buff)
        {
            int buffId = buff.GetBuffId();
            if (buffTimerMap.ContainsKey(buffId))
            {
                int timerId = GetTimerId(buffId, true);
                float life = buff.GetDuringTime();
                timerService.RefreshTimerLife(timerId, life);
            }
        }

        private List<int> tempTimerIdList = new List<int>();

        private void RemoveTimer(int buffId, bool forceClean)
        {
            int timerId = 0;
            if (forceClean) //Buff下所有计时器全部清除
            {
                GetAllTimerId(buffId, ref tempTimerIdList);
                for (int i = 0; i < tempTimerIdList.Count; i++)
                {
                    timerId = tempTimerIdList[i];
                    timerService.DeleteTimer(timerId);
                }
            }
            else
            {
                timerId = GetTimerId(buffId, false);
                if (timerId != SkillDefine.NONE)
                {
                    timerService.DeleteTimer(timerId);
                    //SkillUtil.Log(string.Format("[BUFFMANAGER] Remove Timer  [BUffId->{0}] [TimeId->{1}] ])"
                    //  , buffId, timerId));
                }
            }
        }

        #endregion

        public void OnBuffEventDispatch(BuffActionType type)
        {
            var buffList = buffMap.AsList();
            for (int i = 0; i < buffList.Count; i++)
            {
                var buff = buffList[i];
                if (buff == null)
                {
                    SkillUtil.LogError(string.Format("[BUFFMANAGER] BuffList index->[{0}] 's element is null ", i));
                    continue;
                }

                if (IsBuffActionType(buff, type, out int actionId))
                    Trigger(buff, type, actionId);
            }
        }

        private bool IsBuffActionType(IBuff buff, BuffActionType type, out int actionId)
        {
            actionId = 0;
            if (buff == null)
                return false;
            int buffId = buff.GetBuffId();
            return IsBuffActionType(buffId, type, out actionId);
        }

        private bool IsBuffActionType(int buffId, BuffActionType type, out int actionId)
        {
            actionId = 0;
            var data = SkillUtil.GetSkillBuffData(buffId);
            if (data == null)
                return false;

            foreach (var t in data.Action)
            {
                BuffActionType triggerType = (BuffActionType)t.Key;
                if (type != triggerType) continue;
                actionId = t.Value;
                return true;
            }

            return false;
        }

        private void Trigger(IBuff buff, BuffActionType type, int actionId)
        {
            if (buff.GetIdentifyId() == SkillDefine.NONE)
            {
                var param = buff.GetOwner().CreateBuffParam(mContext.GetSelfActor());
                buff.SetIdentifyId(param.IdentifyId);
            }

            // SkillUtil.Log(string.Format("[BUFFMANAGER] [NAME->{0}] [Trigger]->{1} [ActionID]->{2}   ",
            //    mContext.GetSelfActor().GetTransform().name, type, actionId));
            SkillActionTrigger.Instance.TriggerBuff(buff.GetOwner(), actionId, buff.GetIdentifyId());
        }
    }
}