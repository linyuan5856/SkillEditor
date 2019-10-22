namespace BluePro.Skill
{
    public class OverlayBuff : Buff, IOverLayBuff
    {
        private int time; //是否叠加

        public override void Create(ISkill skill, ISkillActor target, int buffId)
        {
            base.Create(skill, target, buffId);
            this.time = 1;
        }

        public int GetOverlayTime()
        {
            return time;
        }

        public override void Add()
        {
            base.Add();
            this.time++;
        }

        /// <summary>
        /// 尝试移除Buff
        /// </summary>
        public bool TryRemove()
        {
            this.time--;
            var canRemove = time <= 0;
            if (canRemove)
                this.Remove();
            return canRemove;
        }

        public override void Release()
        {
            base.Release();
            this.time = 0;
        }
    }
}