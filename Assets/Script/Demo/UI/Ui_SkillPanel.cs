using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ui_SkillPanel : MonoBehaviour
{
    private readonly List<Ui_SkillItem> _uiSkillItems = new(8);
    [SerializeField] private Button btn_clearBuff;

    void Start()
    {
        btn_clearBuff.onClick.AddListener(OnClearBuffClick);
        GetComponentsInChildren(true, _uiSkillItems);
        foreach (var ui in _uiSkillItems)
            ui.Init(OnSkillClick);
        var data = GetSkillData();
        UpdateUi(data);
    }

    List<SkillData> GetSkillData()
    {
        List<SkillData> datas = new List<SkillData>();
        foreach (var id in DemoEntry.Instance.SkillList)
            datas.Add(SkillUtil.GetSkillData(id));
        return datas;
    }


    void UpdateUi(List<SkillData> dataList)
    {
        for (int i = 0; i < _uiSkillItems.Count; i++)
        {
            var data = i < dataList.Count ? dataList[i] : null;
            _uiSkillItems[i].UpdateUI(data);
        }
    }

    private void OnSkillClick(SkillData data)
    {
        DemoEntry.Instance.CastSkill(data);
    }

    private void OnClearBuffClick()
    {
        DemoEntry.Instance.ClearAllBuff();
    }
}