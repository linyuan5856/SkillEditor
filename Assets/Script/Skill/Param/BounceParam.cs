using System.Collections.Generic;

namespace BluePro.Skill
{
    public class BounceParam
    {
        private int bounceTime; //已经弹射次数
        private int bounceActionId; //弹射的Action ID
        private int searchTargetActionId; //弹射寻找目标的Action ID(默认选择弹射关联ACTION列表第一个)
        private List<ISkillActor> bouncedTargets; //已经被弹射过的目标
        private ISkillActor lastBouncedActor; //当前弹射的目标,下次目标的发起点

        public int BounceActionId => bounceActionId;
        public int BounceTime => bounceTime;
        public ISkillActor LastBouncedActor => lastBouncedActor;
        public int SearchTargetActionId => searchTargetActionId;

        /// <summary>
        /// 设置弹射状态
        /// </summary>
        /// <param name="bBounce"></param>
        public void SetBounceState(bool bBounce, int actionId, int searchActionId)
        {
            this.bounceTime = bBounce ? 1 : 0;
            this.bounceActionId = actionId;
            this.searchTargetActionId = searchActionId;
            if (!bBounce)
            {
                this.bouncedTargets.Clear();
                this.lastBouncedActor = null;
            }
        }

        public void AddBounceTime()
        {
            var newTime = this.bounceTime + 1;
            this.bounceTime = newTime;
        }

        public void AddLastBouncedTarget(ISkillActor actor)
        {
            if (actor == null)
                return;
            if (bouncedTargets == null)
                bouncedTargets = new List<ISkillActor>();
            bouncedTargets.Add(actor);
            lastBouncedActor = actor;
        }

        public bool HasBounced(ISkillActor actor)
        {
            if (bouncedTargets == null || actor == null)
                return false;
            return bouncedTargets.Contains(actor);
        }
    }
}