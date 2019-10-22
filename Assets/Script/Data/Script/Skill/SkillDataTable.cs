using System.Collections.Generic;

public class SkillDataTable : BaseTable
{
    public List<SkillData> Datas;

    protected override BaseRow CreateRow()
    {
        return new SkillData();
    }

    public override List<BaseRow> GetList()
    {
        if (m_BaseRowList == null)
            m_BaseRowList = Datas.ConvertAll(item => item as BaseRow); //todo not a good choice
        return m_BaseRowList;
    }

    protected override void AfterDeserialize()
    {
        base.AfterDeserialize();
        Datas = this.datas.ConvertAll(item => item as SkillData);
    }
}