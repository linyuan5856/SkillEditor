namespace BluePro.Skill
{
    public interface ISkill
    {
        void Init(SkillData data, ISkillContext skillContext);

        CommonParam CreateBuffParam(ISkillActor actor);

        void RemoveBuffParam(int id);
        bool IsSKillEnd(int indentifyId);
        void CastSkill(CommonParam param);

        void AddSkillAction(int actionId, int identifyId);
        void RemoveSkillAction(int actionId, CommonParam param);

        void OnBounceNodeEnd(int identifyId, int activeId);
        void OnProjectileHit(int identifyId);

        void OnProjectileDisappear(int identifyId);
        int GetSkillId();

        int GetSkillLevel();

        void UpdateSkillLevel(int newLv);

        ISkillContext GetContext();

        SkillData GetData();

        CommonParam GetParam(int identifyId,bool isBuff);
    }
}