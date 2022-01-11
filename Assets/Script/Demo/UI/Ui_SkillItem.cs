using System;
using UnityEngine;
using UnityEngine.UI;

public class Ui_SkillItem : MonoBehaviour
{
    private Button btn_skill;
    private Image img_icon;
    private Text txt_Name;
    private SkillData _data;
    private Action<SkillData> callBack;

    public void Init(Action<SkillData> onClickCallBack)
    {
        callBack = onClickCallBack;
        btn_skill = GetComponent<Button>();
        img_icon = GetComponentInChildren<Image>();
        txt_Name = GetComponentInChildren<Text>();
        btn_skill.onClick.AddListener(OnButtonClick);
    }

    public void UpdateUI(SkillData data)
    {
        if (data == null) return;
        _data = data;
        txt_Name.text = data.Name;
    }

    private void OnButtonClick()
    {
        if (_data != null)
            callBack?.Invoke(_data);
    }
}