namespace BluePro.Skill
{
    public interface IBuff
    {
        void SetIdentifyId(int id);
        int GetIdentifyId();
        int GetBuffId();

        /// <summary>
        /// 获得BUFF叠加时间
        /// </summary>
        /// <returns></returns>
        float GetDuringTime();

        ISkill GetOwner();
        ISkillActor GetTarget();
        void Add();
        void Remove();
        void Create(ISkill skill, ISkillActor target, int buffId);
        void Release();
    }


    public interface INormalBuff : IBuff
    {
    }

    public interface IOverLayBuff : IBuff
    {
        bool TryRemove();

        /// <summary>
        /// 获得叠加次数
        /// </summary>
        /// <returns></returns>
        int GetOverlayTime();
    }
}