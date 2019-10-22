using System;

namespace BluePro.Skill
{
    public class Buff : IBuff
    {
        private ISkill mOwner;
        private ISkillActor mTarget;
        private SkillBuffData data;
        private float duration = SkillDefine.NONE;
        private int buffIdentifyId = SkillDefine.NONE;


        public virtual void Create(ISkill skill, ISkillActor target, int buffId)
        {
            this.mOwner = skill;
            this.mTarget = target;
            this.data = SkillUtil.GetSkillBuffData(buffId);
        }

        public virtual void Release()
        {
            if (buffIdentifyId != SkillDefine.NONE)
                GetOwner().RemoveBuffParam(buffIdentifyId);

            this.mOwner = null;
            this.mTarget = null;
            this.data = null;
            this.buffIdentifyId = SkillDefine.NONE;
        }

        public virtual void Add()
        {
            if (data == null)
                return;
            if (!data.IsHidden)
                this.GetTarget()?.AddBuffEffect(data.Effect, data.Attach);
        }

        public virtual void Remove()
        {
            if (data == null)
                return;
            if (!data.IsHidden)
                this.GetTarget()?.RemoveBuffEffect(data.Effect, data.Attach);
        }

        public float GetDuringTime()
        {
            if (Math.Abs(duration - SkillDefine.NONE) < 0.01)
                duration = SkillUtil.GetBuffDurationTime(this.GetOwner().GetSkillLevel(), data.Duration);
            return duration;
        }
        
        public void SetIdentifyId(int id)
        {
            buffIdentifyId = id;
        }

        public int GetIdentifyId()
        {
            return buffIdentifyId;
        }

        public int GetBuffId()
        {
            return this.data?.Id ?? 0;
        }

        public ISkill GetOwner()
        {
            return mOwner;
        }

        public ISkillActor GetTarget()
        {
            return mTarget;
        }
    }
}