namespace BluePro.Skill
{
    public class SkillComponent : BaseSkillContext
    {
        public SkillComponent(ISkillActor actor) : base(actor)
        {
        }

        public override bool CheckManaValid(int skillCost)
        {
            return true;
        }


        public void CastSkill(int index, CommonParam param)
        {
            GetSkillManager()?.CastSkillByIndex(index, param);
        }

        public void CastSkillById(int id, CommonParam param)
        {
            GetSkillManager()?.CastSkill(id, param);
        }
    }
}