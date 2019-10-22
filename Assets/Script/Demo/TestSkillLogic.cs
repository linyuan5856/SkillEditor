using BluePro.Skill;
using UnityEngine;

public class TestSkillLogic : BaseMain
{
    public TestSkillActor source;
    [SerializeField] private TestSkillActor[] targets;

    [SerializeField] private TestSkillActor componentNow;
    private int skillIndex = 3;

    protected override void Init()
    {
        base.Init();
        targets = GameObject.FindObjectsOfType<TestSkillActor>();
    }

    void OnGUI()
    {
        float x = 0.8f * Screen.width;
        float y = 0.8f * Screen.height;
        float width = 0.1f * Screen.width >= 100.0f ? 0.1f * Screen.width : 100.0f;
        width = Mathf.Min(200.0f, width);
        float height = 30.0f;
        height = Mathf.Min(40.0f, height);


        var btnRect = new Rect(x, y, width, height);
        if (GUI.Button(btnRect, new GUIContent("Cast Skill", "这是技能1")))
        {
            if (!source)
                return;

            if (targets != null && targets[0] != null)
            {
                componentNow = GetRandomTarget();
                var param = new CommonParam(componentNow);
                //  Debug.LogError(targetNow.name);
                source.CastSkill(skillIndex, param);
            }
        }

        var rect = new Rect(x, y - height, width, height);
        int.TryParse(GUI.TextField(rect, skillIndex.ToString()),
            out skillIndex);

        if (GUI.Button(new Rect(x, y - height * 2, width, height), new GUIContent("Clear All Buff ")))
        {
            if (this.source)
                this.source.GetSkillContext().ClearAllBuff();
            for (int i = 0; i < targets.Length; i++)
            {
                var target = targets[i];
                target.GetSkillContext()?.ClearAllBuff();
            }
        }
    }

    TestSkillActor GetRandomTarget()
    {
        var index = UnityEngine.Random.Range(0, targets.Length);
        var target = this.targets[index];
        if (target != source && target.targetTeamType != SkillTargetTeamType.Friend)
            return target;
        else
            return GetRandomTarget();
    }
}