using UnityEngine;

#pragma warning disable 0649
[System.Serializable]
public class SkillStateData : BaseRow
{
    [SerializeField] private int id;
    [SerializeField] private int stateType;
    [SerializeField] private string para1;
    [SerializeField] private string para2;
    [SerializeField] private string para3;
    [SerializeField] private string para4;

    public int Id => id;

    public int StateType => stateType;

    public string Para1 => para1;

    public string Para2 => para2;

    public string Para3 => para3;

    public string Para4 => para4;

    public override int GetId()
    {
        return this.id;
    }
}