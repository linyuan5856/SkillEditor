namespace BluePro.Skill
{
    public interface ISkillActor
    {
        ISkillContext GetSkillContext();
        int GetIdentifyId();
        UnityEngine.Transform GetTransform();

        bool PlayAnimation(string anim);

        bool IsDead();
        
        bool CheckManaValid(int skillCost);

        //---------------------------------Action ---------------------------------
        void ModifyProp(ESkillProp prop,ISkill skill,int value);

        //---------------------------------Buff ---------------------------------
        bool AddBuffEffect(string effectName, string dummyPoint);

        bool RemoveBuffEffect(string effectName, string dummyPoint);
        void ModifyProp(ESkillProp prop,int value);

        void AddState(EActorSkillState state);

        void RemoveState(EActorSkillState state);

        //--------------------------------Target------------------------------------------

        ESkillTargetTeamType GetTargetTeamType();

        ESkillTargetType GetTargetType();

        ESkillTargetFlag GetTargetFlag();
    }
}