using System.Collections.Generic;

public class SkillStateTable : BaseTable
{
    public List<SkillStateData> Datas;

    protected override BaseRow CreateRow()
    {
        return new SkillStateData();
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
        Datas = this.datas.ConvertAll(item => item as SkillStateData);
    }
}