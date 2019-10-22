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
        bool Damage(ISkill skill, int value);

        bool Heal(ISkill skill, int value);

        //---------------------------------Buff ---------------------------------
        bool AddBuffEffect(string effectName, string dummyPoint);

        bool RemoveBuffEffect(string effectName, string dummyPoint);

        void ModifySpeed(int value);

        void ModifyAttack(int value);

        void ModifyArmor(int value);

        void AddState(ActorSkillState state);

        void RemoveState(ActorSkillState state);

        //--------------------------------Target------------------------------------------

        SkillTargetTeamType GetTargetTeamType();

        SkillTargetType GetTargetType();

        SkillTargetFlag GetTargetFlag();
    }
}